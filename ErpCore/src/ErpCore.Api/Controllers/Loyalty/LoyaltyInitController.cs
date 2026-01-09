using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Loyalty;
using ErpCore.Application.Services.Loyalty;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Loyalty;

/// <summary>
/// 忠誠度系統初始化控制器 (WEBLOYALTYINI - 忠誠度系統初始化)
/// </summary>
[Route("api/v1/loyalty-system")]
public class LoyaltyInitController : BaseController
{
    private readonly ILoyaltyInitService _service;

    public LoyaltyInitController(
        ILoyaltyInitService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢系統設定列表
    /// </summary>
    [HttpGet("configs")]
    public async Task<ActionResult<ApiResponse<PagedResult<LoyaltySystemConfigDto>>>> GetConfigs(
        [FromQuery] LoyaltySystemConfigQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigsAsync(query);
            return result;
        }, "查詢系統設定列表失敗");
    }

    /// <summary>
    /// 查詢單筆系統設定
    /// </summary>
    [HttpGet("configs/{configId}")]
    public async Task<ActionResult<ApiResponse<LoyaltySystemConfigDto>>> GetConfig(string configId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigByIdAsync(configId);
            return result;
        }, $"查詢系統設定失敗: {configId}");
    }

    /// <summary>
    /// 新增系統設定
    /// </summary>
    [HttpPost("configs")]
    public async Task<ActionResult<ApiResponse<string>>> CreateConfig(
        [FromBody] CreateLoyaltySystemConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateConfigAsync(dto);
            return result;
        }, "新增系統設定失敗");
    }

    /// <summary>
    /// 修改系統設定
    /// </summary>
    [HttpPut("configs/{configId}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateConfig(
        string configId,
        [FromBody] UpdateLoyaltySystemConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateConfigAsync(configId, dto);
            return true;
        }, $"修改系統設定失敗: {configId}");
    }

    /// <summary>
    /// 刪除系統設定
    /// </summary>
    [HttpDelete("configs/{configId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteConfig(string configId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigAsync(configId);
            return true;
        }, $"刪除系統設定失敗: {configId}");
    }

    /// <summary>
    /// 執行系統初始化
    /// </summary>
    [HttpPost("initialize")]
    public async Task<ActionResult<ApiResponse<LoyaltySystemInitResponseDto>>> Initialize(
        [FromBody] InitializeLoyaltySystemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.InitializeAsync(dto);
            return result;
        }, "執行系統初始化失敗");
    }

    /// <summary>
    /// 查詢初始化記錄列表
    /// </summary>
    [HttpGet("init-logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<LoyaltySystemInitLogDto>>>> GetInitLogs(
        [FromQuery] LoyaltySystemInitLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInitLogsAsync(query);
            return result;
        }, "查詢初始化記錄列表失敗");
    }
}

