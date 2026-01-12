namespace ErpCore.Domain.Entities.Transfer;

/// <summary>
/// 調撥驗退單明細
/// </summary>
public class TransferReturnDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; }

    /// <summary>
    /// 驗退單號
    /// </summary>
    public string ReturnId { get; set; } = string.Empty;

    /// <summary>
    /// 調撥單明細ID
    /// </summary>
    public Guid? TransferDetailId { get; set; }

    /// <summary>
    /// 原驗收單明細ID
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
    /// 原驗收數量
    /// </summary>
    public decimal ReceiptQty { get; set; }

    /// <summary>
    /// 驗退數量
    /// </summary>
    public decimal ReturnQty { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 驗退原因
    /// </summary>
    public string? ReturnReason { get; set; }

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
