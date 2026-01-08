using ErpCore.Application.DTOs.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 商品進銷碼服務介面
/// </summary>
public interface IProductGoodsIdService
{
    /// <summary>
    /// 查詢商品進銷碼列表
    /// </summary>
    Task<PagedResult<ProductGoodsIdDto>> GetProductGoodsIdsAsync(ProductGoodsIdQueryDto query);

    /// <summary>
    /// 查詢單筆商品進銷碼
    /// </summary>
    Task<ProductGoodsIdDto> GetProductGoodsIdByIdAsync(string goodsId);

    /// <summary>
    /// 新增商品進銷碼
    /// </summary>
    Task<string> CreateProductGoodsIdAsync(CreateProductGoodsIdDto dto);

    /// <summary>
    /// 修改商品進銷碼
    /// </summary>
    Task UpdateProductGoodsIdAsync(string goodsId, UpdateProductGoodsIdDto dto);

    /// <summary>
    /// 刪除商品進銷碼
    /// </summary>
    Task DeleteProductGoodsIdAsync(string goodsId);

    /// <summary>
    /// 批次刪除商品進銷碼
    /// </summary>
    Task BatchDeleteProductGoodsIdAsync(BatchDeleteProductGoodsIdDto dto);

    /// <summary>
    /// 檢查商品進銷碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string goodsId);

    /// <summary>
    /// 更新商品進銷碼狀態
    /// </summary>
    Task UpdateStatusAsync(string goodsId, UpdateProductGoodsIdStatusDto dto);
}

