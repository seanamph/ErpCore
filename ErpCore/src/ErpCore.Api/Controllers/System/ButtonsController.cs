using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 系統功能按鈕資料維護作業控制器 (SYS0440)
/// </summary>
[Route("api/v1/buttons")]
public class ButtonsController : BaseController
{
    private readonly IButtonService _service;

    public ButtonsController(
        IButtonService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢按鈕列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ButtonDto>>>> GetButtons(
        [FromQuery] ButtonQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetButtonsAsync(query);
            return result;
        }, "查詢按鈕列表失敗");
    }

    /// <summary>
    /// 查詢單筆按鈕
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<ButtonDto>>> GetButton(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetButtonByIdAsync(tKey);
            return result;
        }, $"查詢按鈕失敗: {tKey}");
    }

    /// <summary>
    /// 新增按鈕
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateButton(
        [FromBody] CreateButtonDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateButtonAsync(dto);
            return result;
        }, "新增按鈕失敗");
    }

    /// <summary>
    /// 修改按鈕
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateButton(
        long tKey,
        [FromBody] UpdateButtonDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateButtonAsync(tKey, dto);
        }, $"修改按鈕失敗: {tKey}");
    }

    /// <summary>
    /// 刪除按鈕
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteButton(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteButtonAsync(tKey);
        }, $"刪除按鈕失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除按鈕
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteButtonsBatch(
        [FromBody] BatchDeleteButtonDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteButtonsAsync(dto);
        }, "批次刪除按鈕失敗");
    }
}
