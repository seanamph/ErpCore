using System.Data;
using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Purchase;

/// <summary>
/// 採購單查詢 Repository 實作 (SYSP310-SYSP330)
/// 使用視圖進行查詢
/// </summary>
public class PurchaseOrderQueryRepository : BaseRepository, IPurchaseOrderQueryRepository
{
    public PurchaseOrderQueryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<IEnumerable<PurchaseOrderQueryResult>> QueryPurchaseOrdersAsync(PurchaseOrderQueryRequest request)
    {
        try
        {
            var sql = @"
                SELECT * FROM v_PurchaseOrderQuery 
                WHERE 1=1";

            var parameters = new DynamicParameters();
            var filters = request.Filters;

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

                if (filters.ExpectedDateFrom.HasValue)
                {
                    sql += " AND ExpectedDate >= @ExpectedDateFrom";
                    parameters.Add("ExpectedDateFrom", filters.ExpectedDateFrom);
                }

                if (filters.ExpectedDateTo.HasValue)
                {
                    sql += " AND ExpectedDate <= @ExpectedDateTo";
                    parameters.Add("ExpectedDateTo", filters.ExpectedDateTo);
                }

                if (filters.MinTotalAmount.HasValue)
                {
                    sql += " AND TotalAmount >= @MinTotalAmount";
                    parameters.Add("MinTotalAmount", filters.MinTotalAmount);
                }

                if (filters.MaxTotalAmount.HasValue)
                {
                    sql += " AND TotalAmount <= @MaxTotalAmount";
                    parameters.Add("MaxTotalAmount", filters.MaxTotalAmount);
                }
            }

            // 排序
            var sortField = !string.IsNullOrEmpty(request.SortField) ? request.SortField : "OrderDate";
            var sortOrder = !string.IsNullOrEmpty(request.SortOrder) ? request.SortOrder : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (request.PageIndex - 1) * request.PageSize);
            parameters.Add("PageSize", request.PageSize);

            return await QueryAsync<PurchaseOrderQueryResult>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購單列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetPurchaseOrderCountAsync(PurchaseOrderQueryRequest request)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM v_PurchaseOrderQuery 
                WHERE 1=1";

            var parameters = new DynamicParameters();
            var filters = request.Filters;

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

                if (filters.ExpectedDateFrom.HasValue)
                {
                    sql += " AND ExpectedDate >= @ExpectedDateFrom";
                    parameters.Add("ExpectedDateFrom", filters.ExpectedDateFrom);
                }

                if (filters.ExpectedDateTo.HasValue)
                {
                    sql += " AND ExpectedDate <= @ExpectedDateTo";
                    parameters.Add("ExpectedDateTo", filters.ExpectedDateTo);
                }

                if (filters.MinTotalAmount.HasValue)
                {
                    sql += " AND TotalAmount >= @MinTotalAmount";
                    parameters.Add("MinTotalAmount", filters.MinTotalAmount);
                }

                if (filters.MaxTotalAmount.HasValue)
                {
                    sql += " AND TotalAmount <= @MaxTotalAmount";
                    parameters.Add("MaxTotalAmount", filters.MaxTotalAmount);
                }
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購單總數失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseOrderDetailQueryItem>> GetPurchaseOrderDetailsAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM v_PurchaseOrderDetailQuery 
                WHERE OrderId = @OrderId 
                ORDER BY LineNum";

            return await QueryAsync<PurchaseOrderDetailQueryItem>(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購單明細失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<PurchaseOrderStatistics> GetPurchaseOrderStatisticsAsync(PurchaseOrderStatisticsRequest request)
    {
        try
        {
            var sql = @"
                SELECT 
                    COUNT(*) AS TotalOrders,
                    SUM(TotalAmount) AS TotalAmount,
                    SUM(TotalQty) AS TotalQty,
                    AVG(TotalAmount) AS AvgAmount
                FROM v_PurchaseOrderQuery 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (request.DateFrom.HasValue)
            {
                sql += " AND OrderDate >= @DateFrom";
                parameters.Add("DateFrom", request.DateFrom);
            }

            if (request.DateTo.HasValue)
            {
                sql += " AND OrderDate <= @DateTo";
                parameters.Add("DateTo", request.DateTo);
            }

            if (!string.IsNullOrEmpty(request.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", request.ShopId);
            }

            if (!string.IsNullOrEmpty(request.SupplierId))
            {
                sql += " AND SupplierId = @SupplierId";
                parameters.Add("SupplierId", request.SupplierId);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", request.Status);
            }

            // 查詢摘要
            var summary = await QueryFirstOrDefaultAsync<PurchaseOrderStatisticsSummary>(sql, parameters);

            // 查詢明細（按分組）
            var detailsSql = "";
            var groupByField = "";
            var groupByNameField = "";

            switch (request.GroupBy.ToLower())
            {
                case "supplier":
                    groupByField = "SupplierId";
                    groupByNameField = "SupplierName";
                    break;
                case "shop":
                    groupByField = "ShopId";
                    groupByNameField = "ShopName";
                    break;
                case "goods":
                    // 按商品分组需要从明细表查询
                    groupByField = "GoodsId";
                    groupByNameField = "GoodsName";
                    break;
                case "status":
                    groupByField = "Status";
                    groupByNameField = "StatusName";
                    break;
                case "date":
                    groupByField = "CAST(OrderDate AS DATE)";
                    groupByNameField = "CAST(OrderDate AS DATE)";
                    break;
                default:
                    groupByField = "SupplierId";
                    groupByNameField = "SupplierName";
                    break;
            }

            // 如果是按商品分组，需要从明细视图查询
            if (request.GroupBy.ToLower() == "goods")
            {
                detailsSql = $@"
                    SELECT 
                        {groupByField} AS GroupKey,
                        {groupByNameField} AS GroupName,
                        COUNT(DISTINCT OrderId) AS OrderCount,
                        SUM(Amount) AS TotalAmount,
                        SUM(OrderQty) AS TotalQty
                    FROM v_PurchaseOrderDetailQuery 
                    WHERE 1=1";
            }
            else
            {
                detailsSql = $@"
                    SELECT 
                        {groupByField} AS GroupKey,
                        {groupByNameField} AS GroupName,
                        COUNT(*) AS OrderCount,
                        SUM(TotalAmount) AS TotalAmount,
                        SUM(TotalQty) AS TotalQty
                    FROM v_PurchaseOrderQuery 
                    WHERE 1=1";
            }

            if (request.DateFrom.HasValue)
            {
                detailsSql += request.GroupBy.ToLower() == "goods" 
                    ? " AND OrderDate >= @DateFrom" 
                    : " AND OrderDate >= @DateFrom";
            }

            if (request.DateTo.HasValue)
            {
                detailsSql += request.GroupBy.ToLower() == "goods" 
                    ? " AND OrderDate <= @DateTo" 
                    : " AND OrderDate <= @DateTo";
            }

            if (!string.IsNullOrEmpty(request.ShopId))
            {
                detailsSql += " AND ShopId = @ShopId";
            }

            if (!string.IsNullOrEmpty(request.SupplierId))
            {
                detailsSql += " AND SupplierId = @SupplierId";
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                detailsSql += " AND Status = @Status";
            }

            detailsSql += $" GROUP BY {groupByField}, {groupByNameField}";
            detailsSql += " ORDER BY TotalAmount DESC";

            var details = await QueryAsync<PurchaseOrderStatisticsDetail>(detailsSql, parameters);

            return new PurchaseOrderStatistics
            {
                Summary = summary ?? new PurchaseOrderStatisticsSummary(),
                Details = details.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購單統計失敗", ex);
            throw;
        }
    }
}
