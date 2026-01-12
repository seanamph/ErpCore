namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 驗收單明細 (對應舊系統 IMS_AM.NAM_ACCEPTANCED)
/// </summary>
public class AcceptanceD
{
    /// <summary>
    /// 交易單號
    /// </summary>
    public string TxnNo { get; set; } = string.Empty;

    /// <summary>
    /// 商品代碼
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 數量
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amt { get; set; }
}
