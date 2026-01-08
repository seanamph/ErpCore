namespace ErpCore.Domain.Entities.Transfer;

/// <summary>
/// 調撥短溢單主檔
/// </summary>
public class TransferShortage
{
    /// <summary>
    /// 短溢單號
    /// </summary>
    public string ShortageId { get; set; } = string.Empty;

    /// <summary>
    /// 調撥單號
    /// </summary>
    public string TransferId { get; set; } = string.Empty;

    /// <summary>
    /// 驗收單號
    /// </summary>
    public string? ReceiptId { get; set; }

    /// <summary>
    /// 短溢日期
    /// </summary>
    public DateTime ShortageDate { get; set; }

    /// <summary>
    /// 調出分店代碼
    /// </summary>
    public string FromShopId { get; set; } = string.Empty;

    /// <summary>
    /// 調入分店代碼
    /// </summary>
    public string ToShopId { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (P:待處理, A:已審核, C:已處理, X:已取消)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 處理方式 (ADJUST:調整庫存, PENDING:待處理, PROCESSED:已處理)
    /// </summary>
    public string? ProcessType { get; set; }

    /// <summary>
    /// 處理人員
    /// </summary>
    public string? ProcessUserId { get; set; }

    /// <summary>
    /// 處理日期
    /// </summary>
    public DateTime? ProcessDate { get; set; }

    /// <summary>
    /// 審核人員
    /// </summary>
    public string? ApproveUserId { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? ApproveDate { get; set; }

    /// <summary>
    /// 總短溢數量（正數為溢收，負數為短少）
    /// </summary>
    public decimal TotalShortageQty { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 短溢原因
    /// </summary>
    public string? ShortageReason { get; set; }

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

