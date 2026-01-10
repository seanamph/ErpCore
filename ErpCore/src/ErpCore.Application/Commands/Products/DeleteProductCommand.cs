namespace ErpCore.Application.Commands.Products;

/// <summary>
/// 刪除商品命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class DeleteProductCommand
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 更新者（執行刪除的使用者）
    /// </summary>
    public string? UpdatedBy { get; set; }
}

