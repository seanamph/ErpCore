namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 子系統項目資料實體 (SYS0420)
/// </summary>
public class Menu
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 子系統項目代碼
    /// </summary>
    public string MenuId { get; set; } = string.Empty;

    /// <summary>
    /// 子系統項目名稱
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 主系統代碼
    /// </summary>
    public string SystemId { get; set; } = string.Empty;

    /// <summary>
    /// 上層子系統代碼
    /// </summary>
    public string? ParentMenuId { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

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
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}
