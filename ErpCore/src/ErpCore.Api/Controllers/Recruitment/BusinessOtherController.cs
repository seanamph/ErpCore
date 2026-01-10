using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Application.Services.Recruitment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Recruitment;

/// <summary>
/// 招商其他功能控制器 (SYSC999)
/// </summary>
[Route("api/v1/tenant-locations")]
public class BusinessOtherController : BaseController
{
    private readonly IBusinessOtherService _service;

    public BusinessOtherController(
        IBusinessOtherService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢租戶位置列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TenantLocationDto>>>> GetTenantLocations(
        [FromQuery] TenantLocationQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTenantLocationsAsync(query);
            return result;
        }, "查詢租戶位置列表失敗");
    }

    /// <summary>
    /// 查詢單筆租戶位置
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<TenantLocationDto>>> GetTenantLocation(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTenantLocationAsync(tKey);
            return result;
        }, $"查詢租戶位置失敗: {tKey}");
    }

    /// <summary>
    /// 根據租戶查詢位置列表
    /// </summary>
    [HttpGet("by-tenant/{agmTKey}")]
    public async Task<ActionResult<ApiResponse<List<TenantLocationDto>>>> GetTenantLocationsByTenant(long agmTKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTenantLocationsByTenantAsync(agmTKey);
            return result;
        }, $"根據租戶查詢位置列表失敗: {agmTKey}");
    }

    /// <summary>
    /// 新增租戶位置
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TenantLocationKeyDto>>> CreateTenantLocation(
        [FromBody] CreateTenantLocationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateTenantLocationAsync(dto);
            return result;
        }, "新增租戶位置失敗");
    }

    /// <summary>
    /// 修改租戶位置
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTenantLocation(
        long tKey,
        [FromBody] UpdateTenantLocationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateTenantLocationAsync(tKey, dto);
        }, $"修改租戶位置失敗: {tKey}");
    }

    /// <summary>
    /// 刪除租戶位置
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTenantLocation(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTenantLocationAsync(tKey);
        }, $"刪除租戶位置失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除租戶位置
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteTenantLocations(
        [FromBody] BatchDeleteTenantLocationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTenantLocationsBatchAsync(dto);
        }, "批次刪除租戶位置失敗");
    }
}

