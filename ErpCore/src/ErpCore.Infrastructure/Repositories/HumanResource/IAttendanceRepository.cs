using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.HumanResource;

/// <summary>
/// 考勤 Repository 介面
/// </summary>
public interface IAttendanceRepository
{
    /// <summary>
    /// 根據考勤編號查詢考勤
    /// </summary>
    Task<Attendance?> GetByIdAsync(string attendanceId);

    /// <summary>
    /// 根據員工編號、日期查詢考勤
    /// </summary>
    Task<Attendance?> GetByEmployeeDateAsync(string employeeId, DateTime date);

    /// <summary>
    /// 查詢考勤列表（分頁）
    /// </summary>
    Task<PagedResult<Attendance>> QueryAsync(AttendanceQuery query);

    /// <summary>
    /// 新增考勤
    /// </summary>
    Task<Attendance> CreateAsync(Attendance attendance);

    /// <summary>
    /// 修改考勤
    /// </summary>
    Task<Attendance> UpdateAsync(Attendance attendance);

    /// <summary>
    /// 刪除考勤
    /// </summary>
    Task DeleteAsync(string attendanceId);

    /// <summary>
    /// 檢查員工日期組合是否存在
    /// </summary>
    Task<bool> ExistsAsync(string employeeId, DateTime date);
}

/// <summary>
/// 考勤查詢條件
/// </summary>
public class AttendanceQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? EmployeeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}

