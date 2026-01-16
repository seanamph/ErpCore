using System.Data;
using System.Linq;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 銷售單 Repository 實作 (SYSG410-SYSG460 - 銷售資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SalesOrderRepository : BaseRepository, ISalesOrderRepository
{
    public SalesOrderRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SalesOrder?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SalesOrders 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<SalesOrder>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售單失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<SalesOrder?> GetByOrderIdAsync(string orderId)
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

    public async Task<PagedResult<SalesOrder>> QueryAsync(SalesOrderQuery query)
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
                sql += " AND CustomerId = @CustomerId";
                parameters.Add("CustomerId", query.CustomerId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND OrderDate >= @OrderDateFrom";
                parameters.Add("OrderDateFrom", query.OrderDateFrom.Value);
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND OrderDate <= @OrderDateTo";
                parameters.Add("OrderDateTo", query.OrderDateTo.Value);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY CreatedAt DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<SalesOrder>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM SalesOrders
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.OrderId))
            {
                countSql += " AND OrderId LIKE @OrderId";
                countParameters.Add("OrderId", $"%{query.OrderId}%");
            }
            if (!string.IsNullOrEmpty(query.OrderType))
            {
                countSql += " AND OrderType = @OrderType";
                countParameters.Add("OrderType", query.OrderType);
            }
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                countSql += " AND ShopId = @ShopId";
                countParameters.Add("ShopId", query.ShopId);
            }
            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                countSql += " AND CustomerId = @CustomerId";
                countParameters.Add("CustomerId", query.CustomerId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.OrderDateFrom.HasValue)
            {
                countSql += " AND OrderDate >= @OrderDateFrom";
                countParameters.Add("OrderDateFrom", query.OrderDateFrom.Value);
            }
            if (query.OrderDateTo.HasValue)
            {
                countSql += " AND OrderDate <= @OrderDateTo";
                countParameters.Add("OrderDateTo", query.OrderDateTo.Value);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<SalesOrder>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售單列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(SalesOrder salesOrder)
    {
        try
        {
            const string sql = @"
                INSERT INTO SalesOrders (
                    OrderId, OrderDate, OrderType, ShopId, CustomerId, Status,
                    ApplyUserId, ApplyDate, ApproveUserId, ApproveDate,
                    TotalAmount, TotalQty, Memo, ExpectedDate,
                    SiteId, OrgId, CurrencyId, ExchangeRate,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @OrderId, @OrderDate, @OrderType, @ShopId, @CustomerId, @Status,
                    @ApplyUserId, @ApplyDate, @ApproveUserId, @ApproveDate,
                    @TotalAmount, @TotalQty, @Memo, @ExpectedDate,
                    @SiteId, @OrgId, @CurrencyId, @ExchangeRate,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, salesOrder);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銷售單失敗: {salesOrder.OrderId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(SalesOrder salesOrder)
    {
        try
        {
            const string sql = @"
                UPDATE SalesOrders SET
                    OrderId = @OrderId,
                    OrderDate = @OrderDate,
                    OrderType = @OrderType,
                    ShopId = @ShopId,
                    CustomerId = @CustomerId,
                    Status = @Status,
                    ApplyUserId = @ApplyUserId,
                    ApplyDate = @ApplyDate,
                    ApproveUserId = @ApproveUserId,
                    ApproveDate = @ApproveDate,
                    TotalAmount = @TotalAmount,
                    TotalQty = @TotalQty,
                    Memo = @Memo,
                    ExpectedDate = @ExpectedDate,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    CurrencyId = @CurrencyId,
                    ExchangeRate = @ExchangeRate,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, salesOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷售單失敗: {salesOrder.TKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM SalesOrders 
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售單失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByOrderIdAsync(string orderId, long? excludeTKey = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM SalesOrders 
                WHERE OrderId = @OrderId";

            var parameters = new DynamicParameters();
            parameters.Add("OrderId", orderId);

            if (excludeTKey.HasValue)
            {
                sql += " AND TKey != @ExcludeTKey";
                parameters.Add("ExcludeTKey", excludeTKey.Value);
            }

            var count = await ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查銷售單號是否存在失敗: {orderId}", ex);
            throw;
        }
    }
}

