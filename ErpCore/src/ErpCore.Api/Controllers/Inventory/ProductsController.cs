using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Inventory;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Inventory;

/// <summary>
/// 商品管理控制器
/// </summary>
[ApiController]
[Route("api/v1/products")]
public class ProductsController : BaseController
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService, ILoggerService logger)
        : base(logger)
    {
        _productService = productService;
    }

    /// <summary>
    /// 查詢商品列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts([FromQuery] ProductQueryDto query)
    {
        return await ExecuteAsync(async () => await _productService.GetProductsAsync(query), "查詢商品列表失敗");
    }

    /// <summary>
    /// 根據商品編號查詢商品資訊
    /// </summary>
    [HttpGet("{goodsId}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductById(string goodsId)
    {
        return await ExecuteAsync(async () => await _productService.GetProductByIdAsync(goodsId), "查詢商品資訊失敗");
    }

    /// <summary>
    /// 根據商品編號查詢商品名稱（簡化版，用於快速查詢）
    /// </summary>
    [HttpGet("{goodsId}/name")]
    public async Task<ActionResult<ApiResponse<string>>> GetProductName(string goodsId)
    {
        return await ExecuteAsync(async () => await _productService.GetProductNameAsync(goodsId), "查詢商品名稱失敗");
    }

    /// <summary>
    /// 根據商品名稱查詢商品列表（用於前端下拉選擇）
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> SearchProductsByName(
        [FromQuery] string goodsName,
        [FromQuery] int maxCount = 50)
    {
        return await ExecuteAsync(
            async () => await _productService.GetProductsByNameAsync(goodsName, maxCount),
            "根據商品名稱查詢商品列表失敗");
    }
}

