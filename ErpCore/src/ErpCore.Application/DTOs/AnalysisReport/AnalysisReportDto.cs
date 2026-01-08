namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 進銷存分析報表 DTO (SYSA1000)
/// </summary>
public class AnalysisReportDto
{
    /// <summary>
    /// 報表ID
    /// </summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// 店別名稱
    /// </summary>
    public string? SiteName { get; set; }

    /// <summary>
    /// 報表項目列表
    /// </summary>
    public List<AnalysisReportItemDto> Items { get; set; } = new();

    /// <summary>
    /// 總筆數
    /// </summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// 進銷存分析報表項目 DTO
/// </summary>
public class AnalysisReportItemDto
{
    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 大分類
    /// </summary>
    public string? BId { get; set; }

    /// <summary>
    /// 中分類
    /// </summary>
    public string? MId { get; set; }

    /// <summary>
    /// 小分類
    /// </summary>
    public string? SId { get; set; }

    /// <summary>
    /// 商品代碼
    /// </summary>
    public string? GoodsId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string? GoodsName { get; set; }

    /// <summary>
    /// 包裝單位
    /// </summary>
    public string? PackUnit { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public decimal? Qty { get; set; }

    /// <summary>
    /// 安全庫存量
    /// </summary>
    public decimal? SafeQty { get; set; }

    /// <summary>
    /// 選擇類型
    /// </summary>
    public string? SelectType { get; set; }

    /// <summary>
    /// 其他欄位（根據不同報表類型動態添加）
    /// </summary>
    public Dictionary<string, object>? AdditionalFields { get; set; }
}

/// <summary>
/// 匯出報表 DTO
/// </summary>
public class ExportAnalysisReportDto
{
    /// <summary>
    /// 匯出格式 (Excel, PDF)
    /// </summary>
    public string Format { get; set; } = "Excel";

    /// <summary>
    /// 查詢參數
    /// </summary>
    public AnalysisReportQueryDto QueryParams { get; set; } = new();
}

/// <summary>
/// 列印報表 DTO
/// </summary>
public class PrintAnalysisReportDto
{
    /// <summary>
    /// 列印格式 (PDF)
    /// </summary>
    public string Format { get; set; } = "PDF";

    /// <summary>
    /// 查詢參數
    /// </summary>
    public AnalysisReportQueryDto QueryParams { get; set; } = new();

    /// <summary>
    /// 是否包含頁首頁尾
    /// </summary>
    public bool IncludeHeaderFooter { get; set; } = true;

    /// <summary>
    /// 是否包含日期時間
    /// </summary>
    public bool IncludeDateTime { get; set; } = true;
}

