using System.Data;
using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 銷售報表查詢 Repository 實作 (SYSG610-SYSG640 - 報表查詢作業)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SalesReportQueryRepository : BaseRepository, ISalesReportQueryRepository
{
    public SalesReportQueryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<SalesReportDetailResult>> QueryDetailReportAsync(SalesReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    so.OrderId,
                    so.OrderDate,
                    so.OrderType,
                    so.ShopId,
                    s.ShopName,
                    so.CustomerId,
                    c.CustomerName,
                    sod.GoodsId,
                    g.GoodsName,
                    sod.OrderQty,
                    sod.UnitPrice,
                    sod.Amount,
                    sod.ShippedQty,
                    sod.ReturnQty,
                    so.Status,
                    so.CurrencyId,
                    so.ExchangeRate,
                    so.ApplyUserId,
                    u1.UserName AS ApplyUserName,
                    so.ApplyDate,
                    so.ApproveUserId,
                    u2.UserName AS ApproveUserName,
                    so.ApproveDate
                FROM SalesOrders so
                INNER JOIN SalesOrderDetails sod ON so.OrderId = sod.OrderId
                LEFT JOIN Shops s ON so.ShopId = s.ShopId
                LEFT JOIN Customers c ON so.CustomerId = c.CustomerId
                LEFT JOIN Goods g ON sod.GoodsId = g.GoodsId
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

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND sod.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
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

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY so.OrderDate DESC, so.OrderId DESC, sod.LineNum";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            // 查詢資料
            var items = await QueryAsync<SalesReportDetailResult>(sql, parameters);

            // 查詢總筆數
            var countSql = @"
                SELECT COUNT(*)
                FROM SalesOrders so
                INNER JOIN SalesOrderDetails sod ON so.OrderId = sod.OrderId
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
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                countSql += " AND sod.GoodsId LIKE @GoodsId";
                countParameters.Add("GoodsId", $"%{query.GoodsId}%");
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

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<SalesReportDetailResult>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售報表明細失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SalesReportSummaryResult>> QuerySummaryReportAsync(SalesReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    so.ShopId,
                    s.ShopName,
                    so.CustomerId,
                    c.CustomerName,
                    sod.GoodsId,
                    g.GoodsName,
                    COUNT(DISTINCT so.OrderId) AS OrderCount,
                    SUM(sod.OrderQty) AS TotalQty,
                    SUM(sod.Amount) AS TotalAmount,
                    CASE 
                        WHEN SUM(sod.OrderQty) > 0 THEN SUM(sod.Amount) / SUM(sod.OrderQty)
                        ELSE 0
                    END AS AvgUnitPrice,
                    SUM(sod.ShippedQty) AS ShippedQty,
                    SUM(sod.ReturnQty) AS ReturnQty
                FROM SalesOrders so
                INNER JOIN SalesOrderDetails sod ON so.OrderId = sod.OrderId
                LEFT JOIN Shops s ON so.ShopId = s.ShopId
                LEFT JOIN Customers c ON so.CustomerId = c.CustomerId
                LEFT JOIN Goods g ON sod.GoodsId = g.GoodsId
                WHERE 1=1";

            var parameters = new DynamicParameters();

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

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND sod.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
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

            sql += " GROUP BY so.ShopId, s.ShopName, so.CustomerId, c.CustomerName, sod.GoodsId, g.GoodsName";

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY so.ShopId, sod.GoodsId";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            // 查詢資料
            var items = await QueryAsync<SalesReportSummaryResult>(sql, parameters);

            // 查詢總筆數（需要子查詢）
            var countSql = @"
                SELECT COUNT(*)
                FROM (
                    SELECT 
                        so.ShopId,
                        sod.GoodsId
                    FROM SalesOrders so
                    INNER JOIN SalesOrderDetails sod ON so.OrderId = sod.OrderId
                    WHERE 1=1";

            var countParameters = new DynamicParameters();
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
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                countSql += " AND sod.GoodsId LIKE @GoodsId";
                countParameters.Add("GoodsId", $"%{query.GoodsId}%");
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
            countSql += " GROUP BY so.ShopId, sod.GoodsId) AS t";

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<SalesReportSummaryResult>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售報表彙總失敗", ex);
            throw;
        }
    }
}

