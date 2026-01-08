namespace ErpCore.Domain.Entities.Purchase;

/// <summary>
/// 採購驗收單明細
/// </summary>
public class PurchaseReceiptDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; }

    /// <summary>
    /// 驗收單號
    /// </summary>
    public string ReceiptId { get; set; } = string.Empty;

    /// <summary>
    /// 採購單明細ID
    /// </summary>
    public Guid? OrderDetailId { get; set; }

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
    /// 訂購數量
    /// </summary>
    public decimal OrderQty { get; set; }

    /// <summary>
    /// 驗收數量
    /// </summary>
    public decimal ReceiptQty { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 原始驗收數量（已日結調整用）
    /// </summary>
    public decimal? OriginalReceiptQty { get; set; }

    /// <summary>
    /// 調整數量（已日結調整用）
    /// </summary>
    public decimal? AdjustmentQty { get; set; }

    /// <summary>
    /// 原始單價（已日結調整用）
    /// </summary>
    public decimal? OriginalUnitPrice { get; set; }

    /// <summary>
    /// 調整單價（已日結調整用）
    /// </summary>
    public decimal? AdjustmentPrice { get; set; }

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

