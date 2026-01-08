namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 商品分類 DTO
/// </summary>
public class ProductCategoryDto
{
    public long TKey { get; set; }
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? ClassType { get; set; }
    public string ClassMode { get; set; } = string.Empty;
    public string? BClassId { get; set; }
    public string? MClassId { get; set; }
    public long? ParentTKey { get; set; }
    public string? StypeId { get; set; }
    public string? StypeId2 { get; set; }
    public string? DepreStypeId { get; set; }
    public string? DepreStypeId2 { get; set; }
    public string? StypeTax { get; set; }
    public int? ItemCount { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 商品分類樹狀結構 DTO
/// </summary>
public class ProductCategoryTreeDto : ProductCategoryDto
{
    public List<ProductCategoryTreeDto> Children { get; set; } = new();
}

/// <summary>
/// 商品分類查詢 DTO
/// </summary>
public class ProductCategoryQueryDto
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
/// 商品分類樹狀查詢 DTO
/// </summary>
public class ProductCategoryTreeQueryDto
{
    public string? ClassType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 商品分類列表查詢 DTO（用於下拉選單）
/// </summary>
public class ProductCategoryListQueryDto
{
    public string? BClassId { get; set; }
    public string? MClassId { get; set; }
    public string? ClassType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增商品分類 DTO
/// </summary>
public class CreateProductCategoryDto
{
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? ClassType { get; set; }
    public string ClassMode { get; set; } = string.Empty;
    public string? BClassId { get; set; }
    public string? MClassId { get; set; }
    public long? ParentTKey { get; set; }
    public string? StypeId { get; set; }
    public string? StypeId2 { get; set; }
    public string? DepreStypeId { get; set; }
    public string? DepreStypeId2 { get; set; }
    public string? StypeTax { get; set; }
    public int? ItemCount { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改商品分類 DTO
/// </summary>
public class UpdateProductCategoryDto
{
    public string ClassName { get; set; } = string.Empty;
    public string? ClassType { get; set; }
    public string? BClassId { get; set; }
    public string? MClassId { get; set; }
    public long? ParentTKey { get; set; }
    public string? StypeId { get; set; }
    public string? StypeId2 { get; set; }
    public string? DepreStypeId { get; set; }
    public string? DepreStypeId2 { get; set; }
    public string? StypeTax { get; set; }
    public int? ItemCount { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除商品分類 DTO
/// </summary>
public class BatchDeleteProductCategoryDto
{
    public List<long> TKeys { get; set; } = new();
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateProductCategoryStatusDto
{
    public string Status { get; set; } = "A";
}

/// <summary>
/// 更新項目個數 DTO
/// </summary>
public class UpdateProductCategoryItemCountDto
{
    public int ItemCount { get; set; }
}

