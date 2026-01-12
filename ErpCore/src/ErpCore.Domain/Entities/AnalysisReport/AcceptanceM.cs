namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 驗收單主檔 (對應舊系統 IMS_AM.NAM_ACCEPTANCEM)
/// </summary>
public class AcceptanceM
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
    /// 供應商代碼
    /// </summary>
    public string? SupplierId { get; set; }

    /// <summary>
    /// 供應商名稱
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// 驗收日期
    /// </summary>
    public DateTime? AcceptanceDate { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
