using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreFloor;

/// <summary>
/// 類型代碼查詢控制器 (SYS6501-SYS6560 - 類型代碼查詢)
/// </summary>
[Route("api/v1/type-codes/query")]
public class TypeCodeQueryController : BaseController
{
    private readonly ITypeCodeQueryService _service;

    public TypeCodeQueryController(
        ITypeCodeQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢類型代碼列表（進階查詢）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PagedResult<TypeCodeQueryResultDto>>>> QueryTypeCodes(
        [FromBody] TypeCodeQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryTypeCodesAsync(request);
            return result;
        }, "查詢類型代碼列表失敗");
    }

    /// <summary>
    /// 查詢類型代碼統計資訊
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<TypeCodeStatisticsDto>>> GetTypeCodeStatistics(
        [FromQuery] TypeCodeStatisticsRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTypeCodeStatisticsAsync(request);
            return result;
        }, "查詢類型代碼統計資訊失敗");
    }
}

