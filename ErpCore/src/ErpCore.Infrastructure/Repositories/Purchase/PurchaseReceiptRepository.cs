using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Purchase;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Purchase;

/// <summary>
/// 採購驗收單 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PurchaseReceiptRepository : BaseRepository, IPurchaseReceiptRepository
{
    public PurchaseReceiptRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PurchaseReceipt?> GetByIdAsync(string receiptId)
    {
        const string sql = @"
            SELECT * FROM PurchaseReceipts 
            WHERE ReceiptId = @ReceiptId";

        return await QueryFirstOrDefaultAsync<PurchaseReceipt>(sql, new { ReceiptId = receiptId });
    }

    public async Task<PurchaseReceiptDetail?> GetDetailByIdAsync(Guid detailId)
    {
        const string sql = @"
            SELECT * FROM PurchaseReceiptDetails 
            WHERE DetailId = @DetailId";

        return await QueryFirstOrDefaultAsync<PurchaseReceiptDetail>(sql, new { DetailId = detailId });
    }

    public async Task<IEnumerable<PurchaseReceiptDetail>> GetDetailsByReceiptIdAsync(string receiptId)
    {
        const string sql = @"
            SELECT * FROM PurchaseReceiptDetails 
            WHERE ReceiptId = @ReceiptId 
            ORDER BY LineNum";

        return await QueryAsync<PurchaseReceiptDetail>(sql, new { ReceiptId = receiptId });
    }

    public async Task<IEnumerable<PurchaseReceipt>> QueryAsync(PurchaseReceiptQuery query)
    {
        var sql = @"
            SELECT * FROM PurchaseReceipts 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.OrderId))
        {
            sql += " AND OrderId LIKE @OrderId";
            parameters.Add("OrderId", $"%{query.OrderId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
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

        if (!string.IsNullOrEmpty(query.SourceProgram))
        {
            sql += " AND SourceProgram = @SourceProgram";
            parameters.Add("SourceProgram", query.SourceProgram);
        }

        sql += " ORDER BY ReceiptDate DESC, ReceiptId DESC";
        sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<PurchaseReceipt>(sql, parameters);
    }

    public async Task<int> GetCountAsync(PurchaseReceiptQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM PurchaseReceipts 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.OrderId))
        {
            sql += " AND OrderId LIKE @OrderId";
            parameters.Add("OrderId", $"%{query.OrderId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
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

        if (!string.IsNullOrEmpty(query.SourceProgram))
        {
            sql += " AND SourceProgram = @SourceProgram";
            parameters.Add("SourceProgram", query.SourceProgram);
        }

        return await ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<string> CreateAsync(PurchaseReceipt entity, List<PurchaseReceiptDetail> details)
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
                INSERT INTO PurchaseReceipts 
                (ReceiptId, OrderId, ReceiptDate, ShopId, SupplierId, Status, 
                 ReceiptUserId, TotalAmount, TotalQty, Memo, IsSettled, SettledDate, 
                 PurchaseOrderType, IsSettledAdjustment, OriginalReceiptId, AdjustmentReason,
                 CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ReceiptId, @OrderId, @ReceiptDate, @ShopId, @SupplierId, @Status,
                 @ReceiptUserId, @TotalAmount, @TotalQty, @Memo, @IsSettled, @SettledDate,
                 @PurchaseOrderType, @IsSettledAdjustment, @OriginalReceiptId, @AdjustmentReason,
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await connection.ExecuteAsync(insertMainSql, entity, transaction);

            // 新增明細
            foreach (var detail in details)
            {
                detail.ReceiptId = receiptId;
                detail.DetailId = Guid.NewGuid();
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO PurchaseReceiptDetails 
                    (DetailId, ReceiptId, OrderDetailId, LineNum, GoodsId, BarcodeId,
                     OrderQty, ReceiptQty, UnitPrice, Amount, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @ReceiptId, @OrderDetailId, @LineNum, @GoodsId, @BarcodeId,
                     @OrderQty, @ReceiptQty, @UnitPrice, @Amount, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"建立採購驗收單成功: {receiptId}");
            return receiptId;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立採購驗收單失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(PurchaseReceipt entity, List<PurchaseReceiptDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            entity.UpdatedAt = DateTime.Now;

            // 更新主檔
            const string updateMainSql = @"
                UPDATE PurchaseReceipts SET
                    ReceiptDate = @ReceiptDate,
                    ReceiptUserId = @ReceiptUserId,
                    TotalAmount = @TotalAmount,
                    TotalQty = @TotalQty,
                    Memo = @Memo,
                    AdjustmentReason = @AdjustmentReason,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ReceiptId = @ReceiptId";

            await connection.ExecuteAsync(updateMainSql, entity, transaction);

            // 刪除舊明細
            const string deleteDetailsSql = "DELETE FROM PurchaseReceiptDetails WHERE ReceiptId = @ReceiptId";
            await connection.ExecuteAsync(deleteDetailsSql, new { ReceiptId = entity.ReceiptId }, transaction);

            // 新增新明細
            foreach (var detail in details)
            {
                if (detail.DetailId.Equals(Guid.Empty))
                {
                    detail.DetailId = Guid.NewGuid();
                }
                detail.CreatedAt = DateTime.Now;

                const string insertDetailSql = @"
                    INSERT INTO PurchaseReceiptDetails 
                    (DetailId, ReceiptId, OrderDetailId, LineNum, GoodsId, BarcodeId,
                     OrderQty, ReceiptQty, UnitPrice, Amount, 
                     OriginalReceiptQty, AdjustmentQty, OriginalUnitPrice, AdjustmentPrice,
                     Memo, CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @ReceiptId, @OrderDetailId, @LineNum, @GoodsId, @BarcodeId,
                     @OrderQty, @ReceiptQty, @UnitPrice, @Amount,
                     @OriginalReceiptQty, @AdjustmentQty, @OriginalUnitPrice, @AdjustmentPrice,
                     @Memo, @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"更新採購驗收單成功: {entity.ReceiptId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新採購驗收單失敗: {entity.ReceiptId}", ex);
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
            const string deleteDetailsSql = "DELETE FROM PurchaseReceiptDetails WHERE ReceiptId = @ReceiptId";
            await connection.ExecuteAsync(deleteDetailsSql, new { ReceiptId = receiptId }, transaction);

            // 刪除主檔
            const string deleteMainSql = "DELETE FROM PurchaseReceipts WHERE ReceiptId = @ReceiptId";
            await connection.ExecuteAsync(deleteMainSql, new { ReceiptId = receiptId }, transaction);

            transaction.Commit();
            _logger.LogInfo($"刪除採購驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"刪除採購驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PendingPurchaseOrderForReceipt>> GetPendingOrdersAsync(PendingPurchaseOrderQuery query)
    {
        // 此處需要根據實際的採購單表結構調整 SQL
        // 假設有 PurchaseOrders 表
        var sql = @"
            SELECT 
                p.OrderId,
                p.OrderDate,
                p.ShopId,
                p.SupplierId,
                p.Status,
                ISNULL(SUM(pd.OrderQty), 0) AS TotalQty,
                ISNULL(SUM(pr.ReceiptQty), 0) AS ReceiptQty,
                ISNULL(SUM(pd.OrderQty), 0) - ISNULL(SUM(pr.ReceiptQty), 0) AS PendingReceiptQty
            FROM PurchaseOrders p
            LEFT JOIN PurchaseOrderDetails pd ON p.OrderId = pd.OrderId
            LEFT JOIN PurchaseReceiptDetails pr ON pd.DetailId = pr.OrderDetailId
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.OrderId))
        {
            sql += " AND p.OrderId LIKE @OrderId";
            parameters.Add("OrderId", $"%{query.OrderId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND p.ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND p.SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
        }

        if (query.OrderDateFrom.HasValue)
        {
            sql += " AND p.OrderDate >= @OrderDateFrom";
            parameters.Add("OrderDateFrom", query.OrderDateFrom);
        }

        if (query.OrderDateTo.HasValue)
        {
            sql += " AND p.OrderDate <= @OrderDateTo";
            parameters.Add("OrderDateTo", query.OrderDateTo);
        }

        sql += @"
            GROUP BY p.OrderId, p.OrderDate, p.ShopId, p.SupplierId, p.Status
            HAVING ISNULL(SUM(pd.OrderQty), 0) - ISNULL(SUM(pr.ReceiptQty), 0) > 0
            ORDER BY p.OrderDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<PendingPurchaseOrderForReceipt>(sql, parameters);
    }

    public async Task<int> GetPendingOrdersCountAsync(PendingPurchaseOrderQuery query)
    {
        var sql = @"
            SELECT COUNT(DISTINCT p.OrderId)
            FROM PurchaseOrders p
            LEFT JOIN PurchaseOrderDetails pd ON p.OrderId = pd.OrderId
            LEFT JOIN PurchaseReceiptDetails pr ON pd.DetailId = pr.OrderDetailId
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.OrderId))
        {
            sql += " AND p.OrderId LIKE @OrderId";
            parameters.Add("OrderId", $"%{query.OrderId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND p.ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND p.SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
        }

        if (query.OrderDateFrom.HasValue)
        {
            sql += " AND p.OrderDate >= @OrderDateFrom";
            parameters.Add("OrderDateFrom", query.OrderDateFrom);
        }

        if (query.OrderDateTo.HasValue)
        {
            sql += " AND p.OrderDate <= @OrderDateTo";
            parameters.Add("OrderDateTo", query.OrderDateTo);
        }

        sql += @"
            GROUP BY p.OrderId
            HAVING ISNULL(SUM(pd.OrderQty), 0) - ISNULL(SUM(pr.ReceiptQty), 0) > 0";

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
                UPDATE PurchaseReceipts 
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
            FROM PurchaseReceipts 
            WHERE ReceiptId LIKE @Pattern 
            ORDER BY ReceiptId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"PR{today}%";
        var lastReceipt = await QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern });

        if (string.IsNullOrEmpty(lastReceipt))
        {
            return $"PR{today}001";
        }

        var sequence = int.Parse(lastReceipt.Substring(10)) + 1;
        return $"PR{today}{sequence:D3}";
    }

    /// <summary>
    /// 產生驗收單號（內部方法，用於交易中）
    /// </summary>
    private async Task<string> GenerateReceiptIdAsync(global::System.Data.IDbConnection connection, global::System.Data.IDbTransaction transaction)
    {
        const string sql = @"
            SELECT TOP 1 ReceiptId 
            FROM PurchaseReceipts 
            WHERE ReceiptId LIKE @Pattern 
            ORDER BY ReceiptId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"PR{today}%";
        var lastReceipt = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern }, transaction);

        if (string.IsNullOrEmpty(lastReceipt))
        {
            return $"PR{today}001";
        }

        var sequence = int.Parse(lastReceipt.Substring(10)) + 1;
        return $"PR{today}{sequence:D3}";
    }

    // ========== SYSW333 - 已日結採購單驗收調整作業 ==========

    public async Task<IEnumerable<PurchaseReceipt>> QuerySettledAdjustmentsAsync(PurchaseReceiptQuery query)
    {
        var sql = @"
            SELECT * FROM PurchaseReceipts 
            WHERE IsSettledAdjustment = 1 AND PurchaseOrderType = '1'";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.OrderId))
        {
            sql += " AND OrderId LIKE @OrderId";
            parameters.Add("OrderId", $"%{query.OrderId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
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

        return await QueryAsync<PurchaseReceipt>(sql, parameters);
    }

    public async Task<int> GetSettledAdjustmentsCountAsync(PurchaseReceiptQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM PurchaseReceipts 
            WHERE IsSettledAdjustment = 1 AND PurchaseOrderType = '1'";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.OrderId))
        {
            sql += " AND OrderId LIKE @OrderId";
            parameters.Add("OrderId", $"%{query.OrderId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
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

        return await QueryFirstOrDefaultAsync<int>(sql, parameters);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetSettledOrdersAsync(SettledOrderQuery query)
    {
        var sql = @"
            SELECT * FROM PurchaseOrders 
            WHERE Status = 'A' AND OrderType = 'PO'";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
        }

        if (query.OrderDateFrom.HasValue)
        {
            sql += " AND OrderDate >= @OrderDateFrom";
            parameters.Add("OrderDateFrom", query.OrderDateFrom);
        }

        if (query.OrderDateTo.HasValue)
        {
            sql += " AND OrderDate <= @OrderDateTo";
            parameters.Add("OrderDateTo", query.OrderDateTo);
        }

        sql += " ORDER BY OrderDate DESC, OrderId DESC";

        return await QueryAsync<PurchaseOrder>(sql, parameters);
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(string orderId)
    {
        const string sql = @"SELECT * FROM PurchaseOrders WHERE OrderId = @OrderId";
        return await QueryFirstOrDefaultAsync<PurchaseOrder>(sql, new { OrderId = orderId });
    }

    // ========== SYSW530 - 已日結退貨單驗退調整作業 ==========

    public async Task<IEnumerable<PurchaseReceipt>> QueryClosedReturnAdjustmentsAsync(PurchaseReceiptQuery query)
    {
        var sql = @"
            SELECT * FROM PurchaseReceipts 
            WHERE IsSettledAdjustment = 1 AND PurchaseOrderType = '2'";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.OrderId))
        {
            sql += " AND OrderId LIKE @OrderId";
            parameters.Add("OrderId", $"%{query.OrderId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
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

        return await QueryAsync<PurchaseReceipt>(sql, parameters);
    }

    public async Task<int> GetClosedReturnAdjustmentsCountAsync(PurchaseReceiptQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM PurchaseReceipts 
            WHERE IsSettledAdjustment = 1 AND PurchaseOrderType = '2'";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ReceiptId))
        {
            sql += " AND ReceiptId LIKE @ReceiptId";
            parameters.Add("ReceiptId", $"%{query.ReceiptId}%");
        }

        if (!string.IsNullOrEmpty(query.OrderId))
        {
            sql += " AND OrderId LIKE @OrderId";
            parameters.Add("OrderId", $"%{query.OrderId}%");
        }

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
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

        return await QueryFirstOrDefaultAsync<int>(sql, parameters);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetClosedReturnOrdersAsync(ClosedReturnOrderQuery query)
    {
        var sql = @"
            SELECT * FROM PurchaseOrders 
            WHERE Status = 'A' AND OrderType = 'RT'";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ShopId))
        {
            sql += " AND ShopId = @ShopId";
            parameters.Add("ShopId", query.ShopId);
        }

        if (!string.IsNullOrEmpty(query.SupplierId))
        {
            sql += " AND SupplierId = @SupplierId";
            parameters.Add("SupplierId", query.SupplierId);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", query.Status);
        }

        sql += " ORDER BY OrderDate DESC, OrderId DESC";

        return await QueryAsync<PurchaseOrder>(sql, parameters);
    }
}

