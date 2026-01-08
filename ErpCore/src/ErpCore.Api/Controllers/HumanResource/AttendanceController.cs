using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Application.Services.HumanResource;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.HumanResource;

/// <summary>
/// 考勤管理控制器
/// 提供員工考勤資料維護功能
/// </summary>
[Route("api/v1/human-resource/attendance")]
public class AttendanceController : BaseController
{
    private readonly IAttendanceService _service;

    public AttendanceController(
        IAttendanceService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢考勤列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AttendanceDto>>>> GetAttendances(
        [FromQuery] AttendanceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAttendancesAsync(query);
            return result;
        }, "查詢考勤列表失敗");
    }

    /// <summary>
    /// 查詢單筆考勤
    /// </summary>
    [HttpGet("{attendanceId}")]
    public async Task<ActionResult<ApiResponse<AttendanceDto>>> GetAttendance(string attendanceId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAttendanceByIdAsync(attendanceId);
            return result;
        }, $"查詢考勤失敗: {attendanceId}");
    }

    /// <summary>
    /// 新增考勤
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateAttendance(
        [FromBody] CreateAttendanceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateAttendanceAsync(dto);
            return result;
        }, "新增考勤失敗");
    }

    /// <summary>
    /// 修改考勤
    /// </summary>
    [HttpPut("{attendanceId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAttendance(
        string attendanceId,
        [FromBody] UpdateAttendanceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateAttendanceAsync(attendanceId, dto);
        }, $"修改考勤失敗: {attendanceId}");
    }

    /// <summary>
    /// 刪除考勤
    /// </summary>
    [HttpDelete("{attendanceId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAttendance(string attendanceId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteAttendanceAsync(attendanceId);
        }, $"刪除考勤失敗: {attendanceId}");
    }

    /// <summary>
    /// 上班打卡
    /// </summary>
    [HttpPost("check-in")]
    public async Task<ActionResult<ApiResponse<object>>> CheckIn(
        [FromBody] CheckInOutDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CheckInAsync(dto);
        }, "上班打卡失敗");
    }

    /// <summary>
    /// 下班打卡
    /// </summary>
    [HttpPost("check-out")]
    public async Task<ActionResult<ApiResponse<object>>> CheckOut(
        [FromBody] CheckInOutDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CheckOutAsync(dto);
        }, "下班打卡失敗");
    }

    /// <summary>
    /// 補打卡
    /// </summary>
    [HttpPost("supplement")]
    public async Task<ActionResult<ApiResponse<string>>> SupplementAttendance(
        [FromBody] SupplementAttendanceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SupplementAttendanceAsync(dto);
            return result;
        }, "補打卡失敗");
    }
}

