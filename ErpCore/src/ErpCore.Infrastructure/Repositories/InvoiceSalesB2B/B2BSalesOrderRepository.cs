using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B銷售單 Repository 實作 (SYSG000_B2B - B2B銷售資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class B2BSalesOrderRepository : BaseRepository, IB2BSalesOrderRepository
{
    public B2BSalesOrderRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<B2BSalesOrder?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BSalesOrders 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<B2BSalesOrder>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B銷售單失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<B2BSalesOrder?> GetByOrderIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BSalesOrders 
                WHERE OrderId = @OrderId";

            return await QueryFirstOrDefaultAsync<B2BSalesOrder>(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B銷售單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<B2BSalesOrder>> QueryAsync(B2BSalesOrderQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM B2BSalesOrders
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

            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                sql += " AND B2BFlag = @B2BFlag";
                parameters.Add("B2BFlag", query.B2BFlag);
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

            var items = await QueryAsync<B2BSalesOrder>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM B2BSalesOrders
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
            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                countSql += " AND B2BFlag = @B2BFlag";
                countParameters.Add("B2BFlag", query.B2BFlag);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<B2BSalesOrder>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B銷售單列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(B2BSalesOrder salesOrder)
    {
        try
        {
            const string sql = @"
                INSERT INTO B2BSalesOrders (
                    OrderId, OrderDate, OrderType, ShopId, CustomerId, Status,
                    ApplyUserId, ApplyDate, ApproveUserId, ApproveDate,
                    TotalAmount, TotalQty, Memo, ExpectedDate,
                    SiteId, OrgId, CurrencyId, ExchangeRate, B2BFlag,
                    TransferType, TransferStatus,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @OrderId, @OrderDate, @OrderType, @ShopId, @CustomerId, @Status,
                    @ApplyUserId, @ApplyDate, @ApproveUserId, @ApproveDate,
                    @TotalAmount, @TotalQty, @Memo, @ExpectedDate,
                    @SiteId, @OrgId, @CurrencyId, @ExchangeRate, @B2BFlag,
                    @TransferType, @TransferStatus,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, salesOrder);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增B2B銷售單失敗: {salesOrder.OrderId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(B2BSalesOrder salesOrder)
    {
        try
        {
            const string sql = @"
                UPDATE B2BSalesOrders SET
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
                    B2BFlag = @B2BFlag,
                    TransferType = @TransferType,
                    TransferStatus = @TransferStatus,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, salesOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改B2B銷售單失敗: {salesOrder.TKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM B2BSalesOrders 
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除B2B銷售單失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByOrderIdAsync(string orderId, long? excludeTKey = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM B2BSalesOrders 
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
            _logger.LogError($"檢查B2B銷售單號是否存在失敗: {orderId}", ex);
            throw;
        }
    }
}

