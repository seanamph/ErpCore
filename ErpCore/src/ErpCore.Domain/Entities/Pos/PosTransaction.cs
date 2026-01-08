namespace ErpCore.Domain.Entities.Pos;

/// <summary>
/// POS交易主檔實體
/// </summary>
public class PosTransaction
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
    /// 店別代號
    /// </summary>
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// POS機號
    /// </summary>
    public string? PosId { get; set; }

    /// <summary>
    /// 交易日期時間
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// 交易類型 (Sale/Return/Refund)
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 付款方式
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// 狀態 (Pending/Synced/Failed)
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 同步時間
    /// </summary>
    public DateTime? SyncAt { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

