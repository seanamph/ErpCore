namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 領料單主檔 (對應舊系統 IMS_AM.NAM_REQUISITIONM)
/// </summary>
public class RequisitionM
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
    /// 單位代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 單位分配
    /// </summary>
    public string? OrgAllocation { get; set; }

    /// <summary>
    /// 領料日期
    /// </summary>
    public DateTime? RequisitionDate { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
