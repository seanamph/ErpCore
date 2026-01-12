using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者排程服務介面 (SYS0116)
/// </summary>
public interface IUserScheduleService
{
    /// <summary>
    /// 查詢排程列表
    /// </summary>
    Task<PagedResult<UserScheduleDto>> GetSchedulesAsync(UserScheduleQueryDto query);

    /// <summary>
    /// 查詢單筆排程
    /// </summary>
    Task<UserScheduleDto> GetScheduleByIdAsync(Guid scheduleId);

    /// <summary>
    /// 新增排程
    /// </summary>
    Task<Guid> CreateScheduleAsync(CreateUserScheduleDto dto);

    /// <summary>
    /// 修改排程
    /// </summary>
    Task UpdateScheduleAsync(Guid scheduleId, UpdateUserScheduleDto dto);

    /// <summary>
    /// 取消排程
    /// </summary>
    Task CancelScheduleAsync(Guid scheduleId);

    /// <summary>
    /// 執行排程（系統內部）
    /// </summary>
    Task ExecuteScheduleAsync(Guid scheduleId);

    /// <summary>
    /// 查詢待執行的排程
    /// </summary>
    Task<List<UserScheduleDto>> GetPendingSchedulesAsync(DateTime executeTime);
}
