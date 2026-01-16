using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Transfer;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Transfer;

/// <summary>
/// 調撥短溢單 Repository 實作 (SYSW384)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TransferShortageRepository : BaseRepository, ITransferShortageRepository
{
    public TransferShortageRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TransferShortage?> GetByIdAsync(string shortageId)
    {
        const string sql = @"
            SELECT * FROM TransferShortages 
            WHERE ShortageId = @ShortageId";

        return await QueryFirstOrDefaultAsync<TransferShortage>(sql, new { ShortageId = shortageId });
    }

    public async Task<TransferShortageDetail?> GetDetailByIdAsync(Guid detailId)
    {
        const string sql = @"
            SELECT * FROM TransferShortageDetails 
            WHERE DetailId = @DetailId";

        return await QueryFirstOrDefaultAsync<TransferShortageDetail>(sql, new { DetailId = detailId });
    }

    public async Task<IEnumerable<TransferShortageDetail>> GetDetailsByShortageIdAsync(string shortageId)
    {
        const string sql = @"
            SELECT * FROM TransferShortageDetails 
            WHERE ShortageId = @ShortageId 
            ORDER BY LineNum";

        return await QueryAsync<TransferShortageDetail>(sql, new { ShortageId = shortageId });
    }

    public async Task<IEnumerable<TransferShortage>> QueryAsync(TransferShortageQuery query)
    {
        var sql = @"
            SELECT * FROM TransferShortages 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ShortageId))
        {
            sql += " AND ShortageId LIKE @ShortageId";
            parameters.Add("ShortageId", $"%{query.ShortageId}%");
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

        if (!string.IsNullOrEmpty(query.ProcessType))
        {
            sql += " AND ProcessType = @ProcessType";
            parameters.Add("ProcessType", query.ProcessType);
        }

        if (query.ShortageDateFrom.HasValue)
        {
            sql += " AND ShortageDate >= @ShortageDateFrom";
            parameters.Add("ShortageDateFrom", query.ShortageDateFrom);
        }

        if (query.ShortageDateTo.HasValue)
        {
            sql += " AND ShortageDate <= @ShortageDateTo";
            parameters.Add("ShortageDateTo", query.ShortageDateTo);
        }

        sql += " ORDER BY ShortageDate DESC, ShortageId DESC";
        sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<TransferShortage>(sql, parameters);
    }

    public async Task<int> GetCountAsync(TransferShortageQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM TransferShortages 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ShortageId))
        {
            sql += " AND ShortageId LIKE @ShortageId";
            parameters.Add("ShortageId", $"%{query.ShortageId}%");
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

        if (!string.IsNullOrEmpty(query.ProcessType))
        {
            sql += " AND ProcessType = @ProcessType";
            parameters.Add("ProcessType", query.ProcessType);
        }

        if (query.ShortageDateFrom.HasValue)
        {
            sql += " AND ShortageDate >= @ShortageDateFrom";
            parameters.Add("ShortageDateFrom", query.ShortageDateFrom);
        }

        if (query.ShortageDateTo.HasValue)
        {
            sql += " AND ShortageDate <= @ShortageDateTo";
            parameters.Add("ShortageDateTo", query.ShortageDateTo);
        }

        return await ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<string> CreateAsync(TransferShortage entity, List<TransferShortageDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 產生短溢單號
            var shortageId = await GenerateShortageIdAsync(connection, transaction);

            entity.ShortageId = shortageId;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            // 新增主檔
            const string insertMainSql = @"
                INSERT INTO TransferShortages 
                (ShortageId, TransferId, ReceiptId, ShortageDate, FromShopId, ToShopId, Status, 
                 ProcessType, ProcessUserId, ProcessDate, ApproveUserId, ApproveDate,
                 TotalShortageQty, TotalAmount, ShortageReason, Memo, IsSettled, SettledDate, 
                 CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ShortageId, @TransferId, @ReceiptId, @ShortageDate, @FromShopId, @ToShopId, @Status,
                 @ProcessType, @ProcessUserId, @ProcessDate, @ApproveUserId, @ApproveDate,
                 @TotalShortageQty, @TotalAmount, @ShortageReason, @Memo, @IsSettled, @SettledDate,
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await connection.ExecuteAsync(insertMainSql, entity, transaction);

            // 新增明細
            foreach (var detail in details)
            {
                detail.ShortageId = shortageId;
                detail.DetailId = Guid.NewGuid();
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO TransferShortageDetails 
                    (DetailId, ShortageId, TransferDetailId, ReceiptDetailId, LineNum, GoodsId, BarcodeId,
                     TransferQty, ReceiptQty, ShortageQty, UnitPrice, Amount, ShortageReason, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @ShortageId, @TransferDetailId, @ReceiptDetailId, @LineNum, @GoodsId, @BarcodeId,
                     @TransferQty, @ReceiptQty, @ShortageQty, @UnitPrice, @Amount, @ShortageReason, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"建立調撥短溢單成功: {shortageId}");
            return shortageId;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立調撥短溢單失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(TransferShortage entity, List<TransferShortageDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            entity.UpdatedAt = DateTime.Now;

            // 更新主檔
            const string updateMainSql = @"
                UPDATE TransferShortages SET
                    ShortageDate = @ShortageDate,
                    ProcessType = @ProcessType,
                    ProcessUserId = @ProcessUserId,
                    ProcessDate = @ProcessDate,
                    ApproveUserId = @ApproveUserId,
                    ApproveDate = @ApproveDate,
                    TotalShortageQty = @TotalShortageQty,
                    TotalAmount = @TotalAmount,
                    ShortageReason = @ShortageReason,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ShortageId = @ShortageId";

            await connection.ExecuteAsync(updateMainSql, entity, transaction);

            // 刪除舊明細
            const string deleteDetailsSql = "DELETE FROM TransferShortageDetails WHERE ShortageId = @ShortageId";
            await connection.ExecuteAsync(deleteDetailsSql, new { ShortageId = entity.ShortageId }, transaction);

            // 新增新明細
            foreach (var detail in details)
            {
                if (detail.DetailId.Equals(Guid.Empty))
                {
                    detail.DetailId = Guid.NewGuid();
                }
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO TransferShortageDetails 
                    (DetailId, ShortageId, TransferDetailId, ReceiptDetailId, LineNum, GoodsId, BarcodeId,
                     TransferQty, ReceiptQty, ShortageQty, UnitPrice, Amount, ShortageReason, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @ShortageId, @TransferDetailId, @ReceiptDetailId, @LineNum, @GoodsId, @BarcodeId,
                     @TransferQty, @ReceiptQty, @ShortageQty, @UnitPrice, @Amount, @ShortageReason, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"更新調撥短溢單成功: {entity.ShortageId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新調撥短溢單失敗: {entity.ShortageId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string shortageId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 刪除明細
            const string deleteDetailsSql = "DELETE FROM TransferShortageDetails WHERE ShortageId = @ShortageId";
            await connection.ExecuteAsync(deleteDetailsSql, new { ShortageId = shortageId }, transaction);

            // 刪除主檔
            const string deleteMainSql = "DELETE FROM TransferShortages WHERE ShortageId = @ShortageId";
            await connection.ExecuteAsync(deleteMainSql, new { ShortageId = shortageId }, transaction);

            transaction.Commit();
            _logger.LogInfo($"刪除調撥短溢單成功: {shortageId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"刪除調撥短溢單失敗: {shortageId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 更新狀態
    /// </summary>
    public async Task UpdateStatusAsync(string shortageId, string status, global::System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                UPDATE TransferShortages 
                SET Status = @Status, UpdatedAt = GETDATE()
                WHERE ShortageId = @ShortageId";

            var parameters = new { ShortageId = shortageId, Status = status };

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(sql, parameters);
            }

            _logger.LogInfo($"更新短溢單狀態成功: ShortageId={shortageId}, Status={status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新短溢單狀態失敗: ShortageId={shortageId}, Status={status}", ex);
            throw;
        }
    }

    /// <summary>
    /// 產生短溢單號（公開方法）
    /// </summary>
    public async Task<string> GenerateShortageIdAsync()
    {
        const string sql = @"
            SELECT TOP 1 ShortageId 
            FROM TransferShortages 
            WHERE ShortageId LIKE @Pattern 
            ORDER BY ShortageId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"TS{today}%";
        var lastShortage = await QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern });

        if (string.IsNullOrEmpty(lastShortage))
        {
            return $"TS{today}001";
        }

        var sequence = int.Parse(lastShortage.Substring(10)) + 1;
        return $"TS{today}{sequence:D3}";
    }

    /// <summary>
    /// 產生短溢單號（內部方法，用於交易中）
    /// </summary>
    private async Task<string> GenerateShortageIdAsync(global::System.Data.IDbConnection connection, global::System.Data.IDbTransaction transaction)
    {
        const string sql = @"
            SELECT TOP 1 ShortageId 
            FROM TransferShortages 
            WHERE ShortageId LIKE @Pattern 
            ORDER BY ShortageId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"TS{today}%";
        var lastShortage = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern }, transaction);

        if (string.IsNullOrEmpty(lastShortage))
        {
            return $"TS{today}001";
        }

        var sequence = int.Parse(lastShortage.Substring(10)) + 1;
        return $"TS{today}{sequence:D3}";
    }
}
