namespace ErpCore.Domain.ValueObjects;

/// <summary>
/// 金額值物件
/// </summary>
public class Money
{
    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string Currency { get; set; } = "TWD";

    /// <summary>
    /// 建構函式
    /// </summary>
    public Money(decimal amount, string currency = "TWD")
    {
        Amount = amount;
        Currency = currency;
    }
}

