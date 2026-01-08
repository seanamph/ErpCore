namespace ErpCore.Domain.Entities.Customer;

/// <summary>
/// 客戶交易記錄實體 (CUS5120)
/// </summary>
public class CustomerTransaction
{
    /// <summary>
    /// 交易記錄ID
    /// </summary>
    public Guid TransactionId { get; set; }

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 交易日期
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// 交易序號
    /// </summary>
    public string TransactionNo { get; set; } = string.Empty;

    /// <summary>
    /// 交易類型 (SALE, RETURN, ADJUST)
    /// </summary>
    public string? TransactionType { get; set; }

    /// <summary>
    /// 交易金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

