namespace ErpCore.Domain.Entities.Purchase;

/// <summary>
/// 採購單主檔
/// </summary>
public class PurchaseOrder
{
    /// <summary>
    /// 採購單號
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 採購日期
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 單據類型 (PO:採購, RT:退貨)
    /// </summary>
    public string OrderType { get; set; } = string.Empty;

    /// <summary>
    /// 分店代碼
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 供應商代碼
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (D:草稿, S:已送出, A:已審核, X:已取消)
    /// </summary>
    public string Status { get; set; } = "D";

    /// <summary>
    /// 申請人員
    /// </summary>
    public string? ApplyUserId { get; set; }

    /// <summary>
    /// 申請日期
    /// </summary>
    public DateTime? ApplyDate { get; set; }

    /// <summary>
    /// 審核人員
    /// </summary>
    public string? ApproveUserId { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? ApproveDate { get; set; }

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
    /// 預期交貨日期
    /// </summary>
    public DateTime? ExpectedDate { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 來源程式 (SYSW315/SYSW316)
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

