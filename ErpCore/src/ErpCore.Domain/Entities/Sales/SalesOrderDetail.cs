namespace ErpCore.Domain.Entities.Sales;

/// <summary>
/// 銷售單明細 (SYSD110-SYSD140)
/// </summary>
public class SalesOrderDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 銷售單號
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
    public decimal OrderQty { get; set; } = 0;

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 已出貨數量
    /// </summary>
    public decimal? ShippedQty { get; set; } = 0;

    /// <summary>
    /// 已退數量
    /// </summary>
    public decimal? ReturnQty { get; set; } = 0;

    /// <summary>
    /// 單位
    /// </summary>
    public string? UnitId { get; set; }

    /// <summary>
    /// 折扣率
    /// </summary>
    public decimal? DiscountRate { get; set; } = 0;

    /// <summary>
    /// 折扣金額
    /// </summary>
    public decimal? DiscountAmount { get; set; } = 0;

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
    public DateTime UpdatedAt { get; set; }
}

