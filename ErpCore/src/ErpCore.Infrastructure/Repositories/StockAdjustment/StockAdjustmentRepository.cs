using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StockAdjustment;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StockAdjustment;

/// <summary>
/// 庫存調整單 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class StockAdjustmentRepository : BaseRepository, IStockAdjustmentRepository
{
    public StockAdjustmentRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<InventoryAdjustment?> GetByIdAsync(string adjustmentId)
    {
        const string sql = @"
            SELECT * FROM InventoryAdjustments 
            WHERE AdjustmentId = @AdjustmentId";

        return await QueryFirstOrDefaultAsync<InventoryAdjustment>(sql, new { AdjustmentId = adjustmentId });
    }

    public async Task<InventoryAdjustmentDetail?> GetDetailByIdAsync(Guid detailId)
    {
        const string sql = @"
            SELECT * FROM InventoryAdjustmentDetails 
            WHERE DetailId = @DetailId";

        return await QueryFirstOrDefaultAsync<InventoryAdjustmentDetail>(sql, new { DetailId = detailId });
    }

    public async Task<IEnumerable<InventoryAdjustmentDetail>> GetDetailsByAdjustmentIdAsync(string adjustmentId)
    {
        const string sql = @"
            SELECT * FROM InventoryAdjustmentDetails 
            WHERE AdjustmentId = @AdjustmentId 
            ORDER BY LineNum";

        return await QueryAsync<InventoryAdjustmentDetail>(sql, new { AdjustmentId = adjustmentId });
    }

    public async Task<IEnumerable<InventoryAdjustment>> QueryAsync(InventoryAdjustmentQuery query)
    {
        var sql = @"
            SELECT * FROM InventoryAdjustments 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.AdjustmentId))
        {
            sql += " AND AdjustmentId LIKE @AdjustmentId";
            parameters.Add("AdjustmentId", $"%{query.AdjustmentId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", query.Status);
        }

        if (query.AdjustmentDateFrom.HasValue)
        {
            sql += " AND AdjustmentDate >= @AdjustmentDateFrom";
            parameters.Add("AdjustmentDateFrom", query.AdjustmentDateFrom);
        }

        if (query.AdjustmentDateTo.HasValue)
        {
            sql += " AND AdjustmentDate <= @AdjustmentDateTo";
            parameters.Add("AdjustmentDateTo", query.AdjustmentDateTo);
        }

        if (!string.IsNullOrEmpty(query.AdjustmentUser))
        {
            sql += " AND AdjustmentUser LIKE @AdjustmentUser";
            parameters.Add("AdjustmentUser", $"%{query.AdjustmentUser}%");
        }

        sql += " ORDER BY AdjustmentDate DESC, AdjustmentId DESC";
        sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<InventoryAdjustment>(sql, parameters);
    }

    public async Task<int> GetCountAsync(InventoryAdjustmentQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM InventoryAdjustments 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.AdjustmentId))
        {
            sql += " AND AdjustmentId LIKE @AdjustmentId";
            parameters.Add("AdjustmentId", $"%{query.AdjustmentId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", query.Status);
        }

        if (query.AdjustmentDateFrom.HasValue)
        {
            sql += " AND AdjustmentDate >= @AdjustmentDateFrom";
            parameters.Add("AdjustmentDateFrom", query.AdjustmentDateFrom);
        }

        if (query.AdjustmentDateTo.HasValue)
        {
            sql += " AND AdjustmentDate <= @AdjustmentDateTo";
            parameters.Add("AdjustmentDateTo", query.AdjustmentDateTo);
        }

        if (!string.IsNullOrEmpty(query.AdjustmentUser))
        {
            sql += " AND AdjustmentUser LIKE @AdjustmentUser";
            parameters.Add("AdjustmentUser", $"%{query.AdjustmentUser}%");
        }

        return await ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<string> CreateAsync(InventoryAdjustment entity, List<InventoryAdjustmentDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 產生調整單號
            var adjustmentId = await GenerateAdjustmentIdAsync(connection, transaction);

            entity.AdjustmentId = adjustmentId;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            // 新增主檔
            const string insertMainSql = @"
                INSERT INTO InventoryAdjustments 
                (AdjustmentId, AdjustmentDate, ShopId, Status, AdjustmentType, AdjustmentUser,
                 Memo, Memo2, SourceNo, SourceNum, SourceCheckDate, SourceSuppId, SiteId,
                 TotalQty, TotalCost, TotalAmount,
                 CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@AdjustmentId, @AdjustmentDate, @ShopId, @Status, @AdjustmentType, @AdjustmentUser,
                 @Memo, @Memo2, @SourceNo, @SourceNum, @SourceCheckDate, @SourceSuppId, @SiteId,
                 @TotalQty, @TotalCost, @TotalAmount,
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await connection.ExecuteAsync(insertMainSql, entity, transaction);

            // 新增明細
            foreach (var detail in details)
            {
                detail.AdjustmentId = adjustmentId;
                detail.DetailId = Guid.NewGuid();
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO InventoryAdjustmentDetails 
                    (DetailId, AdjustmentId, LineNum, GoodsId, BarcodeId,
                     AdjustmentQty, BeforeQty, AfterQty, UnitCost, AdjustmentCost, AdjustmentAmount,
                     Reason, Memo, CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @AdjustmentId, @LineNum, @GoodsId, @BarcodeId,
                     @AdjustmentQty, @BeforeQty, @AfterQty, @UnitCost, @AdjustmentCost, @AdjustmentAmount,
                     @Reason, @Memo, @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"建立庫存調整單成功: {adjustmentId}");
            return adjustmentId;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError("建立庫存調整單失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(InventoryAdjustment entity, List<InventoryAdjustmentDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            entity.UpdatedAt = DateTime.Now;

            // 更新主檔
            const string updateMainSql = @"
                UPDATE InventoryAdjustments SET
                    AdjustmentDate = @AdjustmentDate,
                    AdjustmentType = @AdjustmentType,
                    AdjustmentUser = @AdjustmentUser,
                    Memo = @Memo,
                    Memo2 = @Memo2,
                    SourceNo = @SourceNo,
                    SourceNum = @SourceNum,
                    SourceCheckDate = @SourceCheckDate,
                    SourceSuppId = @SourceSuppId,
                    SiteId = @SiteId,
                    TotalQty = @TotalQty,
                    TotalCost = @TotalCost,
                    TotalAmount = @TotalAmount,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE AdjustmentId = @AdjustmentId";

            await connection.ExecuteAsync(updateMainSql, entity, transaction);

            // 刪除舊明細
            const string deleteDetailsSql = "DELETE FROM InventoryAdjustmentDetails WHERE AdjustmentId = @AdjustmentId";
            await connection.ExecuteAsync(deleteDetailsSql, new { AdjustmentId = entity.AdjustmentId }, transaction);

            // 新增新明細
            foreach (var detail in details)
            {
                if (detail.DetailId.Equals(Guid.Empty))
                {
                    detail.DetailId = Guid.NewGuid();
                }
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO InventoryAdjustmentDetails 
                    (DetailId, AdjustmentId, LineNum, GoodsId, BarcodeId,
                     AdjustmentQty, BeforeQty, AfterQty, UnitCost, AdjustmentCost, AdjustmentAmount,
                     Reason, Memo, CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @AdjustmentId, @LineNum, @GoodsId, @BarcodeId,
                     @AdjustmentQty, @BeforeQty, @AfterQty, @UnitCost, @AdjustmentCost, @AdjustmentAmount,
                     @Reason, @Memo, @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"更新庫存調整單成功: {entity.AdjustmentId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新庫存調整單失敗: {entity.AdjustmentId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string adjustmentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 刪除明細（外鍵約束會自動刪除）
            const string deleteDetailsSql = "DELETE FROM InventoryAdjustmentDetails WHERE AdjustmentId = @AdjustmentId";
            await connection.ExecuteAsync(deleteDetailsSql, new { AdjustmentId = adjustmentId }, transaction);

            // 刪除主檔
            const string deleteMainSql = "DELETE FROM InventoryAdjustments WHERE AdjustmentId = @AdjustmentId";
            await connection.ExecuteAsync(deleteMainSql, new { AdjustmentId = adjustmentId }, transaction);

            transaction.Commit();
            _logger.LogInfo($"刪除庫存調整單成功: {adjustmentId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"刪除庫存調整單失敗: {adjustmentId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string adjustmentId, string status, IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                UPDATE InventoryAdjustments 
                SET Status = @Status, UpdatedAt = GETDATE()
                WHERE AdjustmentId = @AdjustmentId";

            var parameters = new { AdjustmentId = adjustmentId, Status = status };

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(sql, parameters);
            }

            _logger.LogInfo($"更新調整單狀態成功: AdjustmentId={adjustmentId}, Status={status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新調整單狀態失敗: AdjustmentId={adjustmentId}, Status={status}", ex);
            throw;
        }
    }

    public async Task<string> GenerateAdjustmentIdAsync()
    {
        const string sql = @"
            SELECT TOP 1 AdjustmentId 
            FROM InventoryAdjustments 
            WHERE AdjustmentId LIKE @Pattern 
            ORDER BY AdjustmentId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"ADJ{today}%";
        var lastAdjustment = await QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern });

        if (string.IsNullOrEmpty(lastAdjustment))
        {
            return $"ADJ{today}001";
        }

        var sequence = int.Parse(lastAdjustment.Substring(11)) + 1;
        return $"ADJ{today}{sequence:D3}";
    }

    /// <summary>
    /// 產生調整單號（內部方法，用於交易中）
    /// </summary>
    private async Task<string> GenerateAdjustmentIdAsync(IDbConnection connection, IDbTransaction transaction)
    {
        const string sql = @"
            SELECT TOP 1 AdjustmentId 
            FROM InventoryAdjustments 
            WHERE AdjustmentId LIKE @Pattern 
            ORDER BY AdjustmentId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"ADJ{today}%";
        var lastAdjustment = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern }, transaction);

        if (string.IsNullOrEmpty(lastAdjustment))
        {
            return $"ADJ{today}001";
        }

        var sequence = int.Parse(lastAdjustment.Substring(11)) + 1;
        return $"ADJ{today}{sequence:D3}";
    }

    public async Task<IEnumerable<AdjustmentReason>> GetAdjustmentReasonsAsync()
    {
        const string sql = @"
            SELECT ReasonId, ReasonName, ReasonType, Status
            FROM AdjustmentReasons
            WHERE Status = 'A'
            ORDER BY ReasonId";

        return await QueryAsync<AdjustmentReason>(sql);
    }
}

