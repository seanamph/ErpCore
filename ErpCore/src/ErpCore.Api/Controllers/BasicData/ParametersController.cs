using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 參數資料設定維護控制器 (SYSBC40)
/// </summary>
[Route("api/v1/parameters")]
public class ParametersController : BaseController
{
    private readonly IParameterService _service;

    public ParametersController(
        IParameterService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢參數列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ParameterDto>>>> GetParameters(
        [FromQuery] ParameterQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetParametersAsync(query);
            return result;
        }, "查詢參數列表失敗");
    }

    /// <summary>
    /// 查詢單筆參數
    /// </summary>
    [HttpGet("{title}/{tag}")]
    public async Task<ActionResult<ApiResponse<ParameterDto>>> GetParameter(string title, string tag)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetParameterAsync(title, tag);
            return result;
        }, $"查詢參數失敗: {title}/{tag}");
    }

    /// <summary>
    /// 根據參數標題查詢參數列表
    /// </summary>
    [HttpGet("by-title/{title}")]
    public async Task<ActionResult<ApiResponse<List<ParameterDto>>>> GetParametersByTitle(string title)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetParametersByTitleAsync(title);
            return result;
        }, $"根據標題查詢參數列表失敗: {title}");
    }

    /// <summary>
    /// 新增參數
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ParameterKeyDto>>> CreateParameter(
        [FromBody] CreateParameterDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateParameterAsync(dto);
            return result;
        }, "新增參數失敗");
    }

    /// <summary>
    /// 修改參數
    /// </summary>
    [HttpPut("{title}/{tag}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateParameter(
        string title,
        string tag,
        [FromBody] UpdateParameterDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateParameterAsync(title, tag, dto);
        }, $"修改參數失敗: {title}/{tag}");
    }

    /// <summary>
    /// 刪除參數
    /// </summary>
    [HttpDelete("{title}/{tag}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteParameter(string title, string tag)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteParameterAsync(title, tag);
        }, $"刪除參數失敗: {title}/{tag}");
    }

    /// <summary>
    /// 批次刪除參數
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteParametersBatch(
        [FromBody] BatchDeleteParameterDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteParametersBatchAsync(dto);
        }, "批次刪除參數失敗");
    }

    /// <summary>
    /// 取得參數值
    /// </summary>
    [HttpGet("value/{title}/{tag}")]
    public async Task<ActionResult<ApiResponse<ParameterValueDto>>> GetParameterValue(
        string title,
        string tag,
        [FromQuery] string lang = null)
    {
        return await ExecuteAsync(async () =>
        {
            var value = await _service.GetParameterValueAsync(title, tag, lang);
            return new ParameterValueDto { Value = value };
        }, $"取得參數值失敗: {title}/{tag}");
    }
}
