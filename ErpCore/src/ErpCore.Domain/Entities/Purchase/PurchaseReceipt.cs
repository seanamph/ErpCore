namespace ErpCore.Domain.Entities.Purchase;

/// <summary>
/// 採購驗收單主檔
/// </summary>
public class PurchaseReceipt
{
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string ReceiptId { get; set; } = string.Empty;

    /// <summary>
    /// 採購單號
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 驗收日期
    /// </summary>
    public DateTime ReceiptDate { get; set; }

    /// <summary>
    /// 分店代碼
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 供應商代碼
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (P:待驗收, R:部分驗收, C:已驗收, X:已取消)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 驗收人員
    /// </summary>
    public string? ReceiptUserId { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 總數量
    /// </summary>
    public decimal TotalQty { get; set; }

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
    /// 單據類型 (1:採購, 2:退貨)
    /// </summary>
    public string PurchaseOrderType { get; set; } = "1";

    /// <summary>
    /// 是否為已日結調整
    /// </summary>
    public bool IsSettledAdjustment { get; set; }

    /// <summary>
    /// 原始驗收單號（如有）
    /// </summary>
    public string? OriginalReceiptId { get; set; }

    /// <summary>
    /// 調整原因
    /// </summary>
    public string? AdjustmentReason { get; set; }

    /// <summary>
    /// 來源程式 (SYSW324/SYSW336)
    /// </summary>
    public string? SourceProgram { get; set; }

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

