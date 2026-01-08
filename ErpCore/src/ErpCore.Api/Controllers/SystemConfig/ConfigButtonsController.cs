using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Application.Services.SystemConfig;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemConfig;

/// <summary>
/// 系統功能按鈕資料維護作業控制器 (CFG0440)
/// </summary>
[Route("api/v1/config/buttons")]
public class ConfigButtonsController : BaseController
{
    private readonly IConfigButtonService _service;

    public ConfigButtonsController(
        IConfigButtonService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢按鈕列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ConfigButtonDto>>>> GetConfigButtons(
        [FromQuery] ConfigButtonQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigButtonsAsync(query);
            return result;
        }, "查詢按鈕列表失敗");
    }

    /// <summary>
    /// 查詢單筆按鈕
    /// </summary>
    [HttpGet("{buttonId}")]
    public async Task<ActionResult<ApiResponse<ConfigButtonDto>>> GetConfigButton(string buttonId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConfigButtonAsync(buttonId);
            return result;
        }, $"查詢按鈕失敗: {buttonId}");
    }

    /// <summary>
    /// 新增按鈕
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateConfigButton(
        [FromBody] CreateConfigButtonDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateConfigButtonAsync(dto);
            return result;
        }, "新增按鈕失敗");
    }

    /// <summary>
    /// 修改按鈕
    /// </summary>
    [HttpPut("{buttonId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateConfigButton(
        string buttonId,
        [FromBody] UpdateConfigButtonDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateConfigButtonAsync(buttonId, dto);
        }, $"修改按鈕失敗: {buttonId}");
    }

    /// <summary>
    /// 刪除按鈕
    /// </summary>
    [HttpDelete("{buttonId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteConfigButton(string buttonId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigButtonAsync(buttonId);
        }, $"刪除按鈕失敗: {buttonId}");
    }

    /// <summary>
    /// 批次刪除按鈕
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteConfigButtonsBatch(
        [FromBody] BatchDeleteConfigButtonDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteConfigButtonsBatchAsync(dto);
        }, "批次刪除按鈕失敗");
    }
}

