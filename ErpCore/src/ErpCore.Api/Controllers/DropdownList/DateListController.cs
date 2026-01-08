using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.DropdownList;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.DropdownList;

/// <summary>
/// 日期列表控制器 (DATE_LIST)
/// </summary>
[Route("api/v1/lists/dates")]
public class DateListController : BaseController
{
    private readonly IDateService _dateService;

    public DateListController(
        IDateService dateService,
        ILoggerService logger) : base(logger)
    {
        _dateService = dateService;
    }

    /// <summary>
    /// 取得系統日期格式設定
    /// </summary>
    [HttpGet("format")]
    public async Task<ActionResult<ApiResponse<DateFormatDto>>> GetDateFormat()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dateService.GetDateFormatAsync();
            return result;
        }, "取得系統日期格式設定失敗");
    }

    /// <summary>
    /// 驗證日期格式
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<DateValidationDto>>> ValidateDate(
        [FromBody] DateValidationRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dateService.ValidateDateAsync(request.DateString, request.DateFormat);
            return result;
        }, "驗證日期格式失敗");
    }

    /// <summary>
    /// 解析日期字串
    /// </summary>
    [HttpPost("parse")]
    public async Task<ActionResult<ApiResponse<DateTime?>>> ParseDate(
        [FromBody] DateValidationRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dateService.ParseDateAsync(request.DateString, request.DateFormat);
            return result;
        }, "解析日期字串失敗");
    }
}

