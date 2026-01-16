using Dapper;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 分析報表 Repository 實作 (SYSA1011)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class AnalysisReportRepository : BaseRepository, IAnalysisReportRepository
{
    public AnalysisReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<SYSA1011ReportItem>> GetSYSA1011ReportAsync(SYSA1011Query query)
    {
        try
        {
            // 計算庫存數量（根據 InventoryStocks 表）
            var sql = @"
                SELECT 
                    g.GoodsId,
                    g.GoodsName,
                    g.BId,
                    g.MId,
                    g.SId,
                    g.Unit,
                    g.PackUnit,
                    ISNULL(g.SafeQty, 0) AS SafeQty,
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(s.SiteName, '') AS SiteName,
                    ROW_NUMBER() OVER (ORDER BY g.GoodsId) AS SeqNo,
                    CASE 
                        WHEN ISNULL(s.Qty, 0) < ISNULL(g.SafeQty, 0) THEN '低於安全庫存'
                        ELSE '全部'
                    END AS SelectType,
                    ISNULL(s.Qty, 0) AS Qty
                FROM Goods g
                LEFT JOIN (
                    SELECT 
                        SiteId,
                        GoodsId,
                        SUM(CASE WHEN StocksStatus IN ('1', '8') THEN Qty ELSE -Qty END) AS Qty
                    FROM InventoryStocks
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                    GROUP BY SiteId, GoodsId
                ) s ON g.GoodsId = s.GoodsId
                LEFT JOIN (
                    SELECT SiteId, SiteName FROM Sites WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                ) sites ON s.SiteId = sites.SiteId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);

            // 分類篩選
            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND g.BId = @BId";
                parameters.Add("BId", query.BId);
            }

            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND g.MId = @MId";
                parameters.Add("MId", query.MId);
            }

            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND g.SId = @SId";
                parameters.Add("SId", query.SId);
            }

            // 商品代碼篩選
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND g.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            // 篩選類型（低於安全庫存量）
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType == "safety")
            {
                sql += " AND ISNULL(s.Qty, 0) < ISNULL(g.SafeQty, 0)";
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1011ReportItem>(sql, parameters);

            return new PagedResult<SYSA1011ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<List<GoodsCategory>> GetGoodsCategoriesAsync(string categoryType, string? parentId = null)
    {
        try
        {
            var sql = @"
                SELECT CategoryId, CategoryName, CategoryType, ParentId, SeqNo, Status,
                       CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM GoodsCategories
                WHERE CategoryType = @CategoryType AND Status = '1'";

            var parameters = new DynamicParameters();
            parameters.Add("CategoryType", categoryType);

            if (!string.IsNullOrEmpty(parentId))
            {
                sql += " AND ParentId = @ParentId";
                parameters.Add("ParentId", parentId);
            }
            else
            {
                sql += " AND ParentId IS NULL";
            }

            sql += " ORDER BY SeqNo, CategoryId";

            var items = await QueryAsync<GoodsCategory>(sql, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分類失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<Dictionary<string, object>>> GetAnalysisReportAsync(string reportId, AnalysisReportQuery query)
    {
        try
        {
            // 根據報表ID選擇不同的查詢邏輯
            return reportId switch
            {
                "SYSA1011" => ConvertToDictionary(await GetSYSA1011ReportAsync(new SYSA1011Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    GoodsId = query.GoodsId,
                    FilterType = query.FilterType
                })),
                "SYSA1012" => ConvertToDictionary(await GetSYSA1012ReportAsync(new SYSA1012Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    GoodsId = query.GoodsId,
                    ReportMonth = query.YearMonth?.Replace("/", "") ?? query.YearMonth
                })),
                "SYSA1013" => ConvertToDictionary(await GetSYSA1013ReportAsync(new SYSA1013Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    OrgId = query.OrgId,
                    GoodsId = query.GoodsId,
                    BeginDate = query.BeginDate,
                    EndDate = query.EndDate,
                    SupplierId = query.Vendor,
                    Use = query.Use,
                    FilterType = query.FilterType
                })),
                "SYSA1014" => ConvertToDictionary(await GetSYSA1014ReportAsync(new SYSA1014Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    OrgId = query.OrgId,
                    GoodsId = query.GoodsId,
                    BeginDate = query.BeginDate,
                    EndDate = query.EndDate
                })),
                "SYSA1015" => ConvertToDictionary(await GetSYSA1015ReportAsync(new SYSA1015Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    GoodsId = query.GoodsId,
                    YearMonth = query.YearMonth,
                    FilterType = query.FilterType
                })),
                "SYSA1016" => ConvertToDictionary(await GetSYSA1016ReportAsync(new SYSA1016Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    OrgId = query.OrgId,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    GoodsId = query.GoodsId,
                    YearMonth = query.YearMonth,
                    FilterType = query.FilterType
                })),
                "SYSA1017" => ConvertToDictionary(await GetSYSA1017ReportAsync(new SYSA1017Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    OrgId = query.OrgId,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    GoodsId = query.GoodsId,
                    YearMonth = query.YearMonth?.Replace("/", "") ?? query.YearMonth,
                    FilterType = query.FilterType
                })),
                "SYSA1018" => ConvertToDictionary(await GetSYSA1018ReportAsync(new SYSA1018Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    OrgId = query.OrgId,
                    YearMonth = query.YearMonth,
                    FilterType = query.FilterType
                })),
                "SYSA1019" => ConvertToDictionary(await GetSYSA1019ReportAsync(new SYSA1019Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    OrgId = query.OrgId,
                    SiteId = query.SiteId,
                    YearMonth = query.YearMonth,
                    FilterType = query.FilterType
                })),
                "SYSA1020" => ConvertToDictionary(await GetSYSA1020ReportAsync(new SYSA1020Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    PlanId = query.OrgId,
                    ShowType = query.FilterType,
                    FilterType = query.FilterType
                })),
                "SYSA1021" => ConvertToDictionary(await GetSYSA1021ReportAsync(new SYSA1021Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    GoodsId = query.GoodsId,
                    YearMonth = query.YearMonth?.Replace("/", "") ?? query.YearMonth,
                    FilterType = query.FilterType
                })),
                "SYSA1022" => ConvertToDictionary(await GetSYSA1022ReportAsync(new SYSA1022Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BelongStatus = query.BelongStatus,
                    ApplyDateB = query.ApplyDateB,
                    ApplyDateE = query.ApplyDateE,
                    BelongOrg = query.BelongOrg,
                    MaintainEmp = query.MaintainEmp,
                    ApplyType = query.ApplyType
                })),
                "SYSA1023" => ConvertToDictionary(await GetSYSA1023ReportAsync(new SYSA1023Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    ReportType = query.FilterType,
                    SiteId = query.SiteId,
                    BelongStatus = query.BelongStatus,
                    ApplyDateB = query.ApplyDateB,
                    ApplyDateE = query.ApplyDateE,
                    BelongOrg = query.BelongOrg,
                    MaintainEmp = query.MaintainEmp,
                    ApplyType = query.ApplyType
                })),
                "SYSA1024" => ConvertToDictionary(await GetSYSA1024ReportAsync(new SYSA1024Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BelongStatus = query.BelongStatus,
                    ApplyDateB = query.ApplyDateB,
                    ApplyDateE = query.ApplyDateE,
                    BelongOrg = query.BelongOrg,
                    MaintainEmp = query.MaintainEmp,
                    ApplyType = query.ApplyType,
                    OtherCondition1 = query.OtherCondition1,
                    OtherCondition2 = query.OtherCondition2
                })),
                _ => throw new ArgumentException($"不支援的報表ID: {reportId}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢進銷存分析報表失敗: {reportId}", ex);
            throw;
        }
    }

    private PagedResult<Dictionary<string, object>> ConvertToDictionary<T>(PagedResult<T> result)
    {
        var items = result.Items.Select(item =>
        {
            var dict = new Dictionary<string, object>();
            foreach (var prop in typeof(T).GetProperties())
            {
                dict[prop.Name] = prop.GetValue(item) ?? string.Empty;
            }
            return dict;
        }).ToList();

        return new PagedResult<Dictionary<string, object>>
        {
            Items = items,
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            PageSize = result.PageSize
        };
    }

    public async Task<PagedResult<SYSA1013ReportItem>> GetSYSA1013ReportAsync(SYSA1013Query query)
    {
        try
        {
            var sql = @"
                SELECT 
                    TxnNo,
                    TxnDate,
                    BId,
                    MId,
                    SId,
                    GoodsId,
                    GoodsName,
                    PackUnit,
                    Unit,
                    Amt,
                    ApplyQty,
                    Qty,
                    NAmt,
                    Use,
                    Vendor,
                    StocksType,
                    OrgId,
                    OrgAllocation
                FROM MaterialOutboundDetailView
                WHERE 1=1";

            var parameters = new DynamicParameters();

            // 店別篩選
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND EXISTS (SELECT 1 FROM InventoryStocks WHERE SourceId = MaterialOutboundDetailView.TxnNo AND SiteId = @SiteId)";
                parameters.Add("SiteId", query.SiteId);
            }

            // 日期範圍篩選
            if (!string.IsNullOrEmpty(query.BeginDate))
            {
                sql += " AND TxnDate >= @BeginDate";
                parameters.Add("BeginDate", DateTime.Parse(query.BeginDate));
            }

            if (!string.IsNullOrEmpty(query.EndDate))
            {
                sql += " AND TxnDate <= @EndDate";
                parameters.Add("EndDate", DateTime.Parse(query.EndDate).AddDays(1).AddSeconds(-1));
            }

            // 分類篩選
            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND BId = @BId";
                parameters.Add("BId", query.BId);
            }

            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND MId = @MId";
                parameters.Add("MId", query.MId);
            }

            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND SId = @SId";
                parameters.Add("SId", query.SId);
            }

            // 單位篩選
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND OrgId LIKE @OrgId";
                parameters.Add("OrgId", $"%{query.OrgId}%");
            }

            // 商品代碼篩選
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            // 廠商篩選
            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                sql += " AND Vendor LIKE @SupplierId";
                parameters.Add("SupplierId", $"%{query.SupplierId}%");
            }

            // 用途篩選
            if (!string.IsNullOrEmpty(query.Use))
            {
                sql += " AND Use = @Use";
                parameters.Add("Use", query.Use);
            }

            // 篩選類型
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType != "all")
            {
                // 可以根據需要添加特定狀態的篩選
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY TxnDate DESC, TxnNo, GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1013ReportItem>(sql, parameters);

            return new PagedResult<SYSA1013ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材出庫明細表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1012ReportItem>> GetSYSA1012ReportAsync(SYSA1012Query query)
    {
        try
        {
            // 計算月份開始和結束日期
            DateTime beginDate, endDate;
            if (!string.IsNullOrEmpty(query.ReportMonth) && query.ReportMonth.Length == 6)
            {
                var year = int.Parse(query.ReportMonth.Substring(0, 4));
                var month = int.Parse(query.ReportMonth.Substring(4, 2));
                beginDate = new DateTime(year, month, 1);
                endDate = beginDate.AddMonths(1).AddDays(-1);
            }
            else
            {
                // 預設為上個月
                beginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                endDate = beginDate.AddMonths(1).AddDays(-1);
            }

            // 期初日期為上個月最後一天
            var beginDateForBeginQty = beginDate.AddDays(-1);

            var sql = @"
                SELECT 
                    g.GoodsId,
                    g.GoodsName,
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(s.SiteName, '') AS SiteName,
                    @ReportMonth AS ReportMonth,
                    ISNULL(beginQty.BeginQty, 0) AS BeginQty,
                    ISNULL(beginQty.BeginAmt, 0) AS BeginAmt,
                    ISNULL(inQty.InQty, 0) AS InQty,
                    ISNULL(inQty.InAmt, 0) AS InAmt,
                    ISNULL(outQty.OutQty, 0) AS OutQty,
                    ISNULL(outQty.OutAmt, 0) AS OutAmt,
                    ISNULL(endQty.EndQty, 0) AS EndQty,
                    ISNULL(endQty.EndAmt, 0) AS EndAmt
                FROM Goods g
                LEFT JOIN (
                    SELECT SiteId, SiteName FROM Sites WHERE SiteId = @SiteId
                ) s ON 1=1
                LEFT JOIN (
                    SELECT GoodsId, SUM(Qty) AS BeginQty, SUM(StocksNtaxAmt) AS BeginAmt
                    FROM InventoryStocks
                    WHERE StocksDate <= @BeginDateForBeginQty AND (@SiteId IS NULL OR SiteId = @SiteId)
                    GROUP BY GoodsId
                ) beginQty ON g.GoodsId = beginQty.GoodsId
                LEFT JOIN (
                    SELECT GoodsId, SUM(Qty) AS InQty, SUM(StocksNtaxAmt) AS InAmt
                    FROM InventoryStocks
                    WHERE StocksStatus = '1' AND StocksDate >= @BeginDate AND StocksDate <= @EndDate
                        AND (@SiteId IS NULL OR SiteId = @SiteId)
                    GROUP BY GoodsId
                ) inQty ON g.GoodsId = inQty.GoodsId
                LEFT JOIN (
                    SELECT GoodsId, SUM(Qty) AS OutQty, SUM(StocksNtaxAmt) AS OutAmt
                    FROM InventoryStocks
                    WHERE StocksStatus IN ('2', '3', '4', '5', '6') AND StocksDate >= @BeginDate AND StocksDate <= @EndDate
                        AND (@SiteId IS NULL OR SiteId = @SiteId)
                    GROUP BY GoodsId
                ) outQty ON g.GoodsId = outQty.GoodsId
                LEFT JOIN (
                    SELECT GoodsId, SUM(Qty) AS EndQty, SUM(StocksNtaxAmt) AS EndAmt
                    FROM InventoryStocks
                    WHERE StocksDate <= @EndDate AND (@SiteId IS NULL OR SiteId = @SiteId)
                    GROUP BY GoodsId
                ) endQty ON g.GoodsId = endQty.GoodsId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("ReportMonth", query.ReportMonth ?? string.Empty);
            parameters.Add("BeginDate", beginDate);
            parameters.Add("EndDate", endDate);
            parameters.Add("BeginDateForBeginQty", beginDateForBeginQty);

            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND g.BId = @BId";
                parameters.Add("BId", query.BId);
            }

            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND g.MId = @MId";
                parameters.Add("MId", query.MId);
            }

            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND g.SId = @SId";
                parameters.Add("SId", query.SId);
            }

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND g.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.QueryAsync<SYSA1012ReportItem>(sql, parameters);

            var items = rows.ToList();

            return new PagedResult<SYSA1012ReportItem>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢進銷存月報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1014ReportItem>> GetSYSA1014ReportAsync(SYSA1014Query query)
    {
        try
        {
            // 解析日期範圍
            DateTime? beginDate = null;
            DateTime? endDate = null;
            
            if (!string.IsNullOrEmpty(query.BeginDate))
            {
                beginDate = DateTime.Parse(query.BeginDate);
            }
            
            if (!string.IsNullOrEmpty(query.EndDate))
            {
                endDate = DateTime.Parse(query.EndDate).AddDays(1).AddSeconds(-1);
            }

            var sql = @"
                SELECT 
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(s.SiteName, '') AS SiteName,
                    '商品分析報表' AS ReportName,
                    CASE 
                        WHEN @BeginDate IS NOT NULL AND @EndDate IS NOT NULL 
                        THEN CONVERT(NVARCHAR(10), @BeginDate, 120) + ' ~ ' + CONVERT(NVARCHAR(10), @EndDate, 120)
                        ELSE ''
                    END AS SelectDate,
                    '全部' AS SelectType,
                    ROW_NUMBER() OVER (ORDER BY g.GoodsId) AS SeqNo,
                    g.BId,
                    g.MId,
                    g.SId,
                    g.GoodsId,
                    g.GoodsName,
                    g.PackUnit,
                    g.Unit,
                    ISNULL(purchaseQty.PurchaseQty, 0) AS PurchaseQty,
                    ISNULL(salesQty.SalesQty, 0) AS SalesQty,
                    ISNULL(stockQty.StockQty, 0) AS StockQty,
                    ISNULL(@BeginDate, GETDATE()) AS BeginDate,
                    ISNULL(@EndDate, GETDATE()) AS EndDate
                FROM Goods g
                LEFT JOIN (
                    SELECT SiteId, SiteName FROM Sites WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                ) s ON 1=1
                LEFT JOIN (
                    SELECT 
                        pod.GoodsId,
                        SUM(CASE WHEN po.OrderType = 'PO' THEN pod.OrderQty ELSE -pod.OrderQty END) AS PurchaseQty
                    FROM PurchaseOrderDetails pod
                    INNER JOIN PurchaseOrders po ON pod.OrderId = po.OrderId
                    WHERE po.Status IN ('A', 'C')
                        AND (@BeginDate IS NULL OR po.OrderDate >= @BeginDate)
                        AND (@EndDate IS NULL OR po.OrderDate <= @EndDate)
                        AND (@SiteId IS NULL OR po.SiteId = @SiteId)
                    GROUP BY pod.GoodsId
                ) purchaseQty ON g.GoodsId = purchaseQty.GoodsId
                LEFT JOIN (
                    SELECT 
                        sod.GoodsId,
                        SUM(CASE WHEN so.OrderType = 'SO' THEN sod.OrderQty ELSE -sod.OrderQty END) AS SalesQty
                    FROM SalesOrderDetails sod
                    INNER JOIN SalesOrders so ON sod.OrderId = so.OrderId
                    WHERE so.Status IN ('A', 'C', 'O')
                        AND (@BeginDate IS NULL OR so.OrderDate >= @BeginDate)
                        AND (@EndDate IS NULL OR so.OrderDate <= @EndDate)
                        AND (@SiteId IS NULL OR so.SiteId = @SiteId)
                    GROUP BY sod.GoodsId
                ) salesQty ON g.GoodsId = salesQty.GoodsId
                LEFT JOIN (
                    SELECT 
                        GoodsId,
                        SUM(CASE WHEN StocksStatus IN ('1', '8') THEN Qty ELSE -Qty END) AS StockQty
                    FROM InventoryStocks
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                    GROUP BY GoodsId
                ) stockQty ON g.GoodsId = stockQty.GoodsId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("BeginDate", beginDate);
            parameters.Add("EndDate", endDate);

            // 分類篩選
            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND g.BId = @BId";
                parameters.Add("BId", query.BId);
            }

            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND g.MId = @MId";
                parameters.Add("MId", query.MId);
            }

            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND g.SId = @SId";
                parameters.Add("SId", query.SId);
            }

            // 單位篩選
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND EXISTS (SELECT 1 FROM InventoryStocks WHERE GoodsId = g.GoodsId AND OrgId LIKE @OrgId)";
                parameters.Add("OrgId", $"%{query.OrgId}%");
            }

            // 商品代碼篩選
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND g.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.QueryAsync<SYSA1014ReportItem>(sql, parameters);

            var items = rows.ToList();

            return new PagedResult<SYSA1014ReportItem>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1015ReportItem>> GetSYSA1015ReportAsync(SYSA1015Query query)
    {
        try
        {
            // 計算庫存數量（根據 InventoryStocks 表）
            var sql = @"
                SELECT 
                    g.GoodsId,
                    g.GoodsName,
                    g.BId,
                    g.MId,
                    g.SId,
                    g.Unit,
                    g.PackUnit,
                    ISNULL(g.SafeQty, 0) AS SafeQty,
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    ROW_NUMBER() OVER (ORDER BY g.GoodsId) AS SeqNo,
                    CASE 
                        WHEN ISNULL(s.Qty, 0) < ISNULL(g.SafeQty, 0) THEN '低於安全庫存'
                        ELSE '全部'
                    END AS SelectType,
                    ISNULL(s.Qty, 0) AS Qty,
                    ISNULL(@YearMonth, FORMAT(GETDATE(), 'yyyy-MM')) AS YearMonth
                FROM Goods g
                LEFT JOIN (
                    SELECT 
                        SiteId,
                        GoodsId,
                        SUM(CASE WHEN StocksStatus IN ('1', '8') THEN Qty ELSE -Qty END) AS Qty
                    FROM InventoryStocks
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@YearMonth IS NULL OR FORMAT(StocksDate, 'yyyy-MM') = @YearMonth)
                    GROUP BY SiteId, GoodsId
                ) s ON g.GoodsId = s.GoodsId
                LEFT JOIN (
                    SELECT SiteId, SiteName FROM Sites WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                ) sites ON s.SiteId = sites.SiteId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("YearMonth", query.YearMonth);

            // 分類篩選
            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND g.BId = @BId";
                parameters.Add("BId", query.BId);
            }

            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND g.MId = @MId";
                parameters.Add("MId", query.MId);
            }

            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND g.SId = @SId";
                parameters.Add("SId", query.SId);
            }

            // 商品代碼篩選
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND g.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            // 篩選類型（低於安全庫存量）
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType == "safety")
            {
                sql += " AND ISNULL(s.Qty, 0) < ISNULL(g.SafeQty, 0)";
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1015ReportItem>(sql, parameters);

            return new PagedResult<SYSA1015ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1016ReportItem>> GetSYSA1016ReportAsync(SYSA1016Query query)
    {
        try
        {
            // 計算庫存數量（根據 InventoryStocks 表）
            var sql = @"
                SELECT 
                    g.GoodsId,
                    g.GoodsName,
                    g.BId,
                    g.MId,
                    g.SId,
                    g.Unit,
                    g.PackUnit,
                    ISNULL(g.SafeQty, 0) AS SafeQty,
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    ROW_NUMBER() OVER (ORDER BY g.GoodsId) AS SeqNo,
                    CASE 
                        WHEN ISNULL(s.Qty, 0) < ISNULL(g.SafeQty, 0) THEN '低於安全庫存'
                        ELSE '全部'
                    END AS SelectType,
                    ISNULL(s.Qty, 0) AS Qty,
                    ISNULL(@YearMonth, FORMAT(GETDATE(), 'yyyy-MM')) AS YearMonth
                FROM Goods g
                LEFT JOIN (
                    SELECT 
                        SiteId,
                        GoodsId,
                        SUM(CASE WHEN StocksStatus IN ('1', '8') THEN Qty ELSE -Qty END) AS Qty
                    FROM InventoryStocks
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@YearMonth IS NULL OR FORMAT(StocksDate, 'yyyy-MM') = @YearMonth)
                    GROUP BY SiteId, GoodsId
                ) s ON g.GoodsId = s.GoodsId
                LEFT JOIN (
                    SELECT SiteId, SiteName, OrgId 
                    FROM Sites 
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@OrgId IS NULL OR OrgId = @OrgId)
                ) sites ON s.SiteId = sites.SiteId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("OrgId", query.OrgId);
            parameters.Add("YearMonth", query.YearMonth);

            // 組織篩選
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND sites.OrgId = @OrgId";
            }

            // 分類篩選
            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND g.BId = @BId";
                parameters.Add("BId", query.BId);
            }

            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND g.MId = @MId";
                parameters.Add("MId", query.MId);
            }

            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND g.SId = @SId";
                parameters.Add("SId", query.SId);
            }

            // 商品代碼篩選
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND g.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            // 篩選類型（低於安全庫存量）
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType == "1")
            {
                sql += " AND ISNULL(s.Qty, 0) < ISNULL(g.SafeQty, 0)";
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1016ReportItem>(sql, parameters);

            return new PagedResult<SYSA1016ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1017ReportItem>> GetSYSA1017ReportAsync(SYSA1017Query query)
    {
        try
        {
            // 計算庫存數量（根據 InventoryStocks 表）
            var sql = @"
                SELECT 
                    g.GoodsId,
                    g.GoodsName,
                    g.BId,
                    g.MId,
                    g.SId,
                    g.Unit,
                    g.PackUnit,
                    ISNULL(g.SafeQty, 0) AS SafeQty,
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    ROW_NUMBER() OVER (ORDER BY g.GoodsId) AS SeqNo,
                    CASE 
                        WHEN ISNULL(s.Qty, 0) < ISNULL(g.SafeQty, 0) THEN '低於安全庫存量'
                        ELSE '全部'
                    END AS SelectType,
                    ISNULL(s.Qty, 0) AS Qty,
                    ISNULL(@YearMonth, FORMAT(GETDATE(), 'yyyyMM')) AS YearMonth,
                    ISNULL(sites.OrgId, @OrgId) AS OrgId
                FROM Goods g
                LEFT JOIN (
                    SELECT 
                        SiteId,
                        GoodsId,
                        SUM(CASE WHEN StocksStatus IN ('1', '8') THEN Qty ELSE -Qty END) AS Qty
                    FROM InventoryStocks
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@YearMonth IS NULL OR FORMAT(StocksDate, 'yyyyMM') = @YearMonth)
                    GROUP BY SiteId, GoodsId
                ) s ON g.GoodsId = s.GoodsId
                LEFT JOIN (
                    SELECT SiteId, SiteName, OrgId 
                    FROM Sites 
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@OrgId IS NULL OR OrgId = @OrgId)
                ) sites ON s.SiteId = sites.SiteId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("OrgId", query.OrgId);
            parameters.Add("YearMonth", query.YearMonth);

            // 組織篩選
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND sites.OrgId = @OrgId";
            }

            // 分類篩選
            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND g.BId = @BId";
                parameters.Add("BId", query.BId);
            }

            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND g.MId = @MId";
                parameters.Add("MId", query.MId);
            }

            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND g.SId = @SId";
                parameters.Add("SId", query.SId);
            }

            // 商品代碼篩選
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND g.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            // 篩選類型（低於安全庫存量）
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType == "低於安全庫存量")
            {
                sql += " AND ISNULL(s.Qty, 0) < ISNULL(g.SafeQty, 0)";
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1017ReportItem>(sql, parameters);

            return new PagedResult<SYSA1017ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1018ReportItem>> GetSYSA1018ReportAsync(SYSA1018Query query)
    {
        try
        {
            // 查詢工務維修件數統計報表 (SYSA1018)
            // 根據計劃文件，需要查詢 WorkMaintainM 表，統計維修件數
            // 使用 WorkMaintainM 表，類似於 SYSA1022 的實現
            var sql = @"
                SELECT 
                    ISNULL(wm.OrgId, @OrgId) AS OrgId,
                    ISNULL(org.OrgName, '') AS OrgName,
                    '工務維修件數統計表' AS ReportName,
                    ISNULL(@YearMonth, FORMAT(wm.ApplyDate, 'yyyy-MM')) AS YearMonth,
                    wm.ApplyType AS MaintenanceType,
                    '已完成' AS MaintenanceStatus, -- WorkMaintainM 表沒有狀態欄位，使用預設值
                    COUNT(DISTINCT wm.TxnNo) AS ItemCount,
                    0 AS TotalCount
                FROM WorkMaintainM wm
                LEFT JOIN Organizations org ON wm.OrgId = org.OrgId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("OrgId", query.OrgId);
            parameters.Add("YearMonth", query.YearMonth);
            parameters.Add("FilterType", query.FilterType);

            // 組織單位篩選
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND wm.OrgId = @OrgId";
            }

            // 年月篩選 (YYYY-MM 格式)
            if (!string.IsNullOrEmpty(query.YearMonth))
            {
                sql += " AND FORMAT(wm.ApplyDate, 'yyyy-MM') = @YearMonth";
            }

            // 篩選類型（根據 FilterType 進行篩選，目前 WorkMaintainM 表沒有狀態欄位，暫時不處理）
            // 如果需要根據狀態篩選，可能需要從其他表或欄位獲取狀態資訊
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType != "全部")
            {
                // 暫時不處理，因為 WorkMaintainM 表沒有 MaintenanceStatus 欄位
                // 如果需要，可以根據實際業務需求調整
            }

            // 計算總數（先不分組）
            var countSql = $@"
                SELECT COUNT(*)
                FROM (
                    SELECT 
                        wm.OrgId,
                        FORMAT(wm.ApplyDate, 'yyyy-MM') AS YearMonth,
                        wm.ApplyType
                    FROM WorkMaintainM wm
                    WHERE 1=1
                        {(!string.IsNullOrEmpty(query.OrgId) ? " AND wm.OrgId = @OrgId" : "")}
                        {(!string.IsNullOrEmpty(query.YearMonth) ? " AND FORMAT(wm.ApplyDate, 'yyyy-MM') = @YearMonth" : "")}
                    GROUP BY wm.OrgId, FORMAT(wm.ApplyDate, 'yyyy-MM'), wm.ApplyType
                ) AS t";
            
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 計算總件數
            var totalItemCountSql = @"
                SELECT COUNT(DISTINCT wm.TxnNo)
                FROM WorkMaintainM wm
                WHERE 1=1";
            
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                totalItemCountSql += " AND wm.OrgId = @OrgId";
            }
            if (!string.IsNullOrEmpty(query.YearMonth))
            {
                totalItemCountSql += " AND FORMAT(wm.ApplyDate, 'yyyy-MM') = @YearMonth";
            }
            
            var totalItemCount = await ExecuteScalarAsync<int>(totalItemCountSql, parameters);

            // 分頁
            sql += " GROUP BY wm.OrgId, org.OrgName, FORMAT(wm.ApplyDate, 'yyyy-MM'), wm.ApplyType";
            sql += " ORDER BY wm.OrgId, FORMAT(wm.ApplyDate, 'yyyy-MM'), wm.ApplyType";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1018ReportItem>(sql, parameters);

            // 更新每個項目的 TotalCount
            foreach (var item in items)
            {
                item.TotalCount = totalItemCount;
            }

            return new PagedResult<SYSA1018ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修件數統計報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1019ReportItem>> GetSYSA1019ReportAsync(SYSA1019Query query)
    {
        try
        {
            // 查詢商品分析報表 (SYSA1019)
            // 根據計劃文件，需要查詢 Goods、InventoryStocks、Sites、Organizations 表
            var sql = @"
                SELECT 
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    ISNULL(sites.OrgId, @OrgId) AS OrgId,
                    ISNULL(o.OrgName, '') AS OrgName,
                    '商品分析報表' AS ReportName,
                    ISNULL(@YearMonth, FORMAT(GETDATE(), 'yyyy-MM')) AS YearMonth,
                    ISNULL(@FilterType, '全部') AS FilterType,
                    ROW_NUMBER() OVER (ORDER BY g.GoodsId) AS SeqNo,
                    g.GoodsId AS GoodsId,
                    g.GoodsName AS GoodsName
                FROM Goods g
                LEFT JOIN (
                    SELECT 
                        SiteId,
                        GoodsId
                    FROM InventoryStocks
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@YearMonth IS NULL OR FORMAT(StocksDate, 'yyyy-MM') = @YearMonth)
                    GROUP BY SiteId, GoodsId
                ) s ON g.GoodsId = s.GoodsId
                LEFT JOIN (
                    SELECT SiteId, SiteName, OrgId 
                    FROM Sites 
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@OrgId IS NULL OR OrgId = @OrgId)
                ) sites ON s.SiteId = sites.SiteId
                LEFT JOIN Organizations o ON sites.OrgId = o.OrgId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("OrgId", query.OrgId);
            parameters.Add("YearMonth", query.YearMonth);
            parameters.Add("FilterType", query.FilterType);

            // 組織篩選
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND sites.OrgId = @OrgId";
            }

            // 店別篩選
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND s.SiteId = @SiteId";
            }

            // 年月篩選 (YYYY-MM 格式)
            if (!string.IsNullOrEmpty(query.YearMonth))
            {
                sql += " AND EXISTS (SELECT 1 FROM InventoryStocks st WHERE st.GoodsId = g.GoodsId AND FORMAT(st.StocksDate, 'yyyy-MM') = @YearMonth)";
            }

            // 篩選類型
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType != "全部")
            {
                // 根據實際業務需求調整篩選邏輯
                // 目前先不處理特定條件，因為計劃文件中沒有明確說明
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1019ReportItem>(sql, parameters);

            return new PagedResult<SYSA1019ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1020ReportItem>> GetSYSA1020ReportAsync(SYSA1020Query query)
    {
        try
        {
            // 查詢商品分析報表 (SYSA1020)
            // 根據計劃文件，需要查詢 Goods、InventoryStocks、Sites、Plans 表
            var sql = @"
                SELECT 
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    ISNULL(p.PlanId, @PlanId) AS PlanId,
                    ISNULL(p.PlanName, '') AS PlanName,
                    ISNULL(@ShowType, '') AS ShowType,
                    ISNULL(@FilterType, '全部') AS FilterType,
                    ROW_NUMBER() OVER (ORDER BY g.GoodsId) AS SeqNo,
                    g.GoodsId AS GoodsId,
                    g.GoodsName AS GoodsName
                FROM Goods g
                LEFT JOIN (
                    SELECT 
                        SiteId,
                        GoodsId,
                        PlanId
                    FROM InventoryStocks
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@PlanId IS NULL OR PlanId = @PlanId)
                    GROUP BY SiteId, GoodsId, PlanId
                ) s ON g.GoodsId = s.GoodsId
                LEFT JOIN Sites sites ON s.SiteId = sites.SiteId
                LEFT JOIN Plans p ON s.PlanId = p.PlanId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("PlanId", query.PlanId);
            parameters.Add("ShowType", query.ShowType);
            parameters.Add("FilterType", query.FilterType);

            // 店別篩選
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND s.SiteId = @SiteId";
            }

            // 計劃ID篩選
            if (!string.IsNullOrEmpty(query.PlanId))
            {
                sql += " AND s.PlanId = @PlanId";
            }

            // 顯示類型篩選
            if (!string.IsNullOrEmpty(query.ShowType) && query.ShowType != "全部")
            {
                // 根據實際業務需求調整篩選邏輯
                // 目前先不處理特定條件，因為計劃文件中沒有明確說明
            }

            // 篩選類型
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType != "全部")
            {
                // 根據實際業務需求調整篩選邏輯
                // 目前先不處理特定條件，因為計劃文件中沒有明確說明
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1020ReportItem>(sql, parameters);

            return new PagedResult<SYSA1020ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1021ReportItem>> GetSYSA1021ReportAsync(SYSA1021Query query)
    {
        try
        {
            // 查詢月成本報表 (SYSA1021)
            // 根據計劃文件，需要查詢 Goods、GoodsCosts、Sites 表
            // 如果 GoodsCosts 表不存在，則從 InventoryStocks 表計算成本
            var sql = @"
                SELECT 
                    ISNULL(gc.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    '月成本報表' AS ReportName,
                    ISNULL(gc.YearMonth, @YearMonth) AS YearMonth,
                    g.BId AS BId,
                    g.MId AS MId,
                    g.SId AS SId,
                    g.GoodsId AS GoodsId,
                    g.GoodsName AS GoodsName,
                    ISNULL(gc.Qty, 0) AS Qty,
                    ISNULL(gc.CostAmount, 0) AS CostAmount,
                    CASE 
                        WHEN ISNULL(gc.Qty, 0) > 0 THEN ISNULL(gc.CostAmount, 0) / gc.Qty
                        ELSE 0
                    END AS AvgCost
                FROM Goods g
                LEFT JOIN (
                    SELECT 
                        SiteId,
                        GoodsId,
                        YearMonth,
                        SUM(Qty) AS Qty,
                        SUM(CostAmount) AS CostAmount
                    FROM GoodsCosts
                    WHERE (@SiteId IS NULL OR SiteId = @SiteId)
                        AND (@YearMonth IS NULL OR YearMonth = @YearMonth)
                    GROUP BY SiteId, GoodsId, YearMonth
                ) gc ON g.GoodsId = gc.GoodsId
                LEFT JOIN Sites sites ON gc.SiteId = sites.SiteId
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("BId", query.BId);
            parameters.Add("MId", query.MId);
            parameters.Add("SId", query.SId);
            parameters.Add("GoodsId", query.GoodsId);
            parameters.Add("YearMonth", query.YearMonth);
            parameters.Add("FilterType", query.FilterType);

            // 大分類篩選
            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND g.BId = @BId";
            }

            // 中分類篩選
            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND g.MId = @MId";
            }

            // 小分類篩選
            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND g.SId = @SId";
            }

            // 商品代碼篩選
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND g.GoodsId = @GoodsId";
            }

            // 年月篩選
            if (!string.IsNullOrEmpty(query.YearMonth))
            {
                sql += " AND gc.YearMonth = @YearMonth";
            }

            // 篩選類型（全部、有成本、無成本）
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType != "全部")
            {
                if (query.FilterType == "有成本")
                {
                    sql += " AND ISNULL(gc.CostAmount, 0) > 0";
                }
                else if (query.FilterType == "無成本")
                {
                    sql += " AND (ISNULL(gc.CostAmount, 0) = 0 OR gc.CostAmount IS NULL)";
                }
            }

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId, gc.YearMonth";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1021ReportItem>(sql, parameters);

            return new PagedResult<SYSA1021ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢月成本報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1022ReportItem>> GetSYSA1022ReportAsync(SYSA1022Query query)
    {
        try
        {
            // 查詢工務維修統計報表 (SYSA1022)
            // 根據計劃文件，需要查詢 WorkMaintainM 表，統計申請件數和總金額
            var sql = @"
                SELECT 
                    ISNULL(wm.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    '工務維修統計報表' AS ReportName,
                    wm.BelongStatus AS BelongStatus,
                    CONVERT(VARCHAR(10), @ApplyDateB, 120) AS ApplyDateB,
                    CONVERT(VARCHAR(10), @ApplyDateE, 120) AS ApplyDateE,
                    wm.BelongOrg AS BelongOrg,
                    wm.MaintainEmp AS MaintainEmp,
                    wm.ApplyType AS ApplyType,
                    COUNT(DISTINCT wm.TxnNo) AS RequestCount,
                    ISNULL(SUM(d.Amount), 0) AS TotalAmount
                FROM WorkMaintainM wm
                LEFT JOIN Sites sites ON wm.SiteId = sites.SiteId
                LEFT JOIN (
                    SELECT 
                        TxnNo,
                        SUM(Amount) AS Amount
                    FROM WorkMaintainD
                    GROUP BY TxnNo
                ) d ON wm.TxnNo = d.TxnNo
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            
            // 日期範圍
            if (!string.IsNullOrEmpty(query.ApplyDateB))
            {
                parameters.Add("ApplyDateB", DateTime.Parse(query.ApplyDateB));
                sql += " AND wm.ApplyDate >= @ApplyDateB";
            }
            else
            {
                parameters.Add("ApplyDateB", DateTime.Now.AddMonths(-1));
            }

            if (!string.IsNullOrEmpty(query.ApplyDateE))
            {
                parameters.Add("ApplyDateE", DateTime.Parse(query.ApplyDateE));
                sql += " AND wm.ApplyDate <= @ApplyDateE";
            }
            else
            {
                parameters.Add("ApplyDateE", DateTime.Now);
            }

            // 店別篩選
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND wm.SiteId = @SiteId";
            }

            // 費用負擔篩選
            if (!string.IsNullOrEmpty(query.BelongStatus))
            {
                sql += " AND wm.BelongStatus = @BelongStatus";
                parameters.Add("BelongStatus", query.BelongStatus);
            }

            // 費用歸屬單位篩選
            if (!string.IsNullOrEmpty(query.BelongOrg))
            {
                sql += " AND wm.BelongOrg = @BelongOrg";
                parameters.Add("BelongOrg", query.BelongOrg);
            }

            // 維保人員篩選（支援多選，以分號分隔）
            if (!string.IsNullOrEmpty(query.MaintainEmp))
            {
                sql += " AND (wm.MaintainEmp LIKE '%' + @MaintainEmp + '%' OR wm.MaintainEmp = @MaintainEmp)";
                parameters.Add("MaintainEmp", query.MaintainEmp);
            }

            // 請修類別篩選（支援多選，以分號分隔）
            if (!string.IsNullOrEmpty(query.ApplyType))
            {
                sql += " AND (wm.ApplyType LIKE '%' + @ApplyType + '%' OR wm.ApplyType = @ApplyType)";
                parameters.Add("ApplyType", query.ApplyType);
            }

            // 分組
            sql += @"
                GROUP BY 
                    wm.SiteId,
                    sites.SiteName,
                    wm.BelongStatus,
                    wm.BelongOrg,
                    wm.MaintainEmp,
                    wm.ApplyType";

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY wm.SiteId, wm.BelongStatus";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1022ReportItem>(sql, parameters);

            return new PagedResult<SYSA1022ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修統計報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1023ReportItem>> GetSYSA1023ReportAsync(SYSA1023Query query)
    {
        try
        {
            // 查詢工務維修統計報表(報表類型) (SYSA1023)
            // 根據計劃文件，需要查詢 WorkMaintainM 表，統計申請件數和總金額，並支援報表類型篩選
            var sql = @"
                SELECT 
                    ISNULL(wm.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    '工務維修統計報表(報表類型)' AS ReportName,
                    wm.ReportType AS ReportType,
                    wm.BelongStatus AS BelongStatus,
                    CONVERT(VARCHAR(10), @ApplyDateB, 120) AS ApplyDateB,
                    CONVERT(VARCHAR(10), @ApplyDateE, 120) AS ApplyDateE,
                    wm.BelongOrg AS BelongOrg,
                    wm.MaintainEmp AS MaintainEmp,
                    wm.ApplyType AS ApplyType,
                    COUNT(DISTINCT wm.TxnNo) AS RequestCount,
                    ISNULL(SUM(d.Amount), 0) AS TotalAmount
                FROM WorkMaintainM wm
                LEFT JOIN Sites sites ON wm.SiteId = sites.SiteId
                LEFT JOIN (
                    SELECT 
                        TxnNo,
                        SUM(Amount) AS Amount
                    FROM WorkMaintainD
                    GROUP BY TxnNo
                ) d ON wm.TxnNo = d.TxnNo
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("ReportType", query.ReportType ?? string.Empty);
            
            // 日期範圍
            if (!string.IsNullOrEmpty(query.ApplyDateB))
            {
                parameters.Add("ApplyDateB", DateTime.Parse(query.ApplyDateB));
                sql += " AND wm.ApplyDate >= @ApplyDateB";
            }
            else
            {
                parameters.Add("ApplyDateB", DateTime.Now.AddMonths(-1));
            }

            if (!string.IsNullOrEmpty(query.ApplyDateE))
            {
                parameters.Add("ApplyDateE", DateTime.Parse(query.ApplyDateE));
                sql += " AND wm.ApplyDate <= @ApplyDateE";
            }
            else
            {
                parameters.Add("ApplyDateE", DateTime.Now);
            }

            // 店別篩選
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND wm.SiteId = @SiteId";
            }

            // 報表類型篩選
            if (!string.IsNullOrEmpty(query.ReportType))
            {
                sql += " AND wm.ReportType = @ReportType";
            }

            // 費用負擔篩選
            if (!string.IsNullOrEmpty(query.BelongStatus))
            {
                sql += " AND wm.BelongStatus = @BelongStatus";
                parameters.Add("BelongStatus", query.BelongStatus);
            }

            // 費用歸屬單位篩選
            if (!string.IsNullOrEmpty(query.BelongOrg))
            {
                sql += " AND wm.BelongOrg = @BelongOrg";
                parameters.Add("BelongOrg", query.BelongOrg);
            }

            // 維保人員篩選（支援多選，以分號分隔）
            if (!string.IsNullOrEmpty(query.MaintainEmp))
            {
                sql += " AND (wm.MaintainEmp LIKE '%' + @MaintainEmp + '%' OR wm.MaintainEmp = @MaintainEmp)";
                parameters.Add("MaintainEmp", query.MaintainEmp);
            }

            // 請修類別篩選（支援多選，以分號分隔）
            if (!string.IsNullOrEmpty(query.ApplyType))
            {
                sql += " AND (wm.ApplyType LIKE '%' + @ApplyType + '%' OR wm.ApplyType = @ApplyType)";
                parameters.Add("ApplyType", query.ApplyType);
            }

            // 分組
            sql += @"
                GROUP BY 
                    wm.SiteId,
                    sites.SiteName,
                    wm.ReportType,
                    wm.BelongStatus,
                    wm.BelongOrg,
                    wm.MaintainEmp,
                    wm.ApplyType";

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY wm.SiteId, wm.ReportType, wm.BelongStatus";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1023ReportItem>(sql, parameters);

            return new PagedResult<SYSA1023ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修統計報表(報表類型)失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1024ReportItem>> GetSYSA1024ReportAsync(SYSA1024Query query)
    {
        try
        {
            // 查詢工務維修統計報表(其他) (SYSA1024)
            // 根據計劃文件，需要查詢 WorkMaintainM 表，統計申請件數和總金額，並支援其他查詢條件
            var sql = @"
                SELECT 
                    ISNULL(wm.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    '工務維修統計報表(其他)' AS ReportName,
                    wm.BelongStatus AS BelongStatus,
                    CONVERT(VARCHAR(10), @ApplyDateB, 120) AS ApplyDateB,
                    CONVERT(VARCHAR(10), @ApplyDateE, 120) AS ApplyDateE,
                    wm.BelongOrg AS BelongOrg,
                    wm.MaintainEmp AS MaintainEmp,
                    wm.ApplyType AS ApplyType,
                    @OtherCondition1 AS OtherCondition1,
                    @OtherCondition2 AS OtherCondition2,
                    COUNT(DISTINCT wm.TxnNo) AS RequestCount,
                    ISNULL(SUM(d.Amount), 0) AS TotalAmount
                FROM WorkMaintainM wm
                LEFT JOIN Sites sites ON wm.SiteId = sites.SiteId
                LEFT JOIN (
                    SELECT 
                        TxnNo,
                        SUM(Amount) AS Amount
                    FROM WorkMaintainD
                    GROUP BY TxnNo
                ) d ON wm.TxnNo = d.TxnNo
                WHERE 1=1";

            var parameters = new DynamicParameters();
            parameters.Add("SiteId", query.SiteId);
            parameters.Add("OtherCondition1", query.OtherCondition1 ?? string.Empty);
            parameters.Add("OtherCondition2", query.OtherCondition2 ?? string.Empty);
            
            // 日期範圍
            if (!string.IsNullOrEmpty(query.ApplyDateB))
            {
                parameters.Add("ApplyDateB", DateTime.Parse(query.ApplyDateB));
                sql += " AND wm.ApplyDate >= @ApplyDateB";
            }
            else
            {
                parameters.Add("ApplyDateB", DateTime.Now.AddMonths(-1));
            }

            if (!string.IsNullOrEmpty(query.ApplyDateE))
            {
                parameters.Add("ApplyDateE", DateTime.Parse(query.ApplyDateE));
                sql += " AND wm.ApplyDate <= @ApplyDateE";
            }
            else
            {
                parameters.Add("ApplyDateE", DateTime.Now);
            }

            // 店別篩選
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND wm.SiteId = @SiteId";
            }

            // 費用負擔篩選
            if (!string.IsNullOrEmpty(query.BelongStatus))
            {
                sql += " AND wm.BelongStatus = @BelongStatus";
                parameters.Add("BelongStatus", query.BelongStatus);
            }

            // 費用歸屬單位篩選
            if (!string.IsNullOrEmpty(query.BelongOrg))
            {
                sql += " AND wm.BelongOrg = @BelongOrg";
                parameters.Add("BelongOrg", query.BelongOrg);
            }

            // 維保人員篩選（支援多選，以分號分隔）
            if (!string.IsNullOrEmpty(query.MaintainEmp))
            {
                sql += " AND (wm.MaintainEmp LIKE '%' + @MaintainEmp + '%' OR wm.MaintainEmp = @MaintainEmp)";
                parameters.Add("MaintainEmp", query.MaintainEmp);
            }

            // 請修類別篩選（支援多選，以分號分隔）
            if (!string.IsNullOrEmpty(query.ApplyType))
            {
                sql += " AND (wm.ApplyType LIKE '%' + @ApplyType + '%' OR wm.ApplyType = @ApplyType)";
                parameters.Add("ApplyType", query.ApplyType);
            }

            // 其他查詢條件1（可根據實際業務需求調整）
            if (!string.IsNullOrEmpty(query.OtherCondition1))
            {
                // 這裡可以根據實際業務邏輯添加查詢條件
                // 例如：sql += " AND wm.SomeField LIKE '%' + @OtherCondition1 + '%'";
            }

            // 其他查詢條件2（可根據實際業務需求調整）
            if (!string.IsNullOrEmpty(query.OtherCondition2))
            {
                // 這裡可以根據實際業務邏輯添加查詢條件
                // 例如：sql += " AND wm.AnotherField LIKE '%' + @OtherCondition2 + '%'";
            }

            // 分組
            sql += @"
                GROUP BY 
                    wm.SiteId,
                    sites.SiteName,
                    wm.BelongStatus,
                    wm.BelongOrg,
                    wm.MaintainEmp,
                    wm.ApplyType";

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY wm.SiteId, wm.BelongStatus";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1024ReportItem>(sql, parameters);

            return new PagedResult<SYSA1024ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修統計報表(其他)失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSWC10ReportItem>> GetSYSWC10ReportAsync(SYSWC10Query query)
    {
        try
        {
            // 查詢庫存分析報表 (SYSWC10)
            // 根據計劃文件，需要查詢 InventoryStocks、Goods、Warehouses 表，統計入庫、出庫、當前庫存等資訊
            var sql = @"
                SELECT 
                    ISNULL(s.SiteId, @SiteId) AS SiteId,
                    ISNULL(sites.SiteName, '') AS SiteName,
                    g.GoodsId,
                    g.GoodsName,
                    g.BId AS BigCategoryId,
                    bc.ClassName AS BigCategoryName,
                    g.MId AS MidCategoryId,
                    mc.ClassName AS MidCategoryName,
                    g.SId AS SmallCategoryId,
                    sc.ClassName AS SmallCategoryName,
                    ISNULL(w.WarehouseId, '') AS WarehouseId,
                    ISNULL(w.WarehouseName, '') AS WarehouseName,
                    ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE 0 END), 0) AS InQty,
                    ISNULL(SUM(CASE WHEN s.StocksStatus IN ('2', '3', '5', '6') THEN s.Qty ELSE 0 END), 0) AS OutQty,
                    ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE -s.Qty END), 0) AS CurrentQty,
                    ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.StocksNtaxAmt ELSE -s.StocksNtaxAmt END), 0) AS CurrentAmt,
                    MAX(s.StocksDate) AS LastStockDate,
                    ISNULL(g.SafeQty, 0) AS SafeQty,
                    CASE 
                        WHEN ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE -s.Qty END), 0) < ISNULL(g.SafeQty, 0) THEN 1
                        ELSE 0
                    END AS IsLowStock,
                    CASE 
                        WHEN ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE -s.Qty END), 0) > ISNULL(g.SafeQty * 2, 0) THEN 1
                        ELSE 0
                    END AS IsOverStock
                FROM Goods g
                LEFT JOIN InventoryStocks s ON g.GoodsId = s.GoodsId
                LEFT JOIN Sites sites ON s.SiteId = sites.SiteId
                LEFT JOIN Warehouses w ON w.SiteId = s.SiteId
                LEFT JOIN ProductCategories bc ON g.BId = bc.ClassId AND bc.ClassMode = '1'
                LEFT JOIN ProductCategories mc ON g.MId = mc.ClassId AND mc.ClassMode = '2'
                LEFT JOIN ProductCategories sc ON g.SId = sc.ClassId AND sc.ClassMode = '3'
                WHERE 1=1";

            var parameters = new DynamicParameters();
            
            // 商品代碼範圍
            if (!string.IsNullOrEmpty(query.GoodsIdFrom))
            {
                sql += " AND g.GoodsId >= @GoodsIdFrom";
                parameters.Add("GoodsIdFrom", query.GoodsIdFrom);
            }

            if (!string.IsNullOrEmpty(query.GoodsIdTo))
            {
                sql += " AND g.GoodsId <= @GoodsIdTo";
                parameters.Add("GoodsIdTo", query.GoodsIdTo);
            }

            // 商品名稱
            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                sql += " AND g.GoodsName LIKE @GoodsName";
                parameters.Add("GoodsName", $"%{query.GoodsName}%");
            }

            // 店別篩選
            if (query.SiteIds != null && query.SiteIds.Count > 0)
            {
                sql += " AND s.SiteId IN @SiteIds";
                parameters.Add("SiteIds", query.SiteIds);
            }
            else if (!string.IsNullOrEmpty(query.SiteIds?.FirstOrDefault()))
            {
                sql += " AND s.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteIds.FirstOrDefault());
            }

            // 庫別篩選（根據 SiteId 關聯 Warehouses，但可能不準確，因為 InventoryStocks 表沒有 WarehouseId 字段）
            if (query.WarehouseIds != null && query.WarehouseIds.Count > 0)
            {
                // 根據 SiteId 關聯的 Warehouses 進行篩選
                sql += " AND EXISTS (SELECT 1 FROM Warehouses w2 WHERE w2.SiteId = s.SiteId AND w2.WarehouseId IN @WarehouseIds)";
                parameters.Add("WarehouseIds", query.WarehouseIds);
            }

            // 分類篩選
            if (!string.IsNullOrEmpty(query.BId))
            {
                sql += " AND g.BId = @BId";
                parameters.Add("BId", query.BId);
            }

            if (!string.IsNullOrEmpty(query.MId))
            {
                sql += " AND g.MId = @MId";
                parameters.Add("MId", query.MId);
            }

            if (!string.IsNullOrEmpty(query.SId))
            {
                sql += " AND g.SId = @SId";
                parameters.Add("SId", query.SId);
            }

            // 日期範圍
            if (!string.IsNullOrEmpty(query.DateFrom))
            {
                sql += " AND s.StocksDate >= @DateFrom";
                parameters.Add("DateFrom", DateTime.Parse(query.DateFrom));
            }

            if (!string.IsNullOrEmpty(query.DateTo))
            {
                sql += " AND s.StocksDate <= @DateTo";
                parameters.Add("DateTo", DateTime.Parse(query.DateTo));
            }

            // 庫存數量範圍
            if (query.MinQty.HasValue)
            {
                sql += " HAVING ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE -s.Qty END), 0) >= @MinQty";
                parameters.Add("MinQty", query.MinQty.Value);
            }

            if (query.MaxQty.HasValue)
            {
                if (query.MinQty.HasValue)
                {
                    sql += " AND ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE -s.Qty END), 0) <= @MaxQty";
                }
                else
                {
                    sql += " HAVING ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE -s.Qty END), 0) <= @MaxQty";
                }
                parameters.Add("MaxQty", query.MaxQty.Value);
            }

            // 狀態篩選
            if (!string.IsNullOrEmpty(query.Status))
            {
                if (query.Status == "LowStock")
                {
                    sql += " HAVING ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE -s.Qty END), 0) < ISNULL(g.SafeQty, 0)";
                }
                else if (query.Status == "OverStock")
                {
                    sql += " HAVING ISNULL(SUM(CASE WHEN s.StocksStatus IN ('1', '4') THEN s.Qty ELSE -s.Qty END), 0) > ISNULL(g.SafeQty * 2, 0)";
                }
            }

            // 分組
            sql += @"
                GROUP BY 
                    s.SiteId,
                    sites.SiteName,
                    g.GoodsId,
                    g.GoodsName,
                    g.BId,
                    bc.ClassName,
                    g.MId,
                    mc.ClassName,
                    g.SId,
                    sc.ClassName,
                    w.WarehouseId,
                    w.WarehouseName,
                    g.SafeQty";

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY g.GoodsId, s.SiteId";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSWC10ReportItem>(sql, parameters);

            return new PagedResult<SYSWC10ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢庫存分析報表失敗", ex);
            throw;
        }
    }

    private async Task<PagedResult<Dictionary<string, object>>> GetSYSA1012ReportAsync(AnalysisReportQuery query)
    {
        // SYSA1012: 耗材進銷存月報表
        var sql = @"
            SELECT 
                g.GoodsId,
                g.GoodsName,
                ISNULL(@SiteId, '') AS SiteId,
                ISNULL(@YearMonth, '') AS ReportMonth,
                ISNULL(beginQty.BeginQty, 0) AS BeginQty,
                ISNULL(beginQty.BeginAmt, 0) AS BeginAmt,
                ISNULL(inQty.InQty, 0) AS InQty,
                ISNULL(inQty.InAmt, 0) AS InAmt,
                ISNULL(outQty.OutQty, 0) AS OutQty,
                ISNULL(outQty.OutAmt, 0) AS OutAmt,
                ISNULL(endQty.EndQty, 0) AS EndQty,
                ISNULL(endQty.EndAmt, 0) AS EndAmt
            FROM Goods g
            LEFT JOIN (
                SELECT GoodsId, SUM(Qty) AS BeginQty, SUM(StocksNtaxAmt) AS BeginAmt
                FROM InventoryStocks
                WHERE StocksDate < @BeginDate AND (@SiteId IS NULL OR SiteId = @SiteId)
                GROUP BY GoodsId
            ) beginQty ON g.GoodsId = beginQty.GoodsId
            LEFT JOIN (
                SELECT GoodsId, SUM(Qty) AS InQty, SUM(StocksNtaxAmt) AS InAmt
                FROM InventoryStocks
                WHERE StocksStatus = '1' AND StocksDate >= @BeginDate AND StocksDate <= @EndDate
                    AND (@SiteId IS NULL OR SiteId = @SiteId)
                GROUP BY GoodsId
            ) inQty ON g.GoodsId = inQty.GoodsId
            LEFT JOIN (
                SELECT GoodsId, SUM(Qty) AS OutQty, SUM(StocksNtaxAmt) AS OutAmt
                FROM InventoryStocks
                WHERE StocksStatus IN ('2', '3', '4', '5', '6') AND StocksDate >= @BeginDate AND StocksDate <= @EndDate
                    AND (@SiteId IS NULL OR SiteId = @SiteId)
                GROUP BY GoodsId
            ) outQty ON g.GoodsId = outQty.GoodsId
            LEFT JOIN (
                SELECT GoodsId, SUM(Qty) AS EndQty, SUM(StocksNtaxAmt) AS EndAmt
                FROM InventoryStocks
                WHERE StocksDate <= @EndDate AND (@SiteId IS NULL OR SiteId = @SiteId)
                GROUP BY GoodsId
            ) endQty ON g.GoodsId = endQty.GoodsId
            WHERE 1=1";

        var parameters = new DynamicParameters();
        parameters.Add("SiteId", query.SiteId);
        parameters.Add("YearMonth", query.YearMonth);
        
        if (!string.IsNullOrEmpty(query.BeginDate))
        {
            parameters.Add("BeginDate", DateTime.Parse(query.BeginDate));
        }
        else
        {
            parameters.Add("BeginDate", DateTime.Now.AddMonths(-1));
        }

        if (!string.IsNullOrEmpty(query.EndDate))
        {
            parameters.Add("EndDate", DateTime.Parse(query.EndDate));
        }
        else
        {
            parameters.Add("EndDate", DateTime.Now);
        }

        if (!string.IsNullOrEmpty(query.BId))
        {
            sql += " AND g.BId = @BId";
            parameters.Add("BId", query.BId);
        }

        if (!string.IsNullOrEmpty(query.MId))
        {
            sql += " AND g.MId = @MId";
            parameters.Add("MId", query.MId);
        }

        if (!string.IsNullOrEmpty(query.SId))
        {
            sql += " AND g.SId = @SId";
            parameters.Add("SId", query.SId);
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            sql += " AND g.GoodsId LIKE @GoodsId";
            parameters.Add("GoodsId", $"%{query.GoodsId}%");
        }

        // 分頁
        var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
        var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

        sql += " ORDER BY g.GoodsId";
        sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync(sql, parameters);
        
        var items = rows.Select(row =>
        {
            var dict = new Dictionary<string, object>();
            var rowDict = (IDictionary<string, object>)row;
            foreach (var kvp in rowDict)
            {
                dict[kvp.Key] = kvp.Value ?? string.Empty;
            }
            return dict;
        }).ToList();

        return new PagedResult<Dictionary<string, object>>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<PagedResult<SalesAnalysisReportItem>> GetSalesAnalysisReportAsync(SalesAnalysisQuery query)
    {
        try
        {
            _logger.LogInfo($"查詢銷售分析報表 - SiteId: {query.SiteId}, DateFrom: {query.DateFrom}, DateTo: {query.DateTo}");

            // 構建 SQL 查詢
            var sql = @"
                SELECT 
                    d.GoodsId AS ProductId,
                    ISNULL(g.GoodsName, '') AS ProductName,
                    ISNULL(g.BId, '') AS BigClassId,
                    ISNULL(bc.BName, '') AS BigClassName,
                    ISNULL(g.MId, '') AS MidClassId,
                    ISNULL(mc.MName, '') AS MidClassName,
                    ISNULL(g.SId, '') AS SmallClassId,
                    ISNULL(sc.SName, '') AS SmallClassName,
                    ISNULL(g.VendorId, '') AS VendorId,
                    ISNULL(v.VendorName, '') AS VendorName,
                    ISNULL(o.ShopId, '') AS SiteId,
                    ISNULL(s.SiteName, '') AS SiteName,
                    ISNULL(o.ApplyUserId, '') AS SalesPersonId,
                    ISNULL(u.UserName, '') AS SalesPersonName,
                    SUM(ISNULL(d.OrderQty, 0)) AS TotalQuantity,
                    SUM(ISNULL(d.Amount, 0)) AS TotalAmount,
                    SUM(ISNULL(d.Amount, 0) * 0.8) AS TotalCost, -- 假設成本為金額的80%，實際應從成本表查詢
                    SUM(ISNULL(d.Amount, 0) * 0.2) AS TotalProfit, -- 假設毛利為金額的20%
                    CASE 
                        WHEN SUM(ISNULL(d.Amount, 0)) > 0 
                        THEN (SUM(ISNULL(d.Amount, 0) * 0.2) / SUM(ISNULL(d.Amount, 0))) * 100
                        ELSE 0 
                    END AS ProfitRate,
                    COUNT(DISTINCT o.OrderId) AS OrderCount,
                    CASE 
                        WHEN SUM(ISNULL(d.OrderQty, 0)) > 0 
                        THEN SUM(ISNULL(d.Amount, 0)) / SUM(ISNULL(d.OrderQty, 0))
                        ELSE 0 
                    END AS AvgUnitPrice,
                    CASE 
                        WHEN COUNT(DISTINCT o.OrderId) > 0 
                        THEN SUM(ISNULL(d.OrderQty, 0)) / COUNT(DISTINCT o.OrderId)
                        ELSE 0 
                    END AS AvgQuantity
                FROM SalesOrders o
                INNER JOIN SalesOrderDetails d ON o.OrderId = d.OrderId
                LEFT JOIN Goods g ON d.GoodsId = g.GoodsId
                LEFT JOIN Sites s ON o.ShopId = s.SiteId
                LEFT JOIN Vendors v ON g.VendorId = v.VendorId
                LEFT JOIN Users u ON o.ApplyUserId = u.UserId
                LEFT JOIN Classify bc ON g.BId = bc.BId AND bc.ClassifyType = 'B'
                LEFT JOIN Classify mc ON g.MId = mc.MId AND mc.ClassifyType = 'M'
                LEFT JOIN Classify sc ON g.SId = sc.SId AND sc.ClassifyType = 'S'
                WHERE o.Status != 'X' -- 排除已取消的訂單
            ";

            var parameters = new DynamicParameters();
            var conditions = new List<string>();

            // 添加查詢條件
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                conditions.Add("o.ShopId = @SiteId");
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.DateFrom))
            {
                conditions.Add("o.OrderDate >= @DateFrom");
                parameters.Add("DateFrom", DateTime.Parse(query.DateFrom));
            }

            if (!string.IsNullOrEmpty(query.DateTo))
            {
                conditions.Add("o.OrderDate <= @DateTo");
                parameters.Add("DateTo", DateTime.Parse(query.DateTo).AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrEmpty(query.BigClassId))
            {
                conditions.Add("g.BId = @BigClassId");
                parameters.Add("BigClassId", query.BigClassId);
            }

            if (!string.IsNullOrEmpty(query.MidClassId))
            {
                conditions.Add("g.MId = @MidClassId");
                parameters.Add("MidClassId", query.MidClassId);
            }

            if (!string.IsNullOrEmpty(query.SmallClassId))
            {
                conditions.Add("g.SId = @SmallClassId");
                parameters.Add("SmallClassId", query.SmallClassId);
            }

            if (!string.IsNullOrEmpty(query.ProductId))
            {
                conditions.Add("d.GoodsId = @ProductId");
                parameters.Add("ProductId", query.ProductId);
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                conditions.Add("g.VendorId = @VendorId");
                parameters.Add("VendorId", query.VendorId);
            }

            if (!string.IsNullOrEmpty(query.SalesPersonId))
            {
                conditions.Add("o.ApplyUserId = @SalesPersonId");
                parameters.Add("SalesPersonId", query.SalesPersonId);
            }

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                conditions.Add("o.CustomerId = @CustomerId");
                parameters.Add("CustomerId", query.CustomerId);
            }

            if (conditions.Count > 0)
            {
                sql += " AND " + string.Join(" AND ", conditions);
            }

            // 根據群組方式添加 GROUP BY
            var groupByFields = new List<string>();
            if (query.GroupBy == "product")
            {
                groupByFields.AddRange(new[] { "d.GoodsId", "g.GoodsName", "g.BId", "bc.BName", "g.MId", "mc.MName", "g.SId", "sc.SName", "g.VendorId", "v.VendorName" });
            }
            else if (query.GroupBy == "category")
            {
                groupByFields.AddRange(new[] { "g.BId", "bc.BName", "g.MId", "mc.MName", "g.SId", "sc.SName" });
            }
            else if (query.GroupBy == "site")
            {
                groupByFields.AddRange(new[] { "o.ShopId", "s.SiteName" });
            }
            else if (query.GroupBy == "vendor")
            {
                groupByFields.AddRange(new[] { "g.VendorId", "v.VendorName" });
            }
            else if (query.GroupBy == "salesperson")
            {
                groupByFields.AddRange(new[] { "o.ApplyUserId", "u.UserName" });
            }
            else
            {
                // 預設按商品分組
                groupByFields.AddRange(new[] { "d.GoodsId", "g.GoodsName", "g.BId", "bc.BName", "g.MId", "mc.MName", "g.SId", "sc.SName", "g.VendorId", "v.VendorName", "o.ShopId", "s.SiteName", "o.ApplyUserId", "u.UserName" });
            }

            sql += " GROUP BY " + string.Join(", ", groupByFields);

            // 查詢總數
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 添加分頁
            sql += " ORDER BY TotalAmount DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<SalesAnalysisReportItem>(sql, parameters);

            return new PagedResult<SalesAnalysisReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售分析報表失敗", ex);
            throw;
        }
    }

    public async Task<SalesAnalysisSummaryItem> GetSalesAnalysisSummaryAsync(SalesAnalysisQuery query)
    {
        try
        {
            _logger.LogInfo($"查詢銷售分析報表彙總 - SiteId: {query.SiteId}, DateFrom: {query.DateFrom}, DateTo: {query.DateTo}");

            // 構建 SQL 查詢（不包含 GROUP BY，直接彙總）
            var sql = @"
                SELECT 
                    SUM(ISNULL(d.OrderQty, 0)) AS TotalQuantity,
                    SUM(ISNULL(d.Amount, 0)) AS TotalAmount,
                    SUM(ISNULL(d.Amount, 0) * 0.8) AS TotalCost, -- 假設成本為金額的80%，實際應從成本表查詢
                    SUM(ISNULL(d.Amount, 0) * 0.2) AS TotalProfit, -- 假設毛利為金額的20%
                    CASE 
                        WHEN SUM(ISNULL(d.Amount, 0)) > 0 
                        THEN (SUM(ISNULL(d.Amount, 0) * 0.2) / SUM(ISNULL(d.Amount, 0))) * 100
                        ELSE 0 
                    END AS AvgProfitRate,
                    COUNT(DISTINCT o.OrderId) AS TotalOrderCount
                FROM SalesOrders o
                INNER JOIN SalesOrderDetails d ON o.OrderId = d.OrderId
                LEFT JOIN Goods g ON d.GoodsId = g.GoodsId
                LEFT JOIN Sites s ON o.ShopId = s.SiteId
                LEFT JOIN Vendors v ON g.VendorId = v.VendorId
                LEFT JOIN Users u ON o.ApplyUserId = u.UserId
                LEFT JOIN Classify bc ON g.BId = bc.BId AND bc.ClassifyType = 'B'
                LEFT JOIN Classify mc ON g.MId = mc.MId AND mc.ClassifyType = 'M'
                LEFT JOIN Classify sc ON g.SId = sc.SId AND sc.ClassifyType = 'S'
                WHERE o.Status != 'X' -- 排除已取消的訂單
            ";

            var parameters = new DynamicParameters();
            var conditions = new List<string>();

            // 添加查詢條件（與 GetSalesAnalysisReportAsync 相同）
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                conditions.Add("o.ShopId = @SiteId");
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.DateFrom))
            {
                conditions.Add("o.OrderDate >= @DateFrom");
                parameters.Add("DateFrom", DateTime.Parse(query.DateFrom));
            }

            if (!string.IsNullOrEmpty(query.DateTo))
            {
                conditions.Add("o.OrderDate <= @DateTo");
                parameters.Add("DateTo", DateTime.Parse(query.DateTo).AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrEmpty(query.BigClassId))
            {
                conditions.Add("g.BId = @BigClassId");
                parameters.Add("BigClassId", query.BigClassId);
            }

            if (!string.IsNullOrEmpty(query.MidClassId))
            {
                conditions.Add("g.MId = @MidClassId");
                parameters.Add("MidClassId", query.MidClassId);
            }

            if (!string.IsNullOrEmpty(query.SmallClassId))
            {
                conditions.Add("g.SId = @SmallClassId");
                parameters.Add("SmallClassId", query.SmallClassId);
            }

            if (!string.IsNullOrEmpty(query.ProductId))
            {
                conditions.Add("d.GoodsId = @ProductId");
                parameters.Add("ProductId", query.ProductId);
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                conditions.Add("g.VendorId = @VendorId");
                parameters.Add("VendorId", query.VendorId);
            }

            if (!string.IsNullOrEmpty(query.SalesPersonId))
            {
                conditions.Add("o.ApplyUserId = @SalesPersonId");
                parameters.Add("SalesPersonId", query.SalesPersonId);
            }

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                conditions.Add("o.CustomerId = @CustomerId");
                parameters.Add("CustomerId", query.CustomerId);
            }

            if (conditions.Count > 0)
            {
                sql += " AND " + string.Join(" AND ", conditions);
            }

            var summary = await QuerySingleAsync<SalesAnalysisSummaryItem>(sql, parameters);
            return summary ?? new SalesAnalysisSummaryItem();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售分析報表彙總失敗", ex);
            throw;
        }
    }
}
