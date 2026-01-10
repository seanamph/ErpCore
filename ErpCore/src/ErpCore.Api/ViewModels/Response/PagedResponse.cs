namespace ErpCore.Api.ViewModels.Response;

/// <summary>
/// 分頁回應模型
/// </summary>
public class PagedResponse<T>
{
    /// <summary>
    /// 資料列表
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// 總筆數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼（從1開始）
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

