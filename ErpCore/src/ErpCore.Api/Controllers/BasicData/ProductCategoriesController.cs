using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 商品分類資料維護控制器 (SYSB110)
/// </summary>
[Route("api/v1/product-categories")]
public class ProductCategoriesController : BaseController
{
    private readonly IProductCategoryService _service;

    public ProductCategoriesController(
        IProductCategoryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商品分類列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductCategoryDto>>>> GetProductCategories(
        [FromQuery] ProductCategoryQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProductCategoriesAsync(query);
            return result;
        }, "查詢商品分類列表失敗");
    }

    /// <summary>
    /// 查詢單筆商品分類
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> GetProductCategory(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProductCategoryByIdAsync(tKey);
            return result;
        }, $"查詢商品分類失敗: {tKey}");
    }

    /// <summary>
    /// 查詢商品分類樹狀結構
    /// </summary>
    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<ProductCategoryTreeDto>>>> GetProductCategoryTree(
        [FromQuery] ProductCategoryTreeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProductCategoryTreeAsync(query);
            return result;
        }, "查詢商品分類樹狀結構失敗");
    }

    /// <summary>
    /// 新增商品分類
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateProductCategory(
        [FromBody] CreateProductCategoryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateProductCategoryAsync(dto);
            return result;
        }, "新增商品分類失敗");
    }

    /// <summary>
    /// 修改商品分類
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProductCategory(
        long tKey,
        [FromBody] UpdateProductCategoryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateProductCategoryAsync(tKey, dto);
        }, $"修改商品分類失敗: {tKey}");
    }

    /// <summary>
    /// 刪除商品分類
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProductCategory(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProductCategoryAsync(tKey);
        }, $"刪除商品分類失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除商品分類
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProductCategoriesBatch(
        [FromBody] BatchDeleteProductCategoryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteProductCategoriesBatchAsync(dto);
        }, "批次刪除商品分類失敗");
    }

    /// <summary>
    /// 查詢大分類列表
    /// </summary>
    [HttpGet("b-class")]
    public async Task<ActionResult<ApiResponse<List<ProductCategoryDto>>>> GetBClassList(
        [FromQuery] ProductCategoryListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBClassListAsync(query);
            return result;
        }, "查詢大分類列表失敗");
    }

    /// <summary>
    /// 查詢中分類列表
    /// </summary>
    [HttpGet("m-class")]
    public async Task<ActionResult<ApiResponse<List<ProductCategoryDto>>>> GetMClassList(
        [FromQuery] ProductCategoryListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMClassListAsync(query);
            return result;
        }, "查詢中分類列表失敗");
    }

    /// <summary>
    /// 查詢小分類列表
    /// </summary>
    [HttpGet("s-class")]
    public async Task<ActionResult<ApiResponse<List<ProductCategoryDto>>>> GetSClassList(
        [FromQuery] ProductCategoryListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSClassListAsync(query);
            return result;
        }, "查詢小分類列表失敗");
    }

    /// <summary>
    /// 更新商品分類狀態
    /// </summary>
    [HttpPut("{tKey}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProductCategoryStatus(
        long tKey,
        [FromBody] UpdateProductCategoryStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(tKey, dto);
        }, $"更新商品分類狀態失敗: {tKey}");
    }

    /// <summary>
    /// 更新商品分類項目個數
    /// </summary>
    [HttpPut("{tKey}/item-count")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProductCategoryItemCount(
        long tKey,
        [FromBody] UpdateProductCategoryItemCountDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateItemCountAsync(tKey, dto);
        }, $"更新商品分類項目個數失敗: {tKey}");
    }
}

