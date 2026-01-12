using Dapper;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 耗材管理報表 Repository 實作 (SYSA255)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ConsumableReportRepository : BaseRepository, IConsumableReportRepository
{
    public ConsumableReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ConsumableReportItem>> GetReportDataAsync(ConsumableReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    c.ConsumableId, c.ConsumableName, c.CategoryId, c.Unit, c.Specification,
                    c.Brand, c.Model, c.BarCode, c.Status, c.AssetStatus, c.SiteId,
                    c.Location, c.Quantity AS CurrentQty, c.MinQuantity, c.MaxQuantity, c.Price,
                    cc.CategoryName,
                    s.ShopName AS SiteName,
                    w.WarehouseName,
                    CASE WHEN c.Status = '1' THEN '正常' ELSE '停用' END AS StatusName,
                    CASE WHEN c.AssetStatus = 'A' THEN '使用中' 
                         WHEN c.AssetStatus = 'B' THEN '閒置' 
                         WHEN c.AssetStatus = 'C' THEN '報廢' 
                         ELSE c.AssetStatus END AS AssetStatusName,
                    ISNULL(c.Price * c.Quantity, 0) AS CurrentAmt,
                    ISNULL(SUM(CASE WHEN t.TransactionType = '1' THEN t.Quantity ELSE 0 END), 0) AS InQty,
                    ISNULL(SUM(CASE WHEN t.TransactionType IN ('2', '6') THEN t.Quantity ELSE 0 END), 0) AS OutQty,
                    ISNULL(SUM(CASE WHEN t.TransactionType = '1' THEN t.Amount ELSE 0 END), 0) AS InAmt,
                    ISNULL(SUM(CASE WHEN t.TransactionType IN ('2', '6') THEN t.Amount ELSE 0 END), 0) AS OutAmt,
                    CASE WHEN c.Quantity < c.MinQuantity THEN 1 ELSE 0 END AS IsLowStock,
                    CASE WHEN c.Quantity > c.MaxQuantity THEN 1 ELSE 0 END AS IsOverStock
                FROM Consumables c
                LEFT JOIN ConsumableCategories cc ON c.CategoryId = cc.CategoryId
                LEFT JOIN Shops s ON c.SiteId = s.ShopId
                LEFT JOIN Warehouses w ON c.SiteId = w.SiteId AND c.WarehouseId = w.WarehouseId
                LEFT JOIN ConsumableTransactions t ON c.ConsumableId = t.ConsumableId
                    AND (@DateFrom IS NULL OR t.TransactionDate >= @DateFrom)
                    AND (@DateTo IS NULL OR t.TransactionDate <= @DateTo)
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ConsumableId))
            {
                sql += " AND c.ConsumableId LIKE @ConsumableId";
                parameters.Add("ConsumableId", $"%{query.ConsumableId}%");
            }

            if (!string.IsNullOrEmpty(query.ConsumableName))
            {
                sql += " AND c.ConsumableName LIKE @ConsumableName";
                parameters.Add("ConsumableName", $"%{query.ConsumableName}%");
            }

            if (!string.IsNullOrEmpty(query.CategoryId))
            {
                sql += " AND c.CategoryId = @CategoryId";
                parameters.Add("CategoryId", query.CategoryId);
            }

            if (query.SiteIds != null && query.SiteIds.Any())
            {
                sql += " AND c.SiteId IN @SiteIds";
                parameters.Add("SiteIds", query.SiteIds);
            }

            if (query.WarehouseIds != null && query.WarehouseIds.Any())
            {
                sql += " AND c.WarehouseId IN @WarehouseIds";
                parameters.Add("WarehouseIds", query.WarehouseIds);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND c.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.AssetStatus))
            {
                sql += " AND c.AssetStatus = @AssetStatus";
                parameters.Add("AssetStatus", query.AssetStatus);
            }

            parameters.Add("DateFrom", query.DateFrom);
            parameters.Add("DateTo", query.DateTo);

            sql += " GROUP BY c.ConsumableId, c.ConsumableName, c.CategoryId, c.Unit, c.Specification, " +
                   "c.Brand, c.Model, c.BarCode, c.Status, c.AssetStatus, c.SiteId, c.Location, " +
                   "c.Quantity, c.MinQuantity, c.MaxQuantity, c.Price, cc.CategoryName, s.ShopName, w.WarehouseName";

            // 排序
            var sortField = query.SortField ?? "ConsumableId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 查詢總數
            var countSql = $"SELECT COUNT(DISTINCT c.ConsumableId) FROM ({sql.Replace("GROUP BY", "GROUP BY").Split("ORDER BY")[0]}) AS subquery";
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var items = await connection.QueryAsync<ConsumableReportItem>(sql, parameters);

            return new PagedResult<ConsumableReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材管理報表資料失敗", ex);
            throw;
        }
    }

    public async Task<ConsumableReportSummary> GetReportSummaryAsync(ConsumableReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    COUNT(DISTINCT c.ConsumableId) AS TotalConsumables,
                    ISNULL(SUM(c.Quantity), 0) AS TotalCurrentQty,
                    ISNULL(SUM(c.Price * c.Quantity), 0) AS TotalCurrentAmt,
                    ISNULL(SUM(CASE WHEN t.TransactionType = '1' THEN t.Quantity ELSE 0 END), 0) AS TotalInQty,
                    ISNULL(SUM(CASE WHEN t.TransactionType IN ('2', '6') THEN t.Quantity ELSE 0 END), 0) AS TotalOutQty,
                    ISNULL(SUM(CASE WHEN t.TransactionType = '1' THEN t.Amount ELSE 0 END), 0) AS TotalInAmt,
                    ISNULL(SUM(CASE WHEN t.TransactionType IN ('2', '6') THEN t.Amount ELSE 0 END), 0) AS TotalOutAmt
                FROM Consumables c
                LEFT JOIN ConsumableTransactions t ON c.ConsumableId = t.ConsumableId
                    AND (@DateFrom IS NULL OR t.TransactionDate >= @DateFrom)
                    AND (@DateTo IS NULL OR t.TransactionDate <= @DateTo)
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ConsumableId))
            {
                sql += " AND c.ConsumableId LIKE @ConsumableId";
                parameters.Add("ConsumableId", $"%{query.ConsumableId}%");
            }

            if (!string.IsNullOrEmpty(query.ConsumableName))
            {
                sql += " AND c.ConsumableName LIKE @ConsumableName";
                parameters.Add("ConsumableName", $"%{query.ConsumableName}%");
            }

            if (!string.IsNullOrEmpty(query.CategoryId))
            {
                sql += " AND c.CategoryId = @CategoryId";
                parameters.Add("CategoryId", query.CategoryId);
            }

            if (query.SiteIds != null && query.SiteIds.Any())
            {
                sql += " AND c.SiteId IN @SiteIds";
                parameters.Add("SiteIds", query.SiteIds);
            }

            if (query.WarehouseIds != null && query.WarehouseIds.Any())
            {
                sql += " AND c.WarehouseId IN @WarehouseIds";
                parameters.Add("WarehouseIds", query.WarehouseIds);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND c.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.AssetStatus))
            {
                sql += " AND c.AssetStatus = @AssetStatus";
                parameters.Add("AssetStatus", query.AssetStatus);
            }

            parameters.Add("DateFrom", query.DateFrom);
            parameters.Add("DateTo", query.DateTo);

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<ConsumableReportSummary>(sql, parameters);

            return result ?? new ConsumableReportSummary();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材管理報表統計資訊失敗", ex);
            throw;
        }
    }

    public async Task<Consumable?> GetConsumableByIdAsync(string consumableId)
    {
        try
        {
            var sql = @"
                SELECT 
                    ConsumableId, ConsumableName, CategoryId, Unit, Specification,
                    Brand, Model, BarCode, Status, AssetStatus, SiteId,
                    Location, Quantity, MinQuantity, MaxQuantity, Price,
                    SupplierId, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM Consumables
                WHERE ConsumableId = @ConsumableId";

            var parameters = new DynamicParameters();
            parameters.Add("ConsumableId", consumableId);

            using var connection = _connectionFactory.CreateConnection();
            var consumable = await connection.QueryFirstOrDefaultAsync<Consumable>(sql, parameters);

            return consumable;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢耗材資訊失敗: {consumableId}", ex);
            throw;
        }
    }
}
