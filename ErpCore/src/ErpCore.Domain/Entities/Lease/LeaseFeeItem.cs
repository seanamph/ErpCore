namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 費用項目主檔 (SYSE310-SYSE430)
/// </summary>
public class LeaseFeeItem
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 費用項目編號
    /// </summary>
    public string FeeItemId { get; set; } = string.Empty;

    /// <summary>
    /// 費用項目名稱
    /// </summary>
    public string FeeItemName { get; set; } = string.Empty;

    /// <summary>
    /// 費用類型
    /// </summary>
    public string FeeType { get; set; } = string.Empty;

    /// <summary>
    /// 預設金額
    /// </summary>
    public decimal DefaultAmount { get; set; } = 0;

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

