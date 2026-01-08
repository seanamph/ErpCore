namespace ErpCore.Domain.Entities.HumanResource;

/// <summary>
/// 考勤資料實體
/// </summary>
public class Attendance
{
    /// <summary>
    /// 考勤編號
    /// </summary>
    public string AttendanceId { get; set; } = string.Empty;

    /// <summary>
    /// 員工編號
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// 考勤日期
    /// </summary>
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// 上班時間
    /// </summary>
    public DateTime? CheckInTime { get; set; }

    /// <summary>
    /// 下班時間
    /// </summary>
    public DateTime? CheckOutTime { get; set; }

    /// <summary>
    /// 工作時數
    /// </summary>
    public decimal WorkHours { get; set; }

    /// <summary>
    /// 加班時數
    /// </summary>
    public decimal OvertimeHours { get; set; }

    /// <summary>
    /// 請假類型
    /// </summary>
    public string? LeaveType { get; set; }

    /// <summary>
    /// 請假時數
    /// </summary>
    public decimal LeaveHours { get; set; }

    /// <summary>
    /// 狀態 (N:正常, L:請假, A:曠職, O:加班)
    /// </summary>
    public string Status { get; set; } = "N";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

