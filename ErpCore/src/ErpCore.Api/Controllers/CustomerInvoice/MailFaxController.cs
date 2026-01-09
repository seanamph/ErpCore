using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CustomerInvoice;

/// <summary>
/// 郵件傳真作業控制器 (SYS2000 - 郵件傳真作業)
/// </summary>
[Route("api/v1/mail-fax")]
public class MailFaxController : BaseController
{
    private readonly IMailFaxService _service;

    public MailFaxController(
        IMailFaxService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢郵件傳真作業列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EmailFaxJobDto>>>> GetEmailFaxJobs(
        [FromQuery] EmailFaxJobQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEmailFaxJobsAsync(query);
            return result;
        }, "查詢郵件傳真作業列表失敗");
    }

    /// <summary>
    /// 查詢單筆郵件傳真作業
    /// </summary>
    [HttpGet("{jobId}")]
    public async Task<ActionResult<ApiResponse<EmailFaxJobDto>>> GetEmailFaxJob(string jobId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEmailFaxJobByIdAsync(jobId);
            return result;
        }, $"查詢郵件傳真作業失敗: {jobId}");
    }

    /// <summary>
    /// 發送郵件
    /// </summary>
    [HttpPost("send-email")]
    public async Task<ActionResult<ApiResponse<string>>> SendEmail(
        [FromBody] SendEmailRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SendEmailAsync(request);
            return result;
        }, "發送郵件失敗");
    }

    /// <summary>
    /// 發送傳真
    /// </summary>
    [HttpPost("send-fax")]
    public async Task<ActionResult<ApiResponse<string>>> SendFax(
        [FromBody] SendFaxRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SendFaxAsync(request);
            return result;
        }, "發送傳真失敗");
    }

    /// <summary>
    /// 查詢發送記錄
    /// </summary>
    [HttpGet("{jobId}/logs")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmailFaxLogDto>>>> GetLogs(string jobId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLogsAsync(jobId);
            return result;
        }, $"查詢發送記錄失敗: {jobId}");
    }
}

