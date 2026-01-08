using ErpCore.Application.DTOs.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 商品服務介面
/// </summary>
public interface IProductService
{
    /// <summary>
    /// 查詢商品列表
    /// </summary>
    Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryDto query);

    /// <summary>
    /// 根據商品編號查詢商品資訊
    /// </summary>
    Task<ProductDto?> GetProductByIdAsync(string goodsId);

    /// <summary>
    /// 根據商品編號查詢商品名稱（簡化版，用於快速查詢）
    /// </summary>
    Task<string?> GetProductNameAsync(string goodsId);

    /// <summary>
    /// 根據商品名稱查詢商品列表（用於前端下拉選擇）
    /// </summary>
    Task<List<ProductDto>> GetProductsByNameAsync(string goodsName, int maxCount = 50);
}

