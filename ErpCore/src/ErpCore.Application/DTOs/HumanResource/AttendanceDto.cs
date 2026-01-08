namespace ErpCore.Application.DTOs.HumanResource;

/// <summary>
/// 考勤資料 DTO
/// </summary>
public class AttendanceDto
{
    public string AttendanceId { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public decimal WorkHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public string? LeaveType { get; set; }
    public decimal LeaveHours { get; set; }
    public string Status { get; set; } = "N";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 考勤查詢 DTO
/// </summary>
public class AttendanceQueryDto
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

/// <summary>
/// 新增考勤 DTO
/// </summary>
public class CreateAttendanceDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public decimal WorkHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public string? LeaveType { get; set; }
    public decimal LeaveHours { get; set; }
    public string Status { get; set; } = "N";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改考勤 DTO
/// </summary>
public class UpdateAttendanceDto
{
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public decimal WorkHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public string? LeaveType { get; set; }
    public decimal LeaveHours { get; set; }
    public string Status { get; set; } = "N";
    public string? Notes { get; set; }
}

/// <summary>
/// 打卡 DTO
/// </summary>
public class CheckInOutDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime CheckTime { get; set; }
    public string? Location { get; set; }
}

/// <summary>
/// 補打卡 DTO
/// </summary>
public class SupplementAttendanceDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Reason { get; set; }
}

