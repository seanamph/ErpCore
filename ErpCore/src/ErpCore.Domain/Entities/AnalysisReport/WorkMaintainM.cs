namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 工務維修主檔 (對應舊系統 IMS_AM.NAM_WORK_MAINTAINM)
/// </summary>
public class WorkMaintainM
{
    /// <summary>
    /// 交易單號
    /// </summary>
    public string TxnNo { get; set; } = string.Empty;

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 申請單位
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 申請日期
    /// </summary>
    public DateTime? ApplyDate { get; set; }

    /// <summary>
    /// 請修類別 (多選，以分號分隔)
    /// </summary>
    public string? ApplyType { get; set; }

    /// <summary>
    /// 維保人員 (多選，以分號分隔)
    /// </summary>
    public string? MaintainEmp { get; set; }

    /// <summary>
    /// 歸屬狀態 (1:員工負擔, 2:店別負擔)
    /// </summary>
    public string? BelongStatus { get; set; }

    /// <summary>
    /// 費用歸屬單位
    /// </summary>
    public string? BelongOrg { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
