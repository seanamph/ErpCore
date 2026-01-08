using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.UiComponent;
using ErpCore.Application.Services.UiComponent;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.UiComponent;

/// <summary>
/// UI組件查詢與報表控制器
/// </summary>
[Route("api/v1/ui-components/usage")]
public class UiComponentQueryController : BaseController
{
    private readonly IUiComponentQueryService _service;

    public UiComponentQueryController(
        IUiComponentQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢UI組件使用情況
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UIComponentUsageStatsDto>>>> GetUsageStats([FromQuery] UIComponentUsageQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetUsageStatsAsync(query);
        }, "查詢UI組件使用情況失敗");
    }

    /// <summary>
    /// 取得UI組件使用統計資訊
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<UIComponentUsageSummaryDto>>> GetUsageSummary()
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetUsageSummaryAsync();
        }, "取得UI組件使用統計資訊失敗");
    }
}

