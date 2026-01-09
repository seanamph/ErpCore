namespace ErpCore.Domain.Entities.StandardModule;

/// <summary>
/// STD5000 交易明細實體 (SYS5310-SYS53C6 - 交易明細管理)
/// </summary>
public class Std5000TransactionDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 交易單號
    /// </summary>
    public string TransId { get; set; } = string.Empty;

    /// <summary>
    /// 序號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

