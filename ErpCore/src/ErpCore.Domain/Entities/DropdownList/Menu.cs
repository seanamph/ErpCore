namespace ErpCore.Domain.Entities.DropdownList;

/// <summary>
/// 選單資料實體 (MENU_LIST)
/// </summary>
public class Menu
{
    /// <summary>
    /// 選單ID
    /// </summary>
    public string MenuId { get; set; } = string.Empty;

    /// <summary>
    /// 選單名稱
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 系統ID
    /// </summary>
    public string? SystemId { get; set; }

    /// <summary>
    /// 父選單ID
    /// </summary>
    public string? ParentMenuId { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 圖示
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 連結網址
    /// </summary>
    public string? Url { get; set; }

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
}

