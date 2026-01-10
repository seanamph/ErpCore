namespace ErpCore.Application.Commands.Products;

/// <summary>
/// 建立商品命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class CreateProductCommand
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品分類
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }
}

