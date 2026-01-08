using ErpCore.Domain.Entities.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 商品進銷碼 Repository 介面
/// </summary>
public interface IProductGoodsIdRepository
{
    /// <summary>
    /// 根據進銷碼查詢商品
    /// </summary>
    Task<Product?> GetByIdAsync(string goodsId);

    /// <summary>
    /// 查詢商品列表（分頁）
    /// </summary>
    Task<PagedResult<Product>> QueryAsync(ProductGoodsIdQuery query);

    /// <summary>
    /// 檢查商品是否存在
    /// </summary>
    Task<bool> ExistsAsync(string goodsId);

    /// <summary>
    /// 新增商品
    /// </summary>
    Task<Product> CreateAsync(Product product);

    /// <summary>
    /// 修改商品
    /// </summary>
    Task<Product> UpdateAsync(Product product);

    /// <summary>
    /// 刪除商品
    /// </summary>
    Task DeleteAsync(string goodsId);

    /// <summary>
    /// 批次刪除商品
    /// </summary>
    Task BatchDeleteAsync(List<string> goodsIds);
}

/// <summary>
/// 商品進銷碼查詢條件
/// </summary>
public class ProductGoodsIdQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? BarcodeId { get; set; }
    public string? ScId { get; set; }
    public string? Status { get; set; }
    public string? ShopId { get; set; }
}

