using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 使用者排程維護作業控制器 (SYS0116)
/// </summary>
[Route("api/v1/user-schedules")]
public class UserSchedulesController : BaseController
{
    private readonly IUserScheduleService _service;

    public UserSchedulesController(
        IUserScheduleService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢排程列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserScheduleDto>>>> GetSchedules(
        [FromQuery] UserScheduleQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSchedulesAsync(query);
            return result;
        }, "查詢排程列表失敗");
    }

    /// <summary>
    /// 查詢單筆排程
    /// </summary>
    [HttpGet("{scheduleId}")]
    public async Task<ActionResult<ApiResponse<UserScheduleDto>>> GetSchedule(Guid scheduleId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetScheduleByIdAsync(scheduleId);
            return result;
        }, $"查詢排程失敗: {scheduleId}");
    }

    /// <summary>
    /// 新增排程
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateSchedule(
        [FromBody] CreateUserScheduleDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateScheduleAsync(dto);
            return result;
        }, "新增排程失敗");
    }

    /// <summary>
    /// 修改排程
    /// </summary>
    [HttpPut("{scheduleId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSchedule(
        Guid scheduleId,
        [FromBody] UpdateUserScheduleDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateScheduleAsync(scheduleId, dto);
        }, $"修改排程失敗: {scheduleId}");
    }

    /// <summary>
    /// 取消排程
    /// </summary>
    [HttpPost("{scheduleId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelSchedule(Guid scheduleId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelScheduleAsync(scheduleId);
        }, $"取消排程失敗: {scheduleId}");
    }

    /// <summary>
    /// 執行排程（系統內部）
    /// </summary>
    [HttpPost("{scheduleId}/execute")]
    public async Task<ActionResult<ApiResponse<object>>> ExecuteSchedule(Guid scheduleId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ExecuteScheduleAsync(scheduleId);
        }, $"執行排程失敗: {scheduleId}");
    }
}
