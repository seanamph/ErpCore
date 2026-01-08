namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 項目對應實體 (SYS0360)
/// </summary>
public class ItemCorrespond
{
    /// <summary>
    /// 項目代碼
    /// </summary>
    public string ItemId { get; set; } = string.Empty;

    /// <summary>
    /// 項目名稱
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// 項目類型
    /// </summary>
    public string? ItemType { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

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

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

