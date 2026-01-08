namespace ErpCore.Domain.Entities.Transfer;

/// <summary>
/// 調撥驗退單主檔
/// </summary>
public class TransferReturn
{
    /// <summary>
    /// 驗退單號
    /// </summary>
    public string ReturnId { get; set; } = string.Empty;

    /// <summary>
    /// 調撥單號
    /// </summary>
    public string TransferId { get; set; } = string.Empty;

    /// <summary>
    /// 原驗收單號
    /// </summary>
    public string? ReceiptId { get; set; }

    /// <summary>
    /// 驗退日期
    /// </summary>
    public DateTime ReturnDate { get; set; }

    /// <summary>
    /// 調出分店代碼
    /// </summary>
    public string FromShopId { get; set; } = string.Empty;

    /// <summary>
    /// 調入分店代碼
    /// </summary>
    public string ToShopId { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (P:待驗退, R:部分驗退, C:已驗退, X:已取消)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 驗退人員
    /// </summary>
    public string? ReturnUserId { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 總數量
    /// </summary>
    public decimal TotalQty { get; set; }

    /// <summary>
    /// 驗退原因
    /// </summary>
    public string? ReturnReason { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 是否已日結
    /// </summary>
    public bool IsSettled { get; set; }

    /// <summary>
    /// 日結日期
    /// </summary>
    public DateTime? SettledDate { get; set; }

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

