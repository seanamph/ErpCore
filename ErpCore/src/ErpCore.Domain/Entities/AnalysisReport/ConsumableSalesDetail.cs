namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 耗材出售單明細檔 (SYSA297)
/// </summary>
public class ConsumableSalesDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; }

    /// <summary>
    /// 交易單號
    /// </summary>
    public string TxnNo { get; set; } = string.Empty;

    /// <summary>
    /// 序號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 耗材編號
    /// </summary>
    public string ConsumableId { get; set; } = string.Empty;

    /// <summary>
    /// 耗材名稱
    /// </summary>
    public string? ConsumableName { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 稅別 (1:應稅, 0:免稅)
    /// </summary>
    public string Tax { get; set; } = "1";

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 未稅金額
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// 採購驗收狀態 (1:已驗收)
    /// </summary>
    public string PurchaseStatus { get; set; } = "1";

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
}
