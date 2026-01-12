using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Purchase;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Purchase;

/// <summary>
/// 採購單 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PurchaseOrderRepository : BaseRepository, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PurchaseOrder?> GetByIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseOrders 
                WHERE OrderId = @OrderId";

            return await QueryFirstOrDefaultAsync<PurchaseOrder>(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<PurchaseOrderDetail?> GetDetailByIdAsync(Guid detailId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseOrderDetails 
                WHERE DetailId = @DetailId";

            return await QueryFirstOrDefaultAsync<PurchaseOrderDetail>(sql, new { DetailId = detailId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購單明細失敗: {detailId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseOrderDetail>> GetDetailsByOrderIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseOrderDetails 
                WHERE OrderId = @OrderId 
                ORDER BY LineNum";

            return await QueryAsync<PurchaseOrderDetail>(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購單明細失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseOrder>> QueryAsync(PurchaseOrderQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PurchaseOrders 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.OrderId))
            {
                sql += " AND OrderId LIKE @OrderId";
                parameters.Add("OrderId", $"%{query.OrderId}%");
            }

            if (!string.IsNullOrEmpty(query.OrderType))
            {
                sql += " AND OrderType = @OrderType";
                parameters.Add("OrderType", query.OrderType);
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

            if (!string.IsNullOrEmpty(query.SourceProgram))
            {
                sql += " AND SourceProgram = @SourceProgram";
                parameters.Add("SourceProgram", query.SourceProgram);
            }

            sql += " ORDER BY OrderDate DESC, OrderId DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<PurchaseOrder>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購單列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PurchaseOrderQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM PurchaseOrders 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.OrderId))
            {
                sql += " AND OrderId LIKE @OrderId";
                parameters.Add("OrderId", $"%{query.OrderId}%");
            }

            if (!string.IsNullOrEmpty(query.OrderType))
            {
                sql += " AND OrderType = @OrderType";
                parameters.Add("OrderType", query.OrderType);
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

            if (!string.IsNullOrEmpty(query.SourceProgram))
            {
                sql += " AND SourceProgram = @SourceProgram";
                parameters.Add("SourceProgram", query.SourceProgram);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購單數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(PurchaseOrder entity, List<PurchaseOrderDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 產生採購單號
            var orderId = await GenerateOrderIdAsync(connection, transaction);
            entity.OrderId = orderId;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            // 計算總金額和總數量
            entity.TotalQty = details.Sum(d => d.OrderQty);
            entity.TotalAmount = details.Sum(d => (d.Amount ?? (d.OrderQty * (d.UnitPrice ?? 0))));

            // 新增主檔
            const string insertMainSql = @"
                INSERT INTO PurchaseOrders 
                (OrderId, OrderDate, OrderType, ShopId, SupplierId, Status,
                 ApplyUserId, ApplyDate, ApproveUserId, ApproveDate,
                 TotalAmount, TotalQty, Memo, ExpectedDate, SiteId, SourceProgram,
                 CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@OrderId, @OrderDate, @OrderType, @ShopId, @SupplierId, @Status,
                 @ApplyUserId, @ApplyDate, @ApproveUserId, @ApproveDate,
                 @TotalAmount, @TotalQty, @Memo, @ExpectedDate, @SiteId, @SourceProgram,
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await connection.ExecuteAsync(insertMainSql, entity, transaction);

            // 新增明細
            foreach (var detail in details)
            {
                detail.OrderId = orderId;
                detail.DetailId = Guid.NewGuid();
                detail.CreatedAt = DateTime.Now;

                // 計算金額
                if (!detail.Amount.HasValue && detail.UnitPrice.HasValue)
                {
                    detail.Amount = detail.OrderQty * detail.UnitPrice.Value;
                }

                const string insertDetailSql = @"
                    INSERT INTO PurchaseOrderDetails 
                    (DetailId, OrderId, LineNum, GoodsId, BarcodeId,
                     OrderQty, UnitPrice, Amount, ReceivedQty, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @OrderId, @LineNum, @GoodsId, @BarcodeId,
                     @OrderQty, @UnitPrice, @Amount, @ReceivedQty, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"建立採購單成功: {orderId}");
            return orderId;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立採購單失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(PurchaseOrder entity, List<PurchaseOrderDetail> details)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            entity.UpdatedAt = DateTime.Now;

            // 計算總金額和總數量
            entity.TotalQty = details.Sum(d => d.OrderQty);
            entity.TotalAmount = details.Sum(d => (d.Amount ?? (d.OrderQty * (d.UnitPrice ?? 0))));

            // 更新主檔
            const string updateMainSql = @"
                UPDATE PurchaseOrders SET
                    OrderDate = @OrderDate,
                    OrderType = @OrderType,
                    ShopId = @ShopId,
                    SupplierId = @SupplierId,
                    TotalAmount = @TotalAmount,
                    TotalQty = @TotalQty,
                    Memo = @Memo,
                    ExpectedDate = @ExpectedDate,
                    SiteId = @SiteId,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE OrderId = @OrderId AND Status = 'D'";

            var affectedRows = await connection.ExecuteAsync(updateMainSql, entity, transaction);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("只能修改草稿狀態的採購單");
            }

            // 刪除舊明細
            const string deleteDetailsSql = "DELETE FROM PurchaseOrderDetails WHERE OrderId = @OrderId";
            await connection.ExecuteAsync(deleteDetailsSql, new { OrderId = entity.OrderId }, transaction);

            // 新增新明細
            foreach (var detail in details)
            {
                if (detail.DetailId.Equals(Guid.Empty))
                {
                    detail.DetailId = Guid.NewGuid();
                }
                detail.CreatedAt = DateTime.Now;

                // 計算金額
                if (!detail.Amount.HasValue && detail.UnitPrice.HasValue)
                {
                    detail.Amount = detail.OrderQty * detail.UnitPrice.Value;
                }

                const string insertDetailSql = @"
                    INSERT INTO PurchaseOrderDetails 
                    (DetailId, OrderId, LineNum, GoodsId, BarcodeId,
                     OrderQty, UnitPrice, Amount, ReceivedQty, Memo,
                     CreatedBy, CreatedAt)
                    VALUES 
                    (@DetailId, @OrderId, @LineNum, @GoodsId, @BarcodeId,
                     @OrderQty, @UnitPrice, @Amount, @ReceivedQty, @Memo,
                     @CreatedBy, @CreatedAt)";

                await connection.ExecuteAsync(insertDetailSql, detail, transaction);
            }

            transaction.Commit();
            _logger.LogInfo($"更新採購單成功: {entity.OrderId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新採購單失敗: {entity.OrderId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string orderId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 檢查狀態
            var order = await GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("採購單不存在");
            }

            if (order.Status != "D")
            {
                throw new InvalidOperationException("只能刪除草稿狀態的採購單");
            }

            // 刪除明細
            const string deleteDetailsSql = "DELETE FROM PurchaseOrderDetails WHERE OrderId = @OrderId";
            await connection.ExecuteAsync(deleteDetailsSql, new { OrderId = orderId }, transaction);

            // 刪除主檔
            const string deleteMainSql = "DELETE FROM PurchaseOrders WHERE OrderId = @OrderId";
            await connection.ExecuteAsync(deleteMainSql, new { OrderId = orderId }, transaction);

            transaction.Commit();
            _logger.LogInfo($"刪除採購單成功: {orderId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"刪除採購單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string orderId, string status, string? userId = null, IDbTransaction? transaction = null)
    {
        try
        {
            var updateSql = @"
                UPDATE PurchaseOrders SET
                    Status = @Status,
                    UpdatedAt = @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("OrderId", orderId);
            parameters.Add("Status", status);
            parameters.Add("UpdatedAt", DateTime.Now);

            // 使用傳入的 userId，如果沒有則使用 SYSTEM
            var currentUserId = userId ?? "SYSTEM";

            if (status == "S")
            {
                // 已送出
                updateSql += ", ApplyUserId = @ApplyUserId, ApplyDate = @ApplyDate";
                parameters.Add("ApplyUserId", currentUserId);
                parameters.Add("ApplyDate", DateTime.Now);
            }
            else if (status == "A")
            {
                // 已審核
                updateSql += ", ApproveUserId = @ApproveUserId, ApproveDate = @ApproveDate";
                parameters.Add("ApproveUserId", currentUserId);
                parameters.Add("ApproveDate", DateTime.Now);
            }

            updateSql += " WHERE OrderId = @OrderId";

            if (transaction != null)
            {
                await ExecuteAsync(updateSql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(updateSql, parameters);
            }

            _logger.LogInfo($"更新採購單狀態成功: {orderId}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新採購單狀態失敗: {orderId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 產生採購單號（公開方法）
    /// </summary>
    public async Task<string> GenerateOrderIdAsync()
    {
        const string sql = @"
            SELECT TOP 1 OrderId 
            FROM PurchaseOrders 
            WHERE OrderId LIKE @Pattern 
            ORDER BY OrderId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"PO{today}%";
        var lastOrder = await QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern });

        if (string.IsNullOrEmpty(lastOrder))
        {
            return $"PO{today}001";
        }

        var sequence = int.Parse(lastOrder.Substring(10)) + 1;
        return $"PO{today}{sequence:D3}";
    }

    /// <summary>
    /// 產生採購單號（內部方法，用於交易中）
    /// </summary>
    private async Task<string> GenerateOrderIdAsync(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction)
    {
        const string sql = @"
            SELECT TOP 1 OrderId 
            FROM PurchaseOrders 
            WHERE OrderId LIKE @Pattern 
            ORDER BY OrderId DESC";

        var today = DateTime.Now.ToString("yyyyMMdd");
        var pattern = $"PO{today}%";
        var lastOrder = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Pattern = pattern }, transaction);

        if (string.IsNullOrEmpty(lastOrder))
        {
            return $"PO{today}001";
        }

        var sequence = int.Parse(lastOrder.Substring(10)) + 1;
        return $"PO{today}{sequence:D3}";
    }

    /// <summary>
    /// 更新採購單明細已收數量
    /// </summary>
    public async Task UpdateReceiptQtyAsync(Guid orderDetailId, decimal receiptQty, System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                UPDATE PurchaseOrderDetails 
                SET ReceivedQty = ReceivedQty + @ReceiptQty
                WHERE DetailId = @OrderDetailId";

            var parameters = new DynamicParameters();
            parameters.Add("OrderDetailId", orderDetailId);
            parameters.Add("ReceiptQty", receiptQty);

            if (transaction != null)
            {
                await ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(sql, parameters);
            }

            _logger.LogInfo($"更新採購單明細已收數量成功: {orderDetailId}, 數量: {receiptQty}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新採購單明細已收數量失敗: {orderDetailId}", ex);
            throw;
        }
    }
}

