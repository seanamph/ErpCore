namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 進銷存分析報表查詢 DTO (SYSA1000)
/// </summary>
public class AnalysisReportQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    
    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 年月 (YYYY/MM)
    /// </summary>
    public string? YearMonth { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? BeginDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 大分類代碼
    /// </summary>
    public string? BId { get; set; }

    /// <summary>
    /// 中分類代碼
    /// </summary>
    public string? MId { get; set; }

    /// <summary>
    /// 小分類代碼
    /// </summary>
    public string? SId { get; set; }

    /// <summary>
    /// 商品代碼
    /// </summary>
    public string? GoodsId { get; set; }

    /// <summary>
    /// 篩選類型
    /// </summary>
    public string? FilterType { get; set; }

    /// <summary>
    /// 單位代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 供應商
    /// </summary>
    public string? Vendor { get; set; }

    /// <summary>
    /// 用途
    /// </summary>
    public string? Use { get; set; }

    /// <summary>
    /// 歸屬狀態
    /// </summary>
    public string? BelongStatus { get; set; }

    /// <summary>
    /// 申請開始日期
    /// </summary>
    public DateTime? ApplyDateB { get; set; }

    /// <summary>
    /// 申請結束日期
    /// </summary>
    public DateTime? ApplyDateE { get; set; }

    /// <summary>
    /// 開始月份
    /// </summary>
    public string? StartMonth { get; set; }

    /// <summary>
    /// 結束月份
    /// </summary>
    public string? EndMonth { get; set; }

    /// <summary>
    /// 日期類型
    /// </summary>
    public string? DateType { get; set; }

    /// <summary>
    /// 維保人員
    /// </summary>
    public string? MaintainEmp { get; set; }

    /// <summary>
    /// 費用歸屬單位
    /// </summary>
    public string? BelongOrg { get; set; }

    /// <summary>
    /// 請修類別
    /// </summary>
    public string? ApplyType { get; set; }
}

