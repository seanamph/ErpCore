namespace ErpCore.Api.ViewModels.Request;

/// <summary>
/// 通用請求模型
/// </summary>
public class CommonRequest
{
    /// <summary>
    /// 頁碼（從1開始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 排序欄位
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// 排序方向（asc/desc）
    /// </summary>
    public string? SortDirection { get; set; } = "asc";

    /// <summary>
    /// 搜尋關鍵字
    /// </summary>
    public string? SearchKeyword { get; set; }
}

