namespace ErpCore.Application.Queries.Products;

/// <summary>
/// 取得商品列表查詢
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class GetProductsQuery
{
    /// <summary>
    /// 頁碼
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
    /// 排序方向
    /// </summary>
    public string SortOrder { get; set; } = "ASC";

    /// <summary>
    /// 商品編號（篩選）
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// 商品名稱（篩選）
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 商品分類（篩選）
    /// </summary>
    public string? CategoryId { get; set; }
}

