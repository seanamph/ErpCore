namespace ErpCore.Domain.Entities.Purchase;

/// <summary>
/// 採購單明細
/// </summary>
public class PurchaseOrderDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; }

    /// <summary>
    /// 採購單號
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

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
    /// 單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 已收數量
    /// </summary>
    public decimal ReceivedQty { get; set; }

    /// <summary>
    /// 已退數量
    /// </summary>
    public decimal ReturnQty { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? UnitId { get; set; }

    /// <summary>
    /// 稅率
    /// </summary>
    public decimal? TaxRate { get; set; } = 0;

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal? TaxAmount { get; set; } = 0;

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

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

