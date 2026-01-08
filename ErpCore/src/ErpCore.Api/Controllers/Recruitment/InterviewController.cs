using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Application.Services.Recruitment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Recruitment;

/// <summary>
/// 訪談維護作業控制器 (SYSC222)
/// </summary>
[Route("api/v1/interviews")]
public class InterviewController : BaseController
{
    private readonly IInterviewService _service;

    public InterviewController(
        IInterviewService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢訪談列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<InterviewDto>>>> GetInterviews(
        [FromQuery] InterviewQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInterviewsAsync(query);
            return result;
        }, "查詢訪談列表失敗");
    }

    /// <summary>
    /// 根據潛客查詢訪談列表
    /// </summary>
    [HttpGet("by-prospect/{prospectId}")]
    public async Task<ActionResult<ApiResponse<PagedResult<InterviewDto>>>> GetInterviewsByProspect(
        string prospectId,
        [FromQuery] InterviewQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInterviewsByProspectIdAsync(prospectId, query);
            return result;
        }, $"根據潛客查詢訪談列表失敗: {prospectId}");
    }

    /// <summary>
    /// 查詢單筆訪談
    /// </summary>
    [HttpGet("{interviewId}")]
    public async Task<ActionResult<ApiResponse<InterviewDto>>> GetInterview(long interviewId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInterviewByIdAsync(interviewId);
            return result;
        }, $"查詢訪談失敗: {interviewId}");
    }

    /// <summary>
    /// 新增訪談
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateInterview(
        [FromBody] CreateInterviewDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateInterviewAsync(dto);
            return result;
        }, "新增訪談失敗");
    }

    /// <summary>
    /// 修改訪談
    /// </summary>
    [HttpPut("{interviewId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateInterview(
        long interviewId,
        [FromBody] UpdateInterviewDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateInterviewAsync(interviewId, dto);
        }, $"修改訪談失敗: {interviewId}");
    }

    /// <summary>
    /// 刪除訪談
    /// </summary>
    [HttpDelete("{interviewId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteInterview(long interviewId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteInterviewAsync(interviewId);
        }, $"刪除訪談失敗: {interviewId}");
    }

    /// <summary>
    /// 批次刪除訪談
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteInterviews(
        [FromBody] BatchDeleteInterviewDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteInterviewsAsync(dto);
        }, "批次刪除訪談失敗");
    }

    /// <summary>
    /// 更新訪談狀態
    /// </summary>
    [HttpPatch("{interviewId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateInterviewStatus(
        long interviewId,
        [FromBody] UpdateInterviewStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateInterviewStatusAsync(interviewId, dto);
        }, $"更新訪談狀態失敗: {interviewId}");
    }
}

