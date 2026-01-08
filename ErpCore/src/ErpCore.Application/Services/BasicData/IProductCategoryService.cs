using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 商品分類服務介面
/// </summary>
public interface IProductCategoryService
{
    /// <summary>
    /// 查詢商品分類列表
    /// </summary>
    Task<PagedResult<ProductCategoryDto>> GetProductCategoriesAsync(ProductCategoryQueryDto query);

    /// <summary>
    /// 查詢單筆商品分類
    /// </summary>
    Task<ProductCategoryDto> GetProductCategoryByIdAsync(long tKey);

    /// <summary>
    /// 查詢商品分類樹狀結構
    /// </summary>
    Task<List<ProductCategoryTreeDto>> GetProductCategoryTreeAsync(ProductCategoryTreeQueryDto query);

    /// <summary>
    /// 新增商品分類
    /// </summary>
    Task<long> CreateProductCategoryAsync(CreateProductCategoryDto dto);

    /// <summary>
    /// 修改商品分類
    /// </summary>
    Task UpdateProductCategoryAsync(long tKey, UpdateProductCategoryDto dto);

    /// <summary>
    /// 刪除商品分類
    /// </summary>
    Task DeleteProductCategoryAsync(long tKey);

    /// <summary>
    /// 批次刪除商品分類
    /// </summary>
    Task DeleteProductCategoriesBatchAsync(BatchDeleteProductCategoryDto dto);

    /// <summary>
    /// 查詢大分類列表
    /// </summary>
    Task<List<ProductCategoryDto>> GetBClassListAsync(ProductCategoryListQueryDto query);

    /// <summary>
    /// 查詢中分類列表
    /// </summary>
    Task<List<ProductCategoryDto>> GetMClassListAsync(ProductCategoryListQueryDto query);

    /// <summary>
    /// 查詢小分類列表
    /// </summary>
    Task<List<ProductCategoryDto>> GetSClassListAsync(ProductCategoryListQueryDto query);

    /// <summary>
    /// 更新商品分類狀態
    /// </summary>
    Task UpdateStatusAsync(long tKey, UpdateProductCategoryStatusDto dto);

    /// <summary>
    /// 更新商品分類項目個數
    /// </summary>
    Task UpdateItemCountAsync(long tKey, UpdateProductCategoryItemCountDto dto);
}

