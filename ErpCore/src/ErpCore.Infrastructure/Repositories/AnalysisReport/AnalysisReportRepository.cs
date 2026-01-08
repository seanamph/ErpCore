using Dapper;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 進銷存分析報表 Repository 實作 (SYSA1000)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class AnalysisReportRepository : BaseRepository, IAnalysisReportRepository
{
    public AnalysisReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<AnalysisReportItemDto>> GetAnalysisReportAsync(string reportId, AnalysisReportQueryDto query)
    {
        try
        {
            // 根據報表ID動態生成SQL查詢
            var sql = BuildReportSql(reportId, query);
            var parameters = BuildReportParameters(query);

            using var connection = _connectionFactory.CreateConnection();
            
            // 查詢總筆數
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // 查詢分頁資料
            var pagedSql = $@"
                SELECT * FROM (
                    {sql}
                ) AS ReportData
                ORDER BY {GetSortField(query.SortField)} {GetSortOrder(query.SortOrder)}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await connection.QueryAsync<AnalysisReportItemDto>(pagedSql, parameters);

            return new PagedResult<AnalysisReportItemDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢進銷存分析報表失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<List<Consumable>> GetConsumablesForPrintAsync(ConsumablePrintQueryDto query)
    {
        try
        {
            var sql = @"
                SELECT 
                    ConsumableId,
                    ConsumableName,
                    CategoryId,
                    Unit,
                    Specification,
                    Brand,
                    Model,
                    BarCode,
                    Status,
                    AssetStatus,
                    SiteId,
                    Location,
                    Quantity,
                    MinQuantity,
                    MaxQuantity,
                    Price,
                    SupplierId,
                    Notes,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM Consumables
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.AssetStatus))
            {
                sql += " AND AssetStatus = @AssetStatus";
                parameters.Add("AssetStatus", query.AssetStatus);
            }

            if (query.ConsumableIds != null && query.ConsumableIds.Any())
            {
                sql += " AND ConsumableId IN @ConsumableIds";
                parameters.Add("ConsumableIds", query.ConsumableIds);
            }

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<Consumable>(sql, parameters);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 根據報表ID建立SQL查詢
    /// </summary>
    private string BuildReportSql(string reportId, AnalysisReportQueryDto query)
    {
        // 根據不同的報表類型建立對應的SQL
        return reportId switch
        {
            "SYSA1011" => BuildConsumableStockQuery(query), // 耗材庫存查詢表
            "SYSA1012" => BuildConsumableInboundQuery(query), // 耗材入庫明細表
            "SYSA1013" => BuildConsumableOutboundQuery(query), // 耗材出庫明細表
            "SYSA1014" => BuildConsumableIssueReturnQuery(query), // 耗材領用發料退回明細表
            "SYSA1015" => BuildFixedAssetSummaryQuery(query), // 固定資產數量彙總表
            "SYSA1016" => BuildMaterialCostAllocationQuery(query), // 庫房領料進價成本分攤表
            "SYSA1017" => BuildMaintenanceDeductionQuery(query), // 工務修繕扣款報表
            "SYSA1018" => BuildMaintenanceCountQuery(query), // 工務維修件數統計表
            "SYSA1019" => BuildMaintenanceCategoryQuery(query), // 工務維修類別統計表
            "SYSA1020" => BuildInventoryDifferenceQuery(query), // 盤點差異明細表
            "SYSA1021" => BuildMonthlyCostQuery(query), // 耗材進銷存月報表
            "SYSA1022" => BuildPublicExpenseQuery(query), // 公務費用歸屬統計表
            _ => BuildConsumableStockQuery(query) // 預設使用耗材庫存查詢
        };
    }

    /// <summary>
    /// 耗材庫存查詢表 (SYSA1011)
    /// </summary>
    private string BuildConsumableStockQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                st.SiteId,
                pc_b.BClassId AS BId,
                pc_m.MClassId AS MId,
                pc_s.ClassId AS SId,
                g.GoodsId,
                g.GoodsName,
                g.Unit AS PackUnit,
                g.Unit,
                ISNULL(SUM(CASE WHEN st.StocksStatus = '1' THEN st.Qty ELSE 0 END), 0) AS Qty,
                0 AS SafeQty,
                @FilterType AS SelectType
            FROM Products g
            LEFT JOIN ProductCategories pc_s ON g.ScId = pc_s.ClassId AND pc_s.ClassMode = '3'
            LEFT JOIN ProductCategories pc_m ON pc_s.MClassId = pc_m.ClassId AND pc_m.ClassMode = '2'
            LEFT JOIN ProductCategories pc_b ON pc_m.BClassId = pc_b.ClassId AND pc_b.ClassMode = '1'
            LEFT JOIN InventoryStocks st ON g.GoodsId = st.GoodsId
            WHERE 1=1";

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND st.SiteId = @SiteId";
        }

        if (!string.IsNullOrEmpty(query.BId))
        {
            sql += " AND pc_b.BClassId = @BId";
        }

        if (!string.IsNullOrEmpty(query.MId))
        {
            sql += " AND pc_m.MClassId = @MId";
        }

        if (!string.IsNullOrEmpty(query.SId))
        {
            sql += " AND pc_s.ClassId = @SId";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            sql += " AND g.GoodsId = @GoodsId";
        }

        sql += " GROUP BY st.SiteId, pc_b.BClassId, pc_m.MClassId, pc_s.ClassId, g.GoodsId, g.GoodsName, g.Unit";

        return sql;
    }

    /// <summary>
    /// 耗材入庫明細表 (SYSA1012)
    /// </summary>
    private string BuildConsumableInboundQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                st.SiteId,
                g.BId,
                g.MId,
                g.SId,
                st.GoodsId,
                g.GoodsName,
                g.Unit AS PackUnit,
                g.Unit,
                st.Qty,
                st.McAmt AS Price,
                st.StocksNtaxAmt AS Amount,
                st.StocksDate,
                st.SourceId
            FROM InventoryStocks st
            INNER JOIN Products g ON st.GoodsId = g.GoodsId
            WHERE st.StocksStatus = '1'"; // 1:入庫

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND st.SiteId = @SiteId";
        }

        if (query.BeginDate.HasValue)
        {
            sql += " AND st.StocksDate >= @BeginDate";
        }

        if (query.EndDate.HasValue)
        {
            sql += " AND st.StocksDate <= @EndDate";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            sql += " AND st.GoodsId = @GoodsId";
        }

        return sql;
    }

    /// <summary>
    /// 耗材出庫明細表 (SYSA1013)
    /// </summary>
    private string BuildConsumableOutboundQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                st.SiteId,
                g.BId,
                g.MId,
                g.SId,
                st.GoodsId,
                g.GoodsName,
                g.Unit AS PackUnit,
                g.Unit,
                st.Qty,
                st.McAmt AS Price,
                st.StocksNtaxAmt AS Amount,
                st.StocksDate,
                st.SourceId
            FROM InventoryStocks st
            INNER JOIN Products g ON st.GoodsId = g.GoodsId
            WHERE st.StocksStatus = '2'"; // 2:出庫

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND st.SiteId = @SiteId";
        }

        if (query.BeginDate.HasValue)
        {
            sql += " AND st.StocksDate >= @BeginDate";
        }

        if (query.EndDate.HasValue)
        {
            sql += " AND st.StocksDate <= @EndDate";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            sql += " AND st.GoodsId = @GoodsId";
        }

        return sql;
    }

    /// <summary>
    /// 耗材領用發料退回明細表 (SYSA1014)
    /// </summary>
    private string BuildConsumableIssueReturnQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                st.SiteId,
                pc_b.BClassId AS BId,
                pc_m.MClassId AS MId,
                pc_s.ClassId AS SId,
                g.GoodsId,
                g.GoodsName,
                g.Unit AS PackUnit,
                g.Unit,
                st.Qty,
                st.StocksDate,
                st.SourceId,
                st.StocksStatus,
                CASE WHEN st.StocksStatus = '1' THEN '入庫' 
                     WHEN st.StocksStatus = '2' THEN '出庫' 
                     WHEN st.StocksStatus = '3' THEN '退回' 
                     ELSE '其他' END AS StatusName
            FROM InventoryStocks st
            INNER JOIN Products g ON st.GoodsId = g.GoodsId
            LEFT JOIN ProductCategories pc_s ON g.ScId = pc_s.ClassId AND pc_s.ClassMode = '3'
            LEFT JOIN ProductCategories pc_m ON pc_s.MClassId = pc_m.ClassId AND pc_m.ClassMode = '2'
            LEFT JOIN ProductCategories pc_b ON pc_m.BClassId = pc_b.ClassId AND pc_b.ClassMode = '1'
            WHERE st.StocksStatus IN ('2', '3')"; // 2:出庫, 3:退回

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND st.SiteId = @SiteId";
        }

        if (query.BeginDate.HasValue)
        {
            sql += " AND st.StocksDate >= @BeginDate";
        }

        if (query.EndDate.HasValue)
        {
            sql += " AND st.StocksDate <= @EndDate";
        }

        if (!string.IsNullOrEmpty(query.BId))
        {
            sql += " AND pc_b.BClassId = @BId";
        }

        if (!string.IsNullOrEmpty(query.MId))
        {
            sql += " AND pc_m.MClassId = @MId";
        }

        if (!string.IsNullOrEmpty(query.SId))
        {
            sql += " AND pc_s.ClassId = @SId";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            sql += " AND st.GoodsId = @GoodsId";
        }

        return sql;
    }

    /// <summary>
    /// 固定資產數量彙總表 (SYSA1015)
    /// </summary>
    private string BuildFixedAssetSummaryQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                c.SiteId,
                c.CategoryId AS BId,
                NULL AS MId,
                NULL AS SId,
                c.ConsumableId AS GoodsId,
                c.ConsumableName AS GoodsName,
                c.Unit AS PackUnit,
                c.Unit,
                ISNULL(SUM(c.Quantity), 0) AS Qty,
                0 AS SafeQty,
                @FilterType AS SelectType
            FROM Consumables c
            WHERE c.AssetStatus = '1'"; // 1:固定資產

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND c.SiteId = @SiteId";
        }

        if (!string.IsNullOrEmpty(query.BId))
        {
            sql += " AND c.CategoryId = @BId";
        }

        sql += " GROUP BY c.SiteId, c.CategoryId, c.ConsumableId, c.ConsumableName, c.Unit";

        return sql;
    }

    /// <summary>
    /// 庫房領料進價成本分攤表 (SYSA1016)
    /// </summary>
    private string BuildMaterialCostAllocationQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                st.SiteId,
                pc_b.BClassId AS BId,
                pc_m.MClassId AS MId,
                pc_s.ClassId AS SId,
                g.GoodsId,
                g.GoodsName,
                g.Unit AS PackUnit,
                g.Unit,
                st.Qty,
                st.McAmt AS Price,
                st.StocksNtaxAmt AS Amount,
                st.StocksDate,
                st.SourceId
            FROM InventoryStocks st
            INNER JOIN Products g ON st.GoodsId = g.GoodsId
            LEFT JOIN ProductCategories pc_s ON g.ScId = pc_s.ClassId AND pc_s.ClassMode = '3'
            LEFT JOIN ProductCategories pc_m ON pc_s.MClassId = pc_m.ClassId AND pc_m.ClassMode = '2'
            LEFT JOIN ProductCategories pc_b ON pc_m.BClassId = pc_b.ClassId AND pc_b.ClassMode = '1'
            WHERE st.StocksStatus = '2'"; // 2:出庫

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND st.SiteId = @SiteId";
        }

        if (query.BeginDate.HasValue)
        {
            sql += " AND st.StocksDate >= @BeginDate";
        }

        if (query.EndDate.HasValue)
        {
            sql += " AND st.StocksDate <= @EndDate";
        }

        if (!string.IsNullOrEmpty(query.BId))
        {
            sql += " AND pc_b.BClassId = @BId";
        }

        if (!string.IsNullOrEmpty(query.MId))
        {
            sql += " AND pc_m.MClassId = @MId";
        }

        if (!string.IsNullOrEmpty(query.SId))
        {
            sql += " AND pc_s.ClassId = @SId";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            sql += " AND st.GoodsId = @GoodsId";
        }

        return sql;
    }

    /// <summary>
    /// 工務修繕扣款報表 (SYSA1017)
    /// </summary>
    private string BuildMaintenanceDeductionQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                wm.OrgId AS SiteId,
                NULL AS BId,
                NULL AS MId,
                NULL AS SId,
                wm.WorkId AS GoodsId,
                wm.WorkName AS GoodsName,
                NULL AS PackUnit,
                NULL AS Unit,
                wm.DeductionAmount AS Qty,
                0 AS SafeQty,
                @FilterType AS SelectType,
                wm.MaintainDate,
                wm.MaintainEmp,
                wm.BelongOrg
            FROM WorkMaintainM wm
            WHERE 1=1";

        if (!string.IsNullOrEmpty(query.OrgId))
        {
            sql += " AND wm.OrgId = @OrgId";
        }

        if (!string.IsNullOrEmpty(query.BelongOrg))
        {
            sql += " AND wm.BelongOrg = @BelongOrg";
        }

        if (!string.IsNullOrEmpty(query.MaintainEmp))
        {
            sql += " AND wm.MaintainEmp = @MaintainEmp";
        }

        if (query.BeginDate.HasValue)
        {
            sql += " AND wm.MaintainDate >= @BeginDate";
        }

        if (query.EndDate.HasValue)
        {
            sql += " AND wm.MaintainDate <= @EndDate";
        }

        if (!string.IsNullOrEmpty(query.YearMonth))
        {
            sql += " AND FORMAT(wm.MaintainDate, 'yyyyMM') = @YearMonth";
        }

        return sql;
    }

    /// <summary>
    /// 工務維修件數統計表 (SYSA1018)
    /// </summary>
    private string BuildMaintenanceCountQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                wm.OrgId AS SiteId,
                NULL AS BId,
                NULL AS MId,
                NULL AS SId,
                wm.ApplyType AS GoodsId,
                wm.ApplyType AS GoodsName,
                NULL AS PackUnit,
                NULL AS Unit,
                COUNT(*) AS Qty,
                0 AS SafeQty,
                @FilterType AS SelectType,
                wm.MaintainDate,
                wm.MaintainEmp,
                wm.BelongOrg
            FROM WorkMaintainM wm
            WHERE 1=1";

        if (!string.IsNullOrEmpty(query.OrgId))
        {
            sql += " AND wm.OrgId = @OrgId";
        }

        if (!string.IsNullOrEmpty(query.BelongOrg))
        {
            sql += " AND wm.BelongOrg = @BelongOrg";
        }

        if (!string.IsNullOrEmpty(query.ApplyType))
        {
            sql += " AND wm.ApplyType = @ApplyType";
        }

        if (!string.IsNullOrEmpty(query.YearMonth))
        {
            sql += " AND FORMAT(wm.MaintainDate, 'yyyyMM') = @YearMonth";
        }

        sql += " GROUP BY wm.OrgId, wm.ApplyType, wm.MaintainDate, wm.MaintainEmp, wm.BelongOrg";

        return sql;
    }

    /// <summary>
    /// 工務維修類別統計表 (SYSA1019)
    /// </summary>
    private string BuildMaintenanceCategoryQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                wm.OrgId AS SiteId,
                NULL AS BId,
                NULL AS MId,
                NULL AS SId,
                wm.ApplyType AS GoodsId,
                wm.ApplyType AS GoodsName,
                NULL AS PackUnit,
                NULL AS Unit,
                COUNT(*) AS Qty,
                0 AS SafeQty,
                @FilterType AS SelectType,
                wm.MaintainDate,
                wm.MaintainEmp,
                wm.BelongOrg
            FROM WorkMaintainM wm
            WHERE 1=1";

        if (!string.IsNullOrEmpty(query.OrgId))
        {
            sql += " AND wm.OrgId = @OrgId";
        }

        if (!string.IsNullOrEmpty(query.BelongOrg))
        {
            sql += " AND wm.BelongOrg = @BelongOrg";
        }

        if (!string.IsNullOrEmpty(query.ApplyType))
        {
            sql += " AND wm.ApplyType = @ApplyType";
        }

        if (!string.IsNullOrEmpty(query.YearMonth))
        {
            sql += " AND FORMAT(wm.MaintainDate, 'yyyyMM') = @YearMonth";
        }

        sql += " GROUP BY wm.OrgId, wm.ApplyType, wm.MaintainDate, wm.MaintainEmp, wm.BelongOrg";

        return sql;
    }

    /// <summary>
    /// 盤點差異明細表 (SYSA1020)
    /// </summary>
    private string BuildInventoryDifferenceQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                st.SiteId,
                pc_b.BClassId AS BId,
                pc_m.MClassId AS MId,
                pc_s.ClassId AS SId,
                g.GoodsId,
                g.GoodsName,
                g.Unit AS PackUnit,
                g.Unit,
                st.Qty,
                st.SafeQty,
                (st.Qty - st.SafeQty) AS DifferenceQty,
                st.StocksDate
            FROM InventoryStocks st
            INNER JOIN Products g ON st.GoodsId = g.GoodsId
            LEFT JOIN ProductCategories pc_s ON g.ScId = pc_s.ClassId AND pc_s.ClassMode = '3'
            LEFT JOIN ProductCategories pc_m ON pc_s.MClassId = pc_m.ClassId AND pc_m.ClassMode = '2'
            LEFT JOIN ProductCategories pc_b ON pc_m.BClassId = pc_b.ClassId AND pc_b.ClassMode = '1'
            WHERE ABS(st.Qty - ISNULL(st.SafeQty, 0)) > 0"; // 有差異的記錄

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND st.SiteId = @SiteId";
        }

        if (query.BeginDate.HasValue)
        {
            sql += " AND st.StocksDate >= @BeginDate";
        }

        if (query.EndDate.HasValue)
        {
            sql += " AND st.StocksDate <= @EndDate";
        }

        if (!string.IsNullOrEmpty(query.BId))
        {
            sql += " AND pc_b.BClassId = @BId";
        }

        if (!string.IsNullOrEmpty(query.MId))
        {
            sql += " AND pc_m.MClassId = @MId";
        }

        if (!string.IsNullOrEmpty(query.SId))
        {
            sql += " AND pc_s.ClassId = @SId";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            sql += " AND st.GoodsId = @GoodsId";
        }

        return sql;
    }

    /// <summary>
    /// 耗材進銷存月報表 (SYSA1021)
    /// </summary>
    private string BuildMonthlyCostQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                st.SiteId,
                pc_b.BClassId AS BId,
                pc_m.MClassId AS MId,
                pc_s.ClassId AS SId,
                g.GoodsId,
                g.GoodsName,
                g.Unit AS PackUnit,
                g.Unit,
                SUM(CASE WHEN st.StocksStatus = '1' THEN st.Qty ELSE 0 END) AS InboundQty,
                SUM(CASE WHEN st.StocksStatus = '2' THEN st.Qty ELSE 0 END) AS OutboundQty,
                SUM(CASE WHEN st.StocksStatus = '1' THEN st.Qty ELSE -st.Qty END) AS StockQty,
                FORMAT(st.StocksDate, 'yyyyMM') AS YearMonth
            FROM InventoryStocks st
            INNER JOIN Products g ON st.GoodsId = g.GoodsId
            LEFT JOIN ProductCategories pc_s ON g.ScId = pc_s.ClassId AND pc_s.ClassMode = '3'
            LEFT JOIN ProductCategories pc_m ON pc_s.MClassId = pc_m.ClassId AND pc_m.ClassMode = '2'
            LEFT JOIN ProductCategories pc_b ON pc_m.BClassId = pc_b.ClassId AND pc_b.ClassMode = '1'
            WHERE 1=1";

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND st.SiteId = @SiteId";
        }

        if (!string.IsNullOrEmpty(query.YearMonth))
        {
            sql += " AND FORMAT(st.StocksDate, 'yyyyMM') = @YearMonth";
        }

        if (!string.IsNullOrEmpty(query.StartMonth))
        {
            sql += " AND FORMAT(st.StocksDate, 'yyyyMM') >= @StartMonth";
        }

        if (!string.IsNullOrEmpty(query.EndMonth))
        {
            sql += " AND FORMAT(st.StocksDate, 'yyyyMM') <= @EndMonth";
        }

        if (!string.IsNullOrEmpty(query.BId))
        {
            sql += " AND pc_b.BClassId = @BId";
        }

        if (!string.IsNullOrEmpty(query.MId))
        {
            sql += " AND pc_m.MClassId = @MId";
        }

        if (!string.IsNullOrEmpty(query.SId))
        {
            sql += " AND pc_s.ClassId = @SId";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            sql += " AND st.GoodsId = @GoodsId";
        }

        sql += " GROUP BY st.SiteId, pc_b.BClassId, pc_m.MClassId, pc_s.ClassId, g.GoodsId, g.GoodsName, g.Unit, FORMAT(st.StocksDate, 'yyyyMM')";

        return sql;
    }

    /// <summary>
    /// 公務費用歸屬統計表 (SYSA1022)
    /// </summary>
    private string BuildPublicExpenseQuery(AnalysisReportQueryDto query)
    {
        var sql = @"
            SELECT 
                wm.BelongOrg AS SiteId,
                NULL AS BId,
                NULL AS MId,
                NULL AS SId,
                wm.ApplyType AS GoodsId,
                wm.ApplyType AS GoodsName,
                NULL AS PackUnit,
                NULL AS Unit,
                SUM(wm.DeductionAmount) AS Qty,
                0 AS SafeQty,
                @FilterType AS SelectType,
                wm.MaintainDate,
                wm.MaintainEmp
            FROM WorkMaintainM wm
            WHERE 1=1";

        if (!string.IsNullOrEmpty(query.BelongOrg))
        {
            sql += " AND wm.BelongOrg = @BelongOrg";
        }

        if (!string.IsNullOrEmpty(query.ApplyType))
        {
            sql += " AND wm.ApplyType = @ApplyType";
        }

        if (query.BeginDate.HasValue)
        {
            sql += " AND wm.MaintainDate >= @BeginDate";
        }

        if (query.EndDate.HasValue)
        {
            sql += " AND wm.MaintainDate <= @EndDate";
        }

        if (!string.IsNullOrEmpty(query.YearMonth))
        {
            sql += " AND FORMAT(wm.MaintainDate, 'yyyyMM') = @YearMonth";
        }

        if (!string.IsNullOrEmpty(query.StartMonth))
        {
            sql += " AND FORMAT(wm.MaintainDate, 'yyyyMM') >= @StartMonth";
        }

        if (!string.IsNullOrEmpty(query.EndMonth))
        {
            sql += " AND FORMAT(wm.MaintainDate, 'yyyyMM') <= @EndMonth";
        }

        sql += " GROUP BY wm.BelongOrg, wm.ApplyType, wm.MaintainDate, wm.MaintainEmp";

        return sql;
    }

    /// <summary>
    /// 建立查詢參數
    /// </summary>
    private DynamicParameters BuildReportParameters(AnalysisReportQueryDto query)
    {
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            parameters.Add("SiteId", query.SiteId);
        }

        if (!string.IsNullOrEmpty(query.BId))
        {
            parameters.Add("BId", query.BId);
        }

        if (!string.IsNullOrEmpty(query.MId))
        {
            parameters.Add("MId", query.MId);
        }

        if (!string.IsNullOrEmpty(query.SId))
        {
            parameters.Add("SId", query.SId);
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            parameters.Add("GoodsId", query.GoodsId);
        }

        if (query.BeginDate.HasValue)
        {
            parameters.Add("BeginDate", query.BeginDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            parameters.Add("EndDate", query.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(query.FilterType))
        {
            parameters.Add("FilterType", query.FilterType);
        }

        if (!string.IsNullOrEmpty(query.OrgId))
        {
            parameters.Add("OrgId", query.OrgId);
        }

        if (!string.IsNullOrEmpty(query.YearMonth))
        {
            parameters.Add("YearMonth", query.YearMonth);
        }

        if (!string.IsNullOrEmpty(query.StartMonth))
        {
            parameters.Add("StartMonth", query.StartMonth);
        }

        if (!string.IsNullOrEmpty(query.EndMonth))
        {
            parameters.Add("EndMonth", query.EndMonth);
        }

        if (!string.IsNullOrEmpty(query.MaintainEmp))
        {
            parameters.Add("MaintainEmp", query.MaintainEmp);
        }

        if (!string.IsNullOrEmpty(query.BelongOrg))
        {
            parameters.Add("BelongOrg", query.BelongOrg);
        }

        if (!string.IsNullOrEmpty(query.ApplyType))
        {
            parameters.Add("ApplyType", query.ApplyType);
        }

        return parameters;
    }

    /// <summary>
    /// 取得排序欄位
    /// </summary>
    private string GetSortField(string? sortField)
    {
        return string.IsNullOrEmpty(sortField) ? "GoodsId" : sortField;
    }

    /// <summary>
    /// 取得排序順序
    /// </summary>
    private string GetSortOrder(string? sortOrder)
    {
        return string.IsNullOrEmpty(sortOrder) || sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
    }
}

