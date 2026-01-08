using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 商品分類 Repository 介面
/// </summary>
public interface IProductCategoryRepository
{
    /// <summary>
    /// 根據主鍵查詢商品分類
    /// </summary>
    Task<ProductCategory?> GetByIdAsync(long tKey);

    /// <summary>
    /// 查詢商品分類列表（分頁）
    /// </summary>
    Task<PagedResult<ProductCategory>> QueryAsync(ProductCategoryQuery query);

    /// <summary>
    /// 查詢商品分類樹狀結構
    /// </summary>
    Task<List<ProductCategory>> GetTreeAsync(ProductCategoryTreeQuery query);

    /// <summary>
    /// 新增商品分類
    /// </summary>
    Task<ProductCategory> CreateAsync(ProductCategory category);

    /// <summary>
    /// 修改商品分類
    /// </summary>
    Task<ProductCategory> UpdateAsync(ProductCategory category);

    /// <summary>
    /// 刪除商品分類
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 檢查商品分類是否存在
    /// </summary>
    Task<bool> ExistsAsync(string classId, string classMode, long? parentTKey, long? excludeTKey = null);

    /// <summary>
    /// 檢查是否有子分類
    /// </summary>
    Task<bool> HasChildrenAsync(long tKey);

    /// <summary>
    /// 查詢大分類列表
    /// </summary>
    Task<List<ProductCategory>> GetBClassListAsync(ProductCategoryListQuery query);

    /// <summary>
    /// 查詢中分類列表
    /// </summary>
    Task<List<ProductCategory>> GetMClassListAsync(ProductCategoryListQuery query);

    /// <summary>
    /// 查詢小分類列表
    /// </summary>
    Task<List<ProductCategory>> GetSClassListAsync(ProductCategoryListQuery query);
}

/// <summary>
/// 商品分類查詢條件
/// </summary>
public class ProductCategoryQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ClassId { get; set; }
    public string? ClassName { get; set; }
    public string? ClassMode { get; set; }
    public string? ClassType { get; set; }
    public string? BClassId { get; set; }
    public string? MClassId { get; set; }
    public long? ParentTKey { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 商品分類樹狀查詢條件
/// </summary>
public class ProductCategoryTreeQuery
{
    public string? ClassType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 商品分類列表查詢條件
/// </summary>
public class ProductCategoryListQuery
{
    public string? BClassId { get; set; }
    public string? MClassId { get; set; }
    public string? ClassType { get; set; }
    public string? Status { get; set; }
}

