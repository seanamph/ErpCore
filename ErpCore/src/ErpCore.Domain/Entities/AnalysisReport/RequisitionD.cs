namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 領料單明細 (對應舊系統 IMS_AM.NAM_REQUISITIOND)
/// </summary>
public class RequisitionD
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
    /// 用途
    /// </summary>
    public string? Use { get; set; }

    /// <summary>
    /// 申請數量
    /// </summary>
    public decimal? MapplyQty { get; set; }

    /// <summary>
    /// 實際數量
    /// </summary>
    public decimal Qty { get; set; }
}
