using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 店舖管理控制器
/// </summary>
[ApiController]
[Route("api/v1/shops")]
public class ShopsController : BaseController
{
    private readonly IShopService _shopService;

    public ShopsController(IShopService shopService, ILoggerService logger)
        : base(logger)
    {
        _shopService = shopService;
    }

    /// <summary>
    /// 查詢店舖列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ShopDto>>>> GetShops([FromQuery] ShopQueryDto? query = null)
    {
        return await ExecuteAsync(async () => await _shopService.GetShopsAsync(query), "查詢店舖列表失敗");
    }
}

