namespace ErpCore.Domain.Entities.Pos;

/// <summary>
/// POS交易明細實體
/// </summary>
public class PosTransactionDetail
{
    /// <summary>
    /// 主鍵ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 交易編號
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNo { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 折扣
    /// </summary>
    public decimal? Discount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

