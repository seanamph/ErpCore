namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 業務報表列印資料實體 (SYSL150)
/// </summary>
public class BusinessReportPrint
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 發放年度
    /// </summary>
    public int GiveYear { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 員工編號
    /// </summary>
    public string EmpId { get; set; } = string.Empty;

    /// <summary>
    /// 員工姓名
    /// </summary>
    public string? EmpName { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public decimal? Qty { get; set; }

    /// <summary>
    /// 狀態 (P:待審核, A:已審核, R:已拒絕)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 審核者
    /// </summary>
    public string? Verifier { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? VerifyDate { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}
