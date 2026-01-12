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
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                "SYSA1011" => await GetSYSA1011ReportAsync(new SYSA1011Query
                {
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    SiteId = query.SiteId,
                    BId = query.BId,
                    MId = query.MId,
                    SId = query.SId,
                    GoodsId = query.GoodsId,
                    FilterType = query.FilterType
                }).ContinueWith(t => ConvertToDictionary(t.Result)),
                "SYSA1012" => await GetSYSA1012ReportAsync(query),
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                        SUM(CASE WHEN po.OrderType = 'PO' THEN pod.Qty ELSE -pod.Qty END) AS PurchaseQty
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
                        SUM(CASE WHEN so.OrderType = 'SO' THEN sod.Qty ELSE -sod.Qty END) AS SalesQty
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
            // 查詢工務維修件數統計報表
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            parameters.Add("OrgId", query.OrgId);
            parameters.Add("YearMonth", query.YearMonth);

            // 組織篩選
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                whereClause += " AND mr.OrgId = @OrgId";
            }

            // 年月篩選 (YYYY-MM 格式)
            if (!string.IsNullOrEmpty(query.YearMonth))
            {
                whereClause += " AND FORMAT(mr.MaintenanceDate, 'yyyy-MM') = @YearMonth";
            }

            // 篩選類型（維修狀態）
            if (!string.IsNullOrEmpty(query.FilterType) && query.FilterType != "全部")
            {
                var statusMap = new Dictionary<string, string>
                {
                    { "pending", "待處理" },
                    { "processing", "處理中" },
                    { "completed", "已完成" },
                    { "cancelled", "已取消" }
                };

                if (statusMap.ContainsKey(query.FilterType))
                {
                    whereClause += " AND mr.MaintenanceStatus = @FilterType";
                    parameters.Add("FilterType", statusMap[query.FilterType]);
                }
            }

            // 查詢 SQL
            var sql = $@"
                SELECT 
                    mr.OrgId,
                    ISNULL(org.OrgName, '') AS OrgName,
                    '工務維修件數統計表' AS ReportName,
                    ISNULL(@YearMonth, FORMAT(mr.MaintenanceDate, 'yyyy-MM')) AS YearMonth,
                    mr.MaintenanceType,
                    mr.MaintenanceStatus,
                    SUM(mr.ItemCount) AS ItemCount,
                    0 AS TotalCount
                FROM MaintenanceRecords mr
                LEFT JOIN Organizations org ON mr.OrgId = org.OrgId
                {whereClause}
                GROUP BY mr.OrgId, org.OrgName, FORMAT(mr.MaintenanceDate, 'yyyy-MM'), mr.MaintenanceType, mr.MaintenanceStatus";

            // 計算總數
            var countSql = $@"
                SELECT COUNT(*)
                FROM (
                    SELECT mr.OrgId, FORMAT(mr.MaintenanceDate, 'yyyy-MM') AS YearMonth, mr.MaintenanceType, mr.MaintenanceStatus
                    FROM MaintenanceRecords mr
                    {whereClause}
                    GROUP BY mr.OrgId, FORMAT(mr.MaintenanceDate, 'yyyy-MM'), mr.MaintenanceType, mr.MaintenanceStatus
                ) AS t";
            
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 計算總件數
            var totalItemCountSql = $@"
                SELECT ISNULL(SUM(mr.ItemCount), 0)
                FROM MaintenanceRecords mr
                {whereClause}";
            var totalItemCount = await ExecuteScalarAsync<int>(totalItemCountSql, parameters);

            // 分頁
            sql += " ORDER BY mr.OrgId, FORMAT(mr.MaintenanceDate, 'yyyy-MM'), mr.MaintenanceType, mr.MaintenanceStatus";
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
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
                    @ReportType AS ReportType,
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
                    wm.BelongStatus,
                    wm.BelongOrg,
                    wm.MaintainEmp,
                    wm.ApplyType";

            // 分頁
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            sql += " ORDER BY wm.SiteId, wm.BelongStatus";
            sql += $" OFFSET {(query.PageIndex - 1) * query.PageSize} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";

            var items = await QueryAsync<SYSA1023ReportItem>(sql, parameters);

            return new PagedResult<SYSA1023ReportItem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修統計報表(報表類型)失敗", ex);
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
}
