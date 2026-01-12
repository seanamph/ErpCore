namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 耗材出售單主檔 (SYSA297)
/// </summary>
public class ConsumableSales
{
    /// <summary>
    /// 交易單號
    /// </summary>
    public string TxnNo { get; set; } = string.Empty;

    /// <summary>
    /// 唯一識別碼
    /// </summary>
    public Guid Rrn { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 出售日期
    /// </summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// 狀態 (1:待審核, 2:已審核, 3:已取消)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 未稅金額
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// 申請數量
    /// </summary>
    public int ApplyCount { get; set; }

    /// <summary>
    /// 明細數量
    /// </summary>
    public int DetailCount { get; set; }

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
    /// 審核者
    /// </summary>
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// 審核時間
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// 明細列表
    /// </summary>
    public List<ConsumableSalesDetail> Details { get; set; } = new();
}
