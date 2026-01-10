namespace ErpCore.Application.Commands.Products;

/// <summary>
/// 更新商品命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class UpdateProductCommand
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
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }
}

