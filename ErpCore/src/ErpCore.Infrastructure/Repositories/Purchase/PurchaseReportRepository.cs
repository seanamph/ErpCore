using System.Data;
using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Purchase;

/// <summary>
/// 採購報表查詢 Repository 實作 (SYSP410-SYSP4I0)
/// 使用視圖進行查詢
/// </summary>
public class PurchaseReportRepository : BaseRepository, IPurchaseReportRepository
{
    public PurchaseReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<IEnumerable<PurchaseReportResult>> QueryPurchaseReportsAsync(PurchaseReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM v_PurchaseReportQuery 
                WHERE 1=1";

            var parameters = new DynamicParameters();
            var filters = query.Filters;

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.OrderId))
                {
                    sql += " AND OrderId LIKE @OrderId";
                    parameters.Add("OrderId", $"%{filters.OrderId}%");
                }

                if (!string.IsNullOrEmpty(filters.OrderType))
                {
                    sql += " AND OrderType = @OrderType";
                    parameters.Add("OrderType", filters.OrderType);
                }

                if (!string.IsNullOrEmpty(filters.ShopId))
                {
                    sql += " AND ShopId = @ShopId";
                    parameters.Add("ShopId", filters.ShopId);
                }

                if (!string.IsNullOrEmpty(filters.SupplierId))
                {
                    sql += " AND SupplierId = @SupplierId";
                    parameters.Add("SupplierId", filters.SupplierId);
                }

                if (!string.IsNullOrEmpty(filters.Status))
                {
                    sql += " AND Status = @Status";
                    parameters.Add("Status", filters.Status);
                }

                if (filters.OrderDateFrom.HasValue)
                {
                    sql += " AND OrderDate >= @OrderDateFrom";
                    parameters.Add("OrderDateFrom", filters.OrderDateFrom);
                }

                if (filters.OrderDateTo.HasValue)
                {
                    sql += " AND OrderDate <= @OrderDateTo";
                    parameters.Add("OrderDateTo", filters.OrderDateTo);
                }

                if (!string.IsNullOrEmpty(filters.ApplyUserId))
                {
                    sql += " AND ApplyUserId = @ApplyUserId";
                    parameters.Add("ApplyUserId", filters.ApplyUserId);
                }

                if (!string.IsNullOrEmpty(filters.ApproveUserId))
                {
                    sql += " AND ApproveUserId = @ApproveUserId";
                    parameters.Add("ApproveUserId", filters.ApproveUserId);
                }
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                sql += $" ORDER BY {query.SortField}";
                if (query.SortOrder == "DESC")
                {
                    sql += " DESC";
                }
                else
                {
                    sql += " ASC";
                }
            }
            else
            {
                sql += " ORDER BY OrderDate DESC, OrderId DESC";
            }

            // 分頁
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<PurchaseReportResult>(sql, parameters);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetPurchaseReportCountAsync(PurchaseReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM v_PurchaseReportQuery 
                WHERE 1=1";

            var parameters = new DynamicParameters();
            var filters = query.Filters;

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.OrderId))
                {
                    sql += " AND OrderId LIKE @OrderId";
                    parameters.Add("OrderId", $"%{filters.OrderId}%");
                }

                if (!string.IsNullOrEmpty(filters.OrderType))
                {
                    sql += " AND OrderType = @OrderType";
                    parameters.Add("OrderType", filters.OrderType);
                }

                if (!string.IsNullOrEmpty(filters.ShopId))
                {
                    sql += " AND ShopId = @ShopId";
                    parameters.Add("ShopId", filters.ShopId);
                }

                if (!string.IsNullOrEmpty(filters.SupplierId))
                {
                    sql += " AND SupplierId = @SupplierId";
                    parameters.Add("SupplierId", filters.SupplierId);
                }

                if (!string.IsNullOrEmpty(filters.Status))
                {
                    sql += " AND Status = @Status";
                    parameters.Add("Status", filters.Status);
                }

                if (filters.OrderDateFrom.HasValue)
                {
                    sql += " AND OrderDate >= @OrderDateFrom";
                    parameters.Add("OrderDateFrom", filters.OrderDateFrom);
                }

                if (filters.OrderDateTo.HasValue)
                {
                    sql += " AND OrderDate <= @OrderDateTo";
                    parameters.Add("OrderDateTo", filters.OrderDateTo);
                }

                if (!string.IsNullOrEmpty(filters.ApplyUserId))
                {
                    sql += " AND ApplyUserId = @ApplyUserId";
                    parameters.Add("ApplyUserId", filters.ApplyUserId);
                }

                if (!string.IsNullOrEmpty(filters.ApproveUserId))
                {
                    sql += " AND ApproveUserId = @ApproveUserId";
                    parameters.Add("ApproveUserId", filters.ApproveUserId);
                }
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表總數失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseReportDetailResult>> QueryPurchaseReportDetailsAsync(PurchaseReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM v_PurchaseReportDetailQuery 
                WHERE 1=1";

            var parameters = new DynamicParameters();
            var filters = query.Filters;

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.OrderId))
                {
                    sql += " AND OrderId LIKE @OrderId";
                    parameters.Add("OrderId", $"%{filters.OrderId}%");
                }

                if (!string.IsNullOrEmpty(filters.OrderType))
                {
                    sql += " AND OrderType = @OrderType";
                    parameters.Add("OrderType", filters.OrderType);
                }

                if (!string.IsNullOrEmpty(filters.ShopId))
                {
                    sql += " AND ShopId = @ShopId";
                    parameters.Add("ShopId", filters.ShopId);
                }

                if (!string.IsNullOrEmpty(filters.SupplierId))
                {
                    sql += " AND SupplierId = @SupplierId";
                    parameters.Add("SupplierId", filters.SupplierId);
                }

                if (!string.IsNullOrEmpty(filters.Status))
                {
                    sql += " AND Status = @Status";
                    parameters.Add("Status", filters.Status);
                }

                if (filters.OrderDateFrom.HasValue)
                {
                    sql += " AND OrderDate >= @OrderDateFrom";
                    parameters.Add("OrderDateFrom", filters.OrderDateFrom);
                }

                if (filters.OrderDateTo.HasValue)
                {
                    sql += " AND OrderDate <= @OrderDateTo";
                    parameters.Add("OrderDateTo", filters.OrderDateTo);
                }

                if (!string.IsNullOrEmpty(filters.GoodsId))
                {
                    sql += " AND GoodsId LIKE @GoodsId";
                    parameters.Add("GoodsId", $"%{filters.GoodsId}%");
                }
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                sql += $" ORDER BY {query.SortField}";
                if (query.SortOrder == "DESC")
                {
                    sql += " DESC";
                }
                else
                {
                    sql += " ASC";
                }
            }
            else
            {
                sql += " ORDER BY OrderDate DESC, OrderId DESC, LineNum ASC";
            }

            // 分頁
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<PurchaseReportDetailResult>(sql, parameters);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表明細列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetPurchaseReportDetailCountAsync(PurchaseReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM v_PurchaseReportDetailQuery 
                WHERE 1=1";

            var parameters = new DynamicParameters();
            var filters = query.Filters;

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.OrderId))
                {
                    sql += " AND OrderId LIKE @OrderId";
                    parameters.Add("OrderId", $"%{filters.OrderId}%");
                }

                if (!string.IsNullOrEmpty(filters.OrderType))
                {
                    sql += " AND OrderType = @OrderType";
                    parameters.Add("OrderType", filters.OrderType);
                }

                if (!string.IsNullOrEmpty(filters.ShopId))
                {
                    sql += " AND ShopId = @ShopId";
                    parameters.Add("ShopId", filters.ShopId);
                }

                if (!string.IsNullOrEmpty(filters.SupplierId))
                {
                    sql += " AND SupplierId = @SupplierId";
                    parameters.Add("SupplierId", filters.SupplierId);
                }

                if (!string.IsNullOrEmpty(filters.Status))
                {
                    sql += " AND Status = @Status";
                    parameters.Add("Status", filters.Status);
                }

                if (filters.OrderDateFrom.HasValue)
                {
                    sql += " AND OrderDate >= @OrderDateFrom";
                    parameters.Add("OrderDateFrom", filters.OrderDateFrom);
                }

                if (filters.OrderDateTo.HasValue)
                {
                    sql += " AND OrderDate <= @OrderDateTo";
                    parameters.Add("OrderDateTo", filters.OrderDateTo);
                }

                if (!string.IsNullOrEmpty(filters.GoodsId))
                {
                    sql += " AND GoodsId LIKE @GoodsId";
                    parameters.Add("GoodsId", $"%{filters.GoodsId}%");
                }
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表明細總數失敗", ex);
            throw;
        }
    }
}
