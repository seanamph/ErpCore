namespace ErpCore.Shared.Common;

/// <summary>
/// 分頁查詢基類
/// </summary>
public class PagedQuery
{
    /// <summary>
    /// 頁碼（從1開始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序欄位
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向 (ASC/DESC)
    /// </summary>
    public string? SortOrder { get; set; }
}
