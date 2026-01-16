namespace ErpCore.Domain.Entities.Transfer;

/// <summary>
/// 調撥短溢單明細 (SYSW384)
/// </summary>
public class TransferShortageDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; }

    /// <summary>
    /// 短溢單號
    /// </summary>
    public string ShortageId { get; set; } = string.Empty;

    /// <summary>
    /// 調撥單明細ID
    /// </summary>
    public Guid? TransferDetailId { get; set; }

    /// <summary>
    /// 驗收單明細ID
    /// </summary>
    public Guid? ReceiptDetailId { get; set; }

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 條碼編號
    /// </summary>
    public string? BarcodeId { get; set; }

    /// <summary>
    /// 調撥數量
    /// </summary>
    public decimal TransferQty { get; set; }

    /// <summary>
    /// 驗收數量
    /// </summary>
    public decimal ReceiptQty { get; set; }

    /// <summary>
    /// 短溢數量（正數為溢收，負數為短少）
    /// </summary>
    public decimal ShortageQty { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 短溢原因
    /// </summary>
    public string? ShortageReason { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
