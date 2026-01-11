using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Security.Claims;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 使用者異常登入記錄控制器 (SYS0760)
/// </summary>
[ApiController]
[Route("api/v1/system/login-log")]
public class LoginLogController : BaseController
{
    private readonly ILoginLogService _service;

    public LoginLogController(
        ILoginLogService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢異常登入記錄列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LoginLogDto>>>> GetLoginLogs(
        [FromQuery] LoginLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLoginLogsAsync(query);
            return result;
        }, "查詢異常登入記錄失敗");
    }

    /// <summary>
    /// 取得異常事件代碼選項
    /// </summary>
    [HttpGet("event-types")]
    public async Task<ActionResult<ApiResponse<List<EventTypeDto>>>> GetEventTypes()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEventTypesAsync();
            return result;
        }, "查詢異常事件代碼選項失敗");
    }

    /// <summary>
    /// 刪除異常登入記錄
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<DeleteResultDto>>> DeleteLoginLogs(
        [FromBody] DeleteLoginLogRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("UserId")?.Value
                             ?? "SYSTEM";

            var deletedCount = await _service.DeleteLoginLogsAsync(request.TKeys, currentUserId);

            return new DeleteResultDto
            {
                DeletedCount = deletedCount
            };
        }, "刪除異常登入記錄失敗");
    }

    /// <summary>
    /// 產生異常登入報表
    /// </summary>
    [HttpPost("report")]
    public async Task<IActionResult> GenerateReport(
        [FromBody] LoginLogReportDto reportDto,
        [FromQuery] string format = "PDF")
    {
        return await ExecuteAsync(async () =>
        {
            var bytes = await _service.GenerateReportAsync(reportDto, format);
            var contentType = format.ToUpper() == "PDF" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"使用者異常登入報表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToUpper() == "PDF" ? "pdf" : "xlsx")}";
            return File(bytes, contentType, fileName);
        }, "產生異常登入報表失敗");
    }
}
