using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Transfer;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Transfer;

/// <summary>
/// 調撥驗收單 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TransferReceiptRepository : BaseRepository, ITransferReceiptRepository
{
    public TransferReceiptRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TransferReceipt?> GetByIdAsync(string receiptId)
    {
        const string sql = @"
            SELECT * FROM TransferReceipts 
            WHERE ReceiptId = @ReceiptId";

        return await QueryFirstOrDefaultAsync<TransferReceipt>(sql, new { ReceiptId = receiptId });
    }

    public async Task<TransferReceiptDetail?> GetDetailByIdAsync(Guid detailId)
    {
        const string sql = @"
            SELECT * FROM TransferReceiptDetails 
            WHERE DetailId = @DetailId";

        return await QueryFirstOrDefaultAsync<TransferReceiptDetail>(sql, new { DetailId = detailId });
    }

    public async Task<IEnumerable<TransferReceiptDetail>> GetDetailsByReceiptIdAsync(string receiptId)
    {
        const string sql = @"
            SELECT * FROM TransferReceiptDetails 
            WHERE ReceiptId = @ReceiptId 
            ORDER BY LineNum";

        return await QueryAsync<TransferReceiptDetail>(sql, new { ReceiptId = receiptId });
    }

    public async Task<IEnumerable<TransferReceipt>> QueryAsync(TransferReceiptQuery query)
    {
        var sql = @"
            SELECT * FROM TransferReceipts 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.TransferId))
        {
            sql += " AND TransferId LIKE @TransferId";
            parameters.Add("TransferId", $"%{query.TransferId}%");
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

        if (query.ReceiptDateFrom.HasValue)
        {
            sql += " AND ReceiptDate >= @ReceiptDateFrom";
            parameters.Add("ReceiptDateFrom", query.ReceiptDateFrom);
        }

        if (query.ReceiptDateTo.HasValue)
        {
            sql += " AND ReceiptDate <= @ReceiptDateTo";
            parameters.Add("ReceiptDateTo", query.ReceiptDateTo);
        }

        sql += " ORDER BY ReceiptDate DESC, ReceiptId DESC";
        sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<TransferReceipt>(sql, parameters);
    }

    public async Task<int> GetCountAsync(TransferReceiptQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM TransferReceipts 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.TransferId))
        {
            sql += " AND TransferId LIKE @TransferId";
            parameters.Add("TransferId", $"%{query.TransferId}%");
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

        if (query.ReceiptDateFrom.HasValue)
        {
            sql += " AND ReceiptDate >= @ReceiptDateFrom";
            parameters.Add("ReceiptDateFrom", query.ReceiptDateFrom);
        }

        if (query.ReceiptDateTo.HasValue)
        {
            sql += " AND ReceiptDate <= @ReceiptDateTo";
            parameters.Add("ReceiptDateTo", query.ReceiptDateTo);
        }

        return await ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<string> CreateAsync(TransferReceipt entity, List<TransferReceiptDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 產生驗收單號
            var receiptId = await GenerateReceiptIdAsync(connection, transaction);

            entity.ReceiptId = receiptId;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            // 新增主檔
            const string insertMainSql = @"
                INSERT INTO TransferReceipts 
                (ReceiptId, TransferId, ReceiptDate, FromShopId, ToShopId, Status, 
                 ReceiptUserId, TotalAmount, TotalQty, Memo, IsSettled, SettledDate, 
                 CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ReceiptId, @TransferId, @ReceiptDate, @FromShopId, @ToShopId, @Status,
                 @ReceiptUserId, @TotalAmount, @TotalQty, @Memo, @IsSettled, @SettledDate,
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await connection.ExecuteAsync(insertMainSql, entity, transaction);

            // 新增明細
            foreach (var detail in details)
            {
                detail.ReceiptId = receiptId;
                detail.DetailId = Guid.NewGuid();
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO TransferReceiptDetails 
                    (DetailId, ReceiptId, TransferDetailId, LineNum, GoodsId, BarcodeId,
                     TransferQty, ReceiptQty, UnitPrice, Amount, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @ReceiptId, @TransferDetailId, @LineNum, @GoodsId, @BarcodeId,
                     @TransferQty, @ReceiptQty, @UnitPrice, @Amount, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"建立調撥驗收單成功: {receiptId}");
            return receiptId;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立調撥驗收單失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(TransferReceipt entity, List<TransferReceiptDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            entity.UpdatedAt = DateTime.Now;

            // 更新主檔
            const string updateMainSql = @"
                UPDATE TransferReceipts SET
                    ReceiptDate = @ReceiptDate,
                    ReceiptUserId = @ReceiptUserId,
                    TotalAmount = @TotalAmount,
                    TotalQty = @TotalQty,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ReceiptId = @ReceiptId";

            await connection.ExecuteAsync(updateMainSql, entity, transaction);

            // 刪除舊明細
            const string deleteDetailsSql = "DELETE FROM TransferReceiptDetails WHERE ReceiptId = @ReceiptId";
            await connection.ExecuteAsync(deleteDetailsSql, new { ReceiptId = entity.ReceiptId }, transaction);

            // 新增新明細
            foreach (var detail in details)
            {
                if (!detail.DetailId.Equals(Guid.Empty))
                {
                    detail.DetailId = Guid.NewGuid();
                }
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO TransferReceiptDetails 
                    (DetailId, ReceiptId, TransferDetailId, LineNum, GoodsId, BarcodeId,
                     TransferQty, ReceiptQty, UnitPrice, Amount, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @ReceiptId, @TransferDetailId, @LineNum, @GoodsId, @BarcodeId,
                     @TransferQty, @ReceiptQty, @UnitPrice, @Amount, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"更新調撥驗收單成功: {entity.ReceiptId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新調撥驗收單失敗: {entity.ReceiptId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string receiptId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 刪除明細
            const string deleteDetailsSql = "DELETE FROM TransferReceiptDetails WHERE ReceiptId = @ReceiptId";
            await connection.ExecuteAsync(deleteDetailsSql, new { ReceiptId = receiptId }, transaction);

            // 刪除主檔
            const string deleteMainSql = "DELETE FROM TransferReceipts WHERE ReceiptId = @ReceiptId";
            await connection.ExecuteAsync(deleteMainSql, new { ReceiptId = receiptId }, transaction);

            transaction.Commit();
            _logger.LogInfo($"刪除調撥驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"刪除調撥驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PendingTransferOrderForReceipt>> GetPendingOrdersAsync(PendingTransferOrderQuery query)
    {
        // 此處需要根據實際的調撥單表結構調整 SQL
        // 假設有 TransferOrders 表
        var sql = @"
            SELECT 
                t.TransferId,
                t.TransferDate,
                t.FromShopId,
                t.ToShopId,
                t.Status,
                ISNULL(SUM(td.TransferQty), 0) AS TotalQty,
                ISNULL(SUM(tr.ReceiptQty), 0) AS ReceiptQty,
                ISNULL(SUM(td.TransferQty), 0) - ISNULL(SUM(tr.ReceiptQty), 0) AS PendingReceiptQty
            FROM TransferOrders t
            LEFT JOIN TransferOrderDetails td ON t.TransferId = td.TransferId
            LEFT JOIN TransferReceiptDetails tr ON td.DetailId = tr.TransferDetailId
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
            HAVING ISNULL(SUM(td.TransferQty), 0) - ISNULL(SUM(tr.ReceiptQty), 0) > 0
            ORDER BY t.TransferDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<PendingTransferOrderForReceipt>(sql, parameters);
    }

    public async Task<int> GetPendingOrdersCountAsync(PendingTransferOrderQuery query)
    {
        var sql = @"
            SELECT COUNT(DISTINCT t.TransferId)
            FROM TransferOrders t
            LEFT JOIN TransferOrderDetails td ON t.TransferId = td.TransferId
            LEFT JOIN TransferReceiptDetails tr ON td.DetailId = tr.TransferDetailId
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
            HAVING ISNULL(SUM(td.TransferQty), 0) - ISNULL(SUM(tr.ReceiptQty), 0) > 0";

        var result = await QueryAsync<int>(sql, parameters);
        return result.Count();
    }

    /// <summary>
    /// 更新狀態
    /// </summary>
    public async Task UpdateStatusAsync(string receiptId, string status, global::System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                UPDATE TransferReceipts 
                SET Status = @Status, UpdatedAt = GETDATE()
                WHERE ReceiptId = @ReceiptId";

            var parameters = new { ReceiptId = receiptId, Status = status };

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(sql, parameters);
            }

            _logger.LogInfo($"更新驗收單狀態成功: ReceiptId={receiptId}, Status={status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新驗收單狀態失敗: ReceiptId={receiptId}, Status={status}", ex);
            throw;
        }
    }

    /// <summary>
    /// 產生驗收單號（公開方法）
    /// </summary>
    public async Task<string> GenerateReceiptIdAsync()
    {
        const string sql = @"
            SELECT TOP 1 ReceiptId 
            FROM TransferReceipts 
            WHERE ReceiptId LIKE @Pattern 
            ORDER BY ReceiptId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"TC{today}%";
        var lastReceipt = await QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern });

        if (string.IsNullOrEmpty(lastReceipt))
        {
            return $"TC{today}001";
        }

        var sequence = int.Parse(lastReceipt.Substring(10)) + 1;
        return $"TC{today}{sequence:D3}";
    }

    /// <summary>
    /// 產生驗收單號（內部方法，用於交易中）
    /// </summary>
    private async Task<string> GenerateReceiptIdAsync(global::System.Data.IDbConnection connection, global::System.Data.IDbTransaction transaction)
    {
        const string sql = @"
            SELECT TOP 1 ReceiptId 
            FROM TransferReceipts 
            WHERE ReceiptId LIKE @Pattern 
            ORDER BY ReceiptId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"TC{today}%";
        var lastReceipt = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern }, transaction);

        if (string.IsNullOrEmpty(lastReceipt))
        {
            return $"TC{today}001";
        }

        var sequence = int.Parse(lastReceipt.Substring(10)) + 1;
        return $"TC{today}{sequence:D3}";
    }
}

