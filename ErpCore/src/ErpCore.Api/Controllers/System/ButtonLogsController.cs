using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 按鈕操作記錄控制器 (SYS0790)
/// </summary>
[ApiController]
[Route("api/v1/button-logs")]
public class ButtonLogsController : BaseController
{
    private readonly IButtonLogService _service;

    public ButtonLogsController(
        IButtonLogService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢按鈕操作記錄列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ButtonLogDto>>>> GetButtonLogs(
        [FromQuery] ButtonLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetButtonLogsAsync(query);
            return result;
        }, "查詢按鈕操作記錄失敗");
    }

    /// <summary>
    /// 新增按鈕操作記錄
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateButtonLog(
        [FromBody] CreateButtonLogDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateButtonLogAsync(dto);
            return result;
        }, "新增按鈕操作記錄失敗");
    }

    /// <summary>
    /// 匯出按鈕操作記錄報表
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportButtonLogReport(
        [FromBody] ButtonLogQueryDto query,
        [FromQuery] string format = "excel")
    {
        return await ExecuteAsync(async () =>
        {
            var bytes = await _service.ExportButtonLogReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"ButtonLog_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(bytes, contentType, fileName);
        }, "匯出按鈕操作記錄報表失敗");
    }
}

