using ErpCore.Application.DTOs.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 供應商商品服務介面
/// </summary>
public interface ISupplierGoodsService
{
    /// <summary>
    /// 查詢供應商商品列表
    /// </summary>
    Task<PagedResult<SupplierGoodsDto>> GetSupplierGoodsAsync(SupplierGoodsQueryDto query);

    /// <summary>
    /// 查詢單筆供應商商品
    /// </summary>
    Task<SupplierGoodsDto> GetSupplierGoodsByIdAsync(string supplierId, string barcodeId, string shopId);

    /// <summary>
    /// 新增供應商商品
    /// </summary>
    Task CreateSupplierGoodsAsync(CreateSupplierGoodsDto dto);

    /// <summary>
    /// 修改供應商商品
    /// </summary>
    Task UpdateSupplierGoodsAsync(string supplierId, string barcodeId, string shopId, UpdateSupplierGoodsDto dto);

    /// <summary>
    /// 刪除供應商商品
    /// </summary>
    Task DeleteSupplierGoodsAsync(string supplierId, string barcodeId, string shopId);

    /// <summary>
    /// 批次刪除供應商商品
    /// </summary>
    Task DeleteSupplierGoodsBatchAsync(BatchDeleteSupplierGoodsDto dto);

    /// <summary>
    /// 更新狀態
    /// </summary>
    Task UpdateStatusAsync(string supplierId, string barcodeId, string shopId, string status);
}
