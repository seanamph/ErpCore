using System.Data;
using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B銷售查詢 Repository 實作 (SYSG000_B2B - B2B銷售查詢作業)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class B2BSalesOrderQueryRepository : BaseRepository, IB2BSalesOrderQueryRepository
{
    public B2BSalesOrderQueryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<B2BSalesOrderQueryResult>> QueryAsync(B2BSalesOrderQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    so.TKey,
                    so.OrderId,
                    so.OrderDate,
                    so.OrderType,
                    so.ShopId,
                    s.ShopName,
                    so.CustomerId,
                    c.CustomerName,
                    so.Status,
                    so.TotalAmount,
                    so.TotalQty,
                    so.ApplyUserId,
                    u1.UserName AS ApplyUserName,
                    so.ApplyDate,
                    so.ApproveUserId,
                    u2.UserName AS ApproveUserName,
                    so.ApproveDate,
                    so.TransferType,
                    so.TransferStatus,
                    so.CreatedAt,
                    so.UpdatedAt
                FROM B2BSalesOrders so
                LEFT JOIN Shops s ON so.ShopId = s.ShopId
                LEFT JOIN Customers c ON so.CustomerId = c.CustomerId
                LEFT JOIN Users u1 ON so.ApplyUserId = u1.UserId
                LEFT JOIN Users u2 ON so.ApproveUserId = u2.UserId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.OrderId))
            {
                sql += " AND so.OrderId LIKE @OrderId";
                parameters.Add("OrderId", $"%{query.OrderId}%");
            }

            if (!string.IsNullOrEmpty(query.OrderType))
            {
                sql += " AND so.OrderType = @OrderType";
                parameters.Add("OrderType", query.OrderType);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND so.ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND so.CustomerId LIKE @CustomerId";
                parameters.Add("CustomerId", $"%{query.CustomerId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND so.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND so.OrderDate >= @OrderDateFrom";
                parameters.Add("OrderDateFrom", query.OrderDateFrom.Value);
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND so.OrderDate <= @OrderDateTo";
                parameters.Add("OrderDateTo", query.OrderDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                sql += " AND so.B2BFlag = @B2BFlag";
                parameters.Add("B2BFlag", query.B2BFlag);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY so.{query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY so.OrderDate DESC, so.OrderId DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            // 查詢資料
            var items = await QueryAsync<B2BSalesOrderQueryResult>(sql, parameters);

            // 查詢總筆數
            var countSql = @"
                SELECT COUNT(*)
                FROM B2BSalesOrders so
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.OrderId))
            {
                countSql += " AND so.OrderId LIKE @OrderId";
                countParameters.Add("OrderId", $"%{query.OrderId}%");
            }
            if (!string.IsNullOrEmpty(query.OrderType))
            {
                countSql += " AND so.OrderType = @OrderType";
                countParameters.Add("OrderType", query.OrderType);
            }
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                countSql += " AND so.ShopId = @ShopId";
                countParameters.Add("ShopId", query.ShopId);
            }
            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                countSql += " AND so.CustomerId LIKE @CustomerId";
                countParameters.Add("CustomerId", $"%{query.CustomerId}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND so.Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.OrderDateFrom.HasValue)
            {
                countSql += " AND so.OrderDate >= @OrderDateFrom";
                countParameters.Add("OrderDateFrom", query.OrderDateFrom.Value);
            }
            if (query.OrderDateTo.HasValue)
            {
                countSql += " AND so.OrderDate <= @OrderDateTo";
                countParameters.Add("OrderDateTo", query.OrderDateTo.Value);
            }
            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                countSql += " AND so.B2BFlag = @B2BFlag";
                countParameters.Add("B2BFlag", query.B2BFlag);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<B2BSalesOrderQueryResult>
            {
                Items = items.ToList(),
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

    public async Task<B2BSalesOrderStatisticsResult> GetStatisticsAsync(B2BSalesOrderStatisticsQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    COUNT(*) AS OrderCount,
                    ISNULL(SUM(so.TotalAmount), 0) AS TotalAmount,
                    ISNULL(SUM(so.TotalQty), 0) AS TotalQty,
                    CASE 
                        WHEN COUNT(*) > 0 THEN ISNULL(SUM(so.TotalAmount), 0) / COUNT(*)
                        ELSE 0
                    END AS AvgAmount
                FROM B2BSalesOrders so
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND so.ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.OrderType))
            {
                sql += " AND so.OrderType = @OrderType";
                parameters.Add("OrderType", query.OrderType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND so.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND so.OrderDate >= @OrderDateFrom";
                parameters.Add("OrderDateFrom", query.OrderDateFrom.Value);
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND so.OrderDate <= @OrderDateTo";
                parameters.Add("OrderDateTo", query.OrderDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                sql += " AND so.B2BFlag = @B2BFlag";
                parameters.Add("B2BFlag", query.B2BFlag);
            }

            var summary = await QueryFirstOrDefaultAsync<B2BSalesOrderStatisticsResult>(sql, parameters);
            if (summary == null)
            {
                summary = new B2BSalesOrderStatisticsResult();
            }

            // 按分店統計
            var byShopSql = @"
                SELECT 
                    so.ShopId,
                    s.ShopName,
                    COUNT(*) AS OrderCount,
                    ISNULL(SUM(so.TotalAmount), 0) AS TotalAmount
                FROM B2BSalesOrders so
                LEFT JOIN Shops s ON so.ShopId = s.ShopId
                WHERE 1=1";

            var byShopParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.OrderType))
            {
                byShopSql += " AND so.OrderType = @OrderType";
                byShopParameters.Add("OrderType", query.OrderType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                byShopSql += " AND so.Status = @Status";
                byShopParameters.Add("Status", query.Status);
            }
            if (query.OrderDateFrom.HasValue)
            {
                byShopSql += " AND so.OrderDate >= @OrderDateFrom";
                byShopParameters.Add("OrderDateFrom", query.OrderDateFrom.Value);
            }
            if (query.OrderDateTo.HasValue)
            {
                byShopSql += " AND so.OrderDate <= @OrderDateTo";
                byShopParameters.Add("OrderDateTo", query.OrderDateTo.Value);
            }
            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                byShopSql += " AND so.B2BFlag = @B2BFlag";
                byShopParameters.Add("B2BFlag", query.B2BFlag);
            }
            byShopSql += " GROUP BY so.ShopId, s.ShopName";

            var byShop = await QueryAsync<B2BSalesOrderStatisticsByShop>(byShopSql, byShopParameters);
            summary.ByShop = byShop.ToList();

            // 按狀態統計
            var byStatusSql = @"
                SELECT 
                    so.Status,
                    CASE so.Status
                        WHEN 'D' THEN '草稿'
                        WHEN 'S' THEN '已送出'
                        WHEN 'A' THEN '已審核'
                        WHEN 'X' THEN '已取消'
                        WHEN 'C' THEN '已結案'
                        ELSE so.Status
                    END AS StatusName,
                    COUNT(*) AS OrderCount,
                    ISNULL(SUM(so.TotalAmount), 0) AS TotalAmount
                FROM B2BSalesOrders so
                WHERE 1=1";

            var byStatusParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                byStatusSql += " AND so.ShopId = @ShopId";
                byStatusParameters.Add("ShopId", query.ShopId);
            }
            if (!string.IsNullOrEmpty(query.OrderType))
            {
                byStatusSql += " AND so.OrderType = @OrderType";
                byStatusParameters.Add("OrderType", query.OrderType);
            }
            if (query.OrderDateFrom.HasValue)
            {
                byStatusSql += " AND so.OrderDate >= @OrderDateFrom";
                byStatusParameters.Add("OrderDateFrom", query.OrderDateFrom.Value);
            }
            if (query.OrderDateTo.HasValue)
            {
                byStatusSql += " AND so.OrderDate <= @OrderDateTo";
                byStatusParameters.Add("OrderDateTo", query.OrderDateTo.Value);
            }
            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                byStatusSql += " AND so.B2BFlag = @B2BFlag";
                byStatusParameters.Add("B2BFlag", query.B2BFlag);
            }
            byStatusSql += " GROUP BY so.Status";

            var byStatus = await QueryAsync<B2BSalesOrderStatisticsByStatus>(byStatusSql, byStatusParameters);
            summary.ByStatus = byStatus.ToList();

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B銷售單統計失敗", ex);
            throw;
        }
    }
}

