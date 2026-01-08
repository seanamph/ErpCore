namespace ErpCore.Domain.Entities.StockAdjustment;

/// <summary>
/// 庫存調整單主檔
/// </summary>
public class InventoryAdjustment
{
    /// <summary>
    /// 調整單號
    /// </summary>
    public string AdjustmentId { get; set; } = string.Empty;

    /// <summary>
    /// 調整日期
    /// </summary>
    public DateTime AdjustmentDate { get; set; }

    /// <summary>
    /// 分店代碼
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (D:草稿, C:已確認, X:已取消)
    /// </summary>
    public string Status { get; set; } = "D";

    /// <summary>
    /// 調整類型
    /// </summary>
    public string? AdjustmentType { get; set; }

    /// <summary>
    /// 調整人員
    /// </summary>
    public string? AdjustmentUser { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 備註2
    /// </summary>
    public string? Memo2 { get; set; }

    /// <summary>
    /// 來源單號
    /// </summary>
    public string? SourceNo { get; set; }

    /// <summary>
    /// 來源序號
    /// </summary>
    public string? SourceNum { get; set; }

    /// <summary>
    /// 來源檢查日期
    /// </summary>
    public DateTime? SourceCheckDate { get; set; }

    /// <summary>
    /// 來源供應商
    /// </summary>
    public string? SourceSuppId { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 總調整數量
    /// </summary>
    public decimal TotalQty { get; set; }

    /// <summary>
    /// 總調整成本
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// 總調整金額
    /// </summary>
    public decimal TotalAmount { get; set; }

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

