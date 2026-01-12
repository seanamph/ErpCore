using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Transfer;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Transfer;

/// <summary>
/// 調撥驗退單 Repository 實作 (SYSW362)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TransferReturnRepository : BaseRepository, ITransferReturnRepository
{
    public TransferReturnRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TransferReturn?> GetByIdAsync(string returnId)
    {
        const string sql = @"
            SELECT * FROM TransferReturns 
            WHERE ReturnId = @ReturnId";

        return await QueryFirstOrDefaultAsync<TransferReturn>(sql, new { ReturnId = returnId });
    }

    public async Task<TransferReturnDetail?> GetDetailByIdAsync(Guid detailId)
    {
        const string sql = @"
            SELECT * FROM TransferReturnDetails 
            WHERE DetailId = @DetailId";

        return await QueryFirstOrDefaultAsync<TransferReturnDetail>(sql, new { DetailId = detailId });
    }

    public async Task<IEnumerable<TransferReturnDetail>> GetDetailsByReturnIdAsync(string returnId)
    {
        const string sql = @"
            SELECT * FROM TransferReturnDetails 
            WHERE ReturnId = @ReturnId 
            ORDER BY LineNum";

        return await QueryAsync<TransferReturnDetail>(sql, new { ReturnId = returnId });
    }

    public async Task<IEnumerable<TransferReturn>> QueryAsync(TransferReturnQuery query)
    {
        var sql = @"
            SELECT * FROM TransferReturns 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReturnId))
        {
            sql += " AND ReturnId LIKE @ReturnId";
            parameters.Add("ReturnId", $"%{query.ReturnId}%");
        }

        if (!string.IsNullOrEmpty(query.TransferId))
        {
            sql += " AND TransferId LIKE @TransferId";
            parameters.Add("TransferId", $"%{query.TransferId}%");
        }

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.FromShopId))
        {
            sql += " AND FromShopId = @FromShopId";
            parameters.Add("FromShopId", query.FromShopId);
        }

        if (!string.IsNullOrEmpty(query.ToShopId))
        {
            sql += " AND ToShopId = @ToShopId";
            parameters.Add("ToShopId", query.ToShopId);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", query.Status);
        }

        if (query.ReturnDateFrom.HasValue)
        {
            sql += " AND ReturnDate >= @ReturnDateFrom";
            parameters.Add("ReturnDateFrom", query.ReturnDateFrom);
        }

        if (query.ReturnDateTo.HasValue)
        {
            sql += " AND ReturnDate <= @ReturnDateTo";
            parameters.Add("ReturnDateTo", query.ReturnDateTo);
        }

        sql += " ORDER BY ReturnDate DESC, ReturnId DESC";
        sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<TransferReturn>(sql, parameters);
    }

    public async Task<int> GetCountAsync(TransferReturnQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM TransferReturns 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReturnId))
        {
            sql += " AND ReturnId LIKE @ReturnId";
            parameters.Add("ReturnId", $"%{query.ReturnId}%");
        }

        if (!string.IsNullOrEmpty(query.TransferId))
        {
            sql += " AND TransferId LIKE @TransferId";
            parameters.Add("TransferId", $"%{query.TransferId}%");
        }

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.FromShopId))
        {
            sql += " AND FromShopId = @FromShopId";
            parameters.Add("FromShopId", query.FromShopId);
        }

        if (!string.IsNullOrEmpty(query.ToShopId))
        {
            sql += " AND ToShopId = @ToShopId";
            parameters.Add("ToShopId", query.ToShopId);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", query.Status);
        }

        if (query.ReturnDateFrom.HasValue)
        {
            sql += " AND ReturnDate >= @ReturnDateFrom";
            parameters.Add("ReturnDateFrom", query.ReturnDateFrom);
        }

        if (query.ReturnDateTo.HasValue)
        {
            sql += " AND ReturnDate <= @ReturnDateTo";
            parameters.Add("ReturnDateTo", query.ReturnDateTo);
        }

        return await ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<string> CreateAsync(TransferReturn entity, List<TransferReturnDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 產生驗退單號
            var returnId = await GenerateReturnIdAsync(connection, transaction);

            entity.ReturnId = returnId;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            // 新增主檔
            const string insertMainSql = @"
                INSERT INTO TransferReturns 
                (ReturnId, TransferId, ReceiptId, ReturnDate, FromShopId, ToShopId, Status, 
                 ReturnUserId, TotalAmount, TotalQty, ReturnReason, Memo, IsSettled, SettledDate, 
                 CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ReturnId, @TransferId, @ReceiptId, @ReturnDate, @FromShopId, @ToShopId, @Status,
                 @ReturnUserId, @TotalAmount, @TotalQty, @ReturnReason, @Memo, @IsSettled, @SettledDate,
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await connection.ExecuteAsync(insertMainSql, entity, transaction);

            // 新增明細
            foreach (var detail in details)
            {
                detail.ReturnId = returnId;
                detail.DetailId = Guid.NewGuid();
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO TransferReturnDetails 
                    (DetailId, ReturnId, TransferDetailId, ReceiptDetailId, LineNum, GoodsId, BarcodeId,
                     TransferQty, ReceiptQty, ReturnQty, UnitPrice, Amount, ReturnReason, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @ReturnId, @TransferDetailId, @ReceiptDetailId, @LineNum, @GoodsId, @BarcodeId,
                     @TransferQty, @ReceiptQty, @ReturnQty, @UnitPrice, @Amount, @ReturnReason, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"建立調撥驗退單成功: {returnId}");
            return returnId;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立調撥驗退單失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(TransferReturn entity, List<TransferReturnDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            entity.UpdatedAt = DateTime.Now;

            // 更新主檔
            const string updateMainSql = @"
                UPDATE TransferReturns SET
                    ReturnDate = @ReturnDate,
                    ReturnUserId = @ReturnUserId,
                    TotalAmount = @TotalAmount,
                    TotalQty = @TotalQty,
                    ReturnReason = @ReturnReason,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ReturnId = @ReturnId";

            await connection.ExecuteAsync(updateMainSql, entity, transaction);

            // 刪除舊明細
            const string deleteDetailsSql = "DELETE FROM TransferReturnDetails WHERE ReturnId = @ReturnId";
            await connection.ExecuteAsync(deleteDetailsSql, new { ReturnId = entity.ReturnId }, transaction);

            // 新增新明細
            foreach (var detail in details)
            {
                if (detail.DetailId.Equals(Guid.Empty))
                {
                    detail.DetailId = Guid.NewGuid();
                }
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO TransferReturnDetails 
                    (DetailId, ReturnId, TransferDetailId, ReceiptDetailId, LineNum, GoodsId, BarcodeId,
                     TransferQty, ReceiptQty, ReturnQty, UnitPrice, Amount, ReturnReason, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @ReturnId, @TransferDetailId, @ReceiptDetailId, @LineNum, @GoodsId, @BarcodeId,
                     @TransferQty, @ReceiptQty, @ReturnQty, @UnitPrice, @Amount, @ReturnReason, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"更新調撥驗退單成功: {entity.ReturnId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新調撥驗退單失敗: {entity.ReturnId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string returnId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 刪除明細
            const string deleteDetailsSql = "DELETE FROM TransferReturnDetails WHERE ReturnId = @ReturnId";
            await connection.ExecuteAsync(deleteDetailsSql, new { ReturnId = returnId }, transaction);

            // 刪除主檔
            const string deleteMainSql = "DELETE FROM TransferReturns WHERE ReturnId = @ReturnId";
            await connection.ExecuteAsync(deleteMainSql, new { ReturnId = returnId }, transaction);

            transaction.Commit();
            _logger.LogInfo($"刪除調撥驗退單成功: {returnId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"刪除調撥驗退單失敗: {returnId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PendingTransferOrderForReturn>> GetPendingOrdersAsync(PendingTransferOrderForReturnQuery query)
    {
        // 查詢已驗收但可驗退的調撥單
        var sql = @"
            SELECT 
                t.TransferId,
                t.TransferDate,
                t.FromShopId,
                t.ToShopId,
                t.Status,
                ISNULL(SUM(td.TransferQty), 0) AS TotalQty,
                ISNULL(SUM(tr.ReceiptQty), 0) AS ReceiptQty,
                ISNULL(SUM(trt.ReturnQty), 0) AS ReturnQty,
                ISNULL(SUM(tr.ReceiptQty), 0) - ISNULL(SUM(trt.ReturnQty), 0) AS PendingReturnQty
            FROM TransferOrders t
            LEFT JOIN TransferOrderDetails td ON t.TransferId = td.TransferId
            LEFT JOIN TransferReceiptDetails tr ON td.DetailId = tr.TransferDetailId
            LEFT JOIN TransferReturnDetails trt ON tr.DetailId = trt.ReceiptDetailId
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.TransferId))
        {
            sql += " AND t.TransferId LIKE @TransferId";
            parameters.Add("TransferId", $"%{query.TransferId}%");
        }

        if (!string.IsNullOrEmpty(query.FromShopId))
        {
            sql += " AND t.FromShopId = @FromShopId";
            parameters.Add("FromShopId", query.FromShopId);
        }

        if (!string.IsNullOrEmpty(query.ToShopId))
        {
            sql += " AND t.ToShopId = @ToShopId";
            parameters.Add("ToShopId", query.ToShopId);
        }

        if (query.TransferDateFrom.HasValue)
        {
            sql += " AND t.TransferDate >= @TransferDateFrom";
            parameters.Add("TransferDateFrom", query.TransferDateFrom);
        }

        if (query.TransferDateTo.HasValue)
        {
            sql += " AND t.TransferDate <= @TransferDateTo";
            parameters.Add("TransferDateTo", query.TransferDateTo);
        }

        sql += @"
            GROUP BY t.TransferId, t.TransferDate, t.FromShopId, t.ToShopId, t.Status
            HAVING ISNULL(SUM(tr.ReceiptQty), 0) - ISNULL(SUM(trt.ReturnQty), 0) > 0
            ORDER BY t.TransferDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<PendingTransferOrderForReturn>(sql, parameters);
    }

    public async Task<int> GetPendingOrdersCountAsync(PendingTransferOrderForReturnQuery query)
    {
        var sql = @"
            SELECT COUNT(DISTINCT t.TransferId)
            FROM TransferOrders t
            LEFT JOIN TransferOrderDetails td ON t.TransferId = td.TransferId
            LEFT JOIN TransferReceiptDetails tr ON td.DetailId = tr.TransferDetailId
            LEFT JOIN TransferReturnDetails trt ON tr.DetailId = trt.ReceiptDetailId
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.TransferId))
        {
            sql += " AND t.TransferId LIKE @TransferId";
            parameters.Add("TransferId", $"%{query.TransferId}%");
        }

        if (!string.IsNullOrEmpty(query.FromShopId))
        {
            sql += " AND t.FromShopId = @FromShopId";
            parameters.Add("FromShopId", query.FromShopId);
        }

        if (!string.IsNullOrEmpty(query.ToShopId))
        {
            sql += " AND t.ToShopId = @ToShopId";
            parameters.Add("ToShopId", query.ToShopId);
        }

        if (query.TransferDateFrom.HasValue)
        {
            sql += " AND t.TransferDate >= @TransferDateFrom";
            parameters.Add("TransferDateFrom", query.TransferDateFrom);
        }

        if (query.TransferDateTo.HasValue)
        {
            sql += " AND t.TransferDate <= @TransferDateTo";
            parameters.Add("TransferDateTo", query.TransferDateTo);
        }

        sql += @"
            GROUP BY t.TransferId
            HAVING ISNULL(SUM(tr.ReceiptQty), 0) - ISNULL(SUM(trt.ReturnQty), 0) > 0";

        var result = await QueryAsync<int>(sql, parameters);
        return result.Count();
    }

    /// <summary>
    /// 更新狀態
    /// </summary>
    public async Task UpdateStatusAsync(string returnId, string status, global::System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                UPDATE TransferReturns 
                SET Status = @Status, UpdatedAt = GETDATE()
                WHERE ReturnId = @ReturnId";

            var parameters = new { ReturnId = returnId, Status = status };

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(sql, parameters);
            }

            _logger.LogInfo($"更新驗退單狀態成功: ReturnId={returnId}, Status={status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新驗退單狀態失敗: ReturnId={returnId}, Status={status}", ex);
            throw;
        }
    }

    /// <summary>
    /// 產生驗退單號（公開方法）
    /// </summary>
    public async Task<string> GenerateReturnIdAsync()
    {
        const string sql = @"
            SELECT TOP 1 ReturnId 
            FROM TransferReturns 
            WHERE ReturnId LIKE @Pattern 
            ORDER BY ReturnId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"TR{today}%";
        var lastReturn = await QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern });

        if (string.IsNullOrEmpty(lastReturn))
        {
            return $"TR{today}001";
        }

        var sequence = int.Parse(lastReturn.Substring(10)) + 1;
        return $"TR{today}{sequence:D3}";
    }

    /// <summary>
    /// 產生驗退單號（內部方法，用於交易中）
    /// </summary>
    private async Task<string> GenerateReturnIdAsync(global::System.Data.IDbConnection connection, global::System.Data.IDbTransaction transaction)
    {
        const string sql = @"
            SELECT TOP 1 ReturnId 
            FROM TransferReturns 
            WHERE ReturnId LIKE @Pattern 
            ORDER BY ReturnId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"TR{today}%";
        var lastReturn = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern }, transaction);

        if (string.IsNullOrEmpty(lastReturn))
        {
            return $"TR{today}001";
        }

        var sequence = int.Parse(lastReturn.Substring(10)) + 1;
        return $"TR{today}{sequence:D3}";
    }
}
