using ErpCore.Domain.Entities.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 供應商商品 Repository 介面
/// </summary>
public interface ISupplierGoodsRepository
{
    /// <summary>
    /// 根據主鍵查詢供應商商品
    /// </summary>
    Task<SupplierGoods?> GetByIdAsync(string supplierId, string barcodeId, string shopId);

    /// <summary>
    /// 查詢供應商商品列表（分頁）
    /// </summary>
    Task<PagedResult<SupplierGoods>> QueryAsync(SupplierGoodsQuery query);

    /// <summary>
    /// 新增供應商商品
    /// </summary>
    Task<SupplierGoods> CreateAsync(SupplierGoods supplierGoods);

    /// <summary>
    /// 修改供應商商品
    /// </summary>
    Task<SupplierGoods> UpdateAsync(SupplierGoods supplierGoods);

    /// <summary>
    /// 刪除供應商商品
    /// </summary>
    Task DeleteAsync(string supplierId, string barcodeId, string shopId);

    /// <summary>
    /// 檢查是否存在
    /// </summary>
    Task<bool> ExistsAsync(string supplierId, string barcodeId, string shopId);
}

/// <summary>
/// 供應商商品查詢條件
/// </summary>
public class SupplierGoodsQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SupplierId { get; set; }
    public string? BarcodeId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
}
