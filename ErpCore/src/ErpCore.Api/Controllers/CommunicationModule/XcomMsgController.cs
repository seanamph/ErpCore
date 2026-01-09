using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CommunicationModule;
using ErpCore.Application.Services.CommunicationModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CommunicationModule;

/// <summary>
/// XCOMMSG系列錯誤訊息處理控制器
/// 提供錯誤訊息處理、HTTP錯誤頁面、警告頁面等功能
/// </summary>
[Route("api/v1/error-messages")]
public class XcomMsgController : BaseController
{
    private readonly IErrorMessageService _service;

    public XcomMsgController(
        IErrorMessageService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢錯誤訊息列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ErrorMessageDto>>>> GetErrorMessages(
        [FromQuery] ErrorMessageQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetErrorMessagesAsync(query);
            return result;
        }, "查詢錯誤訊息列表失敗");
    }

    /// <summary>
    /// 查詢單筆錯誤訊息
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<ErrorMessageDto>>> GetErrorMessage(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetErrorMessageByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"錯誤訊息不存在: {tKey}");
            }
            return result;
        }, $"查詢錯誤訊息失敗: {tKey}");
    }

    /// <summary>
    /// 記錄錯誤訊息
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateErrorMessage(
        [FromBody] CreateErrorMessageDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateErrorMessageAsync(dto);
            return result;
        }, "記錄錯誤訊息失敗");
    }

    /// <summary>
    /// 取得HTTP錯誤頁面資訊
    /// </summary>
    [HttpGet("error-pages/{statusCode}")]
    public async Task<ActionResult<ApiResponse<ErrorPageDto>>> GetErrorPage(
        int statusCode,
        [FromQuery] string language = "zh-TW")
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetErrorPageAsync(statusCode, language);
            return result;
        }, $"取得HTTP錯誤頁面資訊失敗: {statusCode}");
    }

    /// <summary>
    /// 取得警告頁面資訊
    /// </summary>
    [HttpGet("warning-pages/{warningCode}")]
    public async Task<ActionResult<ApiResponse<WarningPageDto>>> GetWarningPage(
        string warningCode,
        [FromQuery] string language = "zh-TW")
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetWarningPageAsync(warningCode, language);
            return result;
        }, $"取得警告頁面資訊失敗: {warningCode}");
    }

    /// <summary>
    /// 查詢錯誤訊息模板列表
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ErrorMessageTemplateDto>>>> GetTemplates(
        [FromQuery] string? errorCode = null,
        [FromQuery] string? language = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTemplatesAsync(errorCode, language);
            return result;
        }, "查詢錯誤訊息模板列表失敗");
    }
}

