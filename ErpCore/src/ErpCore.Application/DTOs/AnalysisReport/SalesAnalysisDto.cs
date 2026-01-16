using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 銷售分析報表 DTO
/// </summary>
public class SalesAnalysisReportDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? BigClassId { get; set; }
    public string? BigClassName { get; set; }
    public string? MidClassId { get; set; }
    public string? MidClassName { get; set; }
    public string? SmallClassId { get; set; }
    public string? SmallClassName { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string? SalesPersonId { get; set; }
    public string? SalesPersonName { get; set; }
    public decimal TotalQuantity { get; set; } // 總數量
    public decimal TotalAmount { get; set; } // 總金額
    public decimal TotalCost { get; set; } // 總成本
    public decimal TotalProfit { get; set; } // 總毛利
    public decimal ProfitRate { get; set; } // 毛利率
    public int OrderCount { get; set; } // 訂單筆數
    public decimal AvgUnitPrice { get; set; } // 平均單價
    public decimal AvgQuantity { get; set; } // 平均數量
}

/// <summary>
/// 銷售分析報表查詢 DTO
/// </summary>
public class SalesAnalysisQueryDto : PagedQuery
{
    public string? SiteId { get; set; } // 店別代碼
    public string? DateFrom { get; set; } // 日期起
    public string? DateTo { get; set; } // 日期迄
    public string? BigClassId { get; set; } // 大分類代碼
    public string? MidClassId { get; set; } // 中分類代碼
    public string? SmallClassId { get; set; } // 小分類代碼
    public string? ProductId { get; set; } // 商品代碼
    public string? VendorId { get; set; } // 廠商代碼
    public string? SalesPersonId { get; set; } // 銷售人員代碼
    public string? CustomerId { get; set; } // 客戶代碼
    public string? ReportType { get; set; } // 報表類型 (daily, monthly, yearly, custom)
    public string? GroupBy { get; set; } // 群組方式 (product, category, site, vendor, salesperson)
}

/// <summary>
/// 銷售分析報表彙總 DTO
/// </summary>
public class SalesAnalysisSummaryDto
{
    public decimal TotalQuantity { get; set; } // 總數量
    public decimal TotalAmount { get; set; } // 總金額
    public decimal TotalCost { get; set; } // 總成本
    public decimal TotalProfit { get; set; } // 總毛利
    public decimal AvgProfitRate { get; set; } // 平均毛利率
    public int TotalOrderCount { get; set; } // 總訂單筆數
}

/// <summary>
/// 銷售分析報表結果（包含分頁和彙總）
/// </summary>
public class SalesAnalysisReportResult
{
    public List<SalesAnalysisReportDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public SalesAnalysisSummaryDto? Summary { get; set; }
}
