using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.HumanResource;

/// <summary>
/// 考勤服務介面
/// </summary>
public interface IAttendanceService
{
    /// <summary>
    /// 查詢考勤列表
    /// </summary>
    Task<PagedResult<AttendanceDto>> GetAttendancesAsync(AttendanceQueryDto query);

    /// <summary>
    /// 根據考勤編號查詢考勤
    /// </summary>
    Task<AttendanceDto> GetAttendanceByIdAsync(string attendanceId);

    /// <summary>
    /// 新增考勤
    /// </summary>
    Task<string> CreateAttendanceAsync(CreateAttendanceDto dto);

    /// <summary>
    /// 修改考勤
    /// </summary>
    Task UpdateAttendanceAsync(string attendanceId, UpdateAttendanceDto dto);

    /// <summary>
    /// 刪除考勤
    /// </summary>
    Task DeleteAttendanceAsync(string attendanceId);

    /// <summary>
    /// 上班打卡
    /// </summary>
    Task CheckInAsync(CheckInOutDto dto);

    /// <summary>
    /// 下班打卡
    /// </summary>
    Task CheckOutAsync(CheckInOutDto dto);

    /// <summary>
    /// 補打卡
    /// </summary>
    Task<string> SupplementAttendanceAsync(SupplementAttendanceDto dto);
}

