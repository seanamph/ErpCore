namespace ErpCore.Application.Queries.Products;

/// <summary>
/// 取得商品查詢
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class GetProductQuery
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public string ProductId { get; set; } = string.Empty;
}

