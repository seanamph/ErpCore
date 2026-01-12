using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者排程 Repository 介面 (SYS0116)
/// </summary>
public interface IUserScheduleRepository
{
    /// <summary>
    /// 根據排程編號查詢
    /// </summary>
    Task<UserSchedule?> GetByIdAsync(Guid scheduleId);

    /// <summary>
    /// 查詢排程列表（分頁）
    /// </summary>
    Task<PagedResult<UserSchedule>> QueryAsync(UserScheduleQuery query);

    /// <summary>
    /// 新增排程
    /// </summary>
    Task<UserSchedule> CreateAsync(UserSchedule schedule);

    /// <summary>
    /// 修改排程
    /// </summary>
    Task<UserSchedule> UpdateAsync(UserSchedule schedule);

    /// <summary>
    /// 刪除排程
    /// </summary>
    Task DeleteAsync(Guid scheduleId);

    /// <summary>
    /// 查詢待執行的排程
    /// </summary>
    Task<List<UserSchedule>> GetPendingSchedulesAsync(DateTime executeTime);

    /// <summary>
    /// 更新排程狀態
    /// </summary>
    Task UpdateStatusAsync(Guid scheduleId, string status, string? errorMessage = null, string? executeResult = null);
}

/// <summary>
/// 使用者排程查詢條件
/// </summary>
public class UserScheduleQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? UserId { get; set; }
    public string? Status { get; set; }
    public string? ScheduleType { get; set; }
    public DateTime? ScheduleDateFrom { get; set; }
    public DateTime? ScheduleDateTo { get; set; }
}
