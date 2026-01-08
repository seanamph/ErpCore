using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Sales;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Sales;

/// <summary>
/// 銷售單 Repository 實作 (SYSD110-SYSD140)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SalesOrderRepository : BaseRepository, ISalesOrderRepository
{
    public SalesOrderRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SalesOrder?> GetByIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SalesOrders 
                WHERE OrderId = @OrderId";

            return await QueryFirstOrDefaultAsync<SalesOrder>(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<SalesOrder>> QueryAsync(SalesOrderQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM SalesOrders 
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

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND CustomerId LIKE @CustomerId";
                parameters.Add("CustomerId", $"%{query.CustomerId}%");
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

            sql += " ORDER BY OrderDate DESC, OrderId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<SalesOrder>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售單列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(SalesOrderQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM SalesOrders 
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

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND CustomerId LIKE @CustomerId";
                parameters.Add("CustomerId", $"%{query.CustomerId}%");
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

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售單數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM SalesOrders 
                WHERE OrderId = @OrderId";

            var count = await ExecuteScalarAsync<int>(sql, new { OrderId = orderId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查銷售單是否存在失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<SalesOrder> CreateAsync(SalesOrder salesOrder)
    {
        try
        {
            const string sql = @"
                INSERT INTO SalesOrders (
                    OrderId, OrderDate, OrderType, ShopId, CustomerId, Status,
                    ApplyUserId, ApplyDate, ApproveUserId, ApproveDate, ShipDate,
                    TotalAmount, TotalQty, DiscountAmount, TaxAmount, Memo,
                    ExpectedDate, SiteId, OrgId, CurrencyId, ExchangeRate,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @OrderId, @OrderDate, @OrderType, @ShopId, @CustomerId, @Status,
                    @ApplyUserId, @ApplyDate, @ApproveUserId, @ApproveDate, @ShipDate,
                    @TotalAmount, @TotalQty, @DiscountAmount, @TaxAmount, @Memo,
                    @ExpectedDate, @SiteId, @OrgId, @CurrencyId, @ExchangeRate,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, salesOrder);
            salesOrder.TKey = tKey;

            return salesOrder;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銷售單失敗: {salesOrder.OrderId}", ex);
            throw;
        }
    }

    public async Task<SalesOrder> UpdateAsync(SalesOrder salesOrder)
    {
        try
        {
            const string sql = @"
                UPDATE SalesOrders SET
                    OrderDate = @OrderDate,
                    OrderType = @OrderType,
                    ShopId = @ShopId,
                    CustomerId = @CustomerId,
                    Status = @Status,
                    ApplyUserId = @ApplyUserId,
                    ApplyDate = @ApplyDate,
                    ApproveUserId = @ApproveUserId,
                    ApproveDate = @ApproveDate,
                    ShipDate = @ShipDate,
                    TotalAmount = @TotalAmount,
                    TotalQty = @TotalQty,
                    DiscountAmount = @DiscountAmount,
                    TaxAmount = @TaxAmount,
                    Memo = @Memo,
                    ExpectedDate = @ExpectedDate,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    CurrencyId = @CurrencyId,
                    ExchangeRate = @ExchangeRate,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE OrderId = @OrderId";

            await ExecuteAsync(sql, salesOrder);
            return salesOrder;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷售單失敗: {salesOrder.OrderId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string orderId)
    {
        try
        {
            const string sql = @"
                DELETE FROM SalesOrders 
                WHERE OrderId = @OrderId";

            await ExecuteAsync(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售單失敗: {orderId}", ex);
            throw;
        }
    }
}

