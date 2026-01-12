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
