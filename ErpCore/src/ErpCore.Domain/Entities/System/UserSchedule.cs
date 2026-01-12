namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者排程實體 (SYS0116)
/// </summary>
public class UserSchedule
{
    /// <summary>
    /// 排程編號
    /// </summary>
    public Guid ScheduleId { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 排程執行時間
    /// </summary>
    public DateTime ScheduleDate { get; set; }

    /// <summary>
    /// 排程類型 (PASSWORD_RESET, USER_UPDATE, STATUS_CHANGE)
    /// </summary>
    public string ScheduleType { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (PENDING, EXECUTING, COMPLETED, CANCELLED, FAILED)
    /// </summary>
    public string Status { get; set; } = "PENDING";

    /// <summary>
    /// 排程資料 (JSON格式)
    /// </summary>
    public string? ScheduleData { get; set; }

    /// <summary>
    /// 執行結果
    /// </summary>
    public string? ExecuteResult { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 實際執行時間
    /// </summary>
    public DateTime? ExecutedAt { get; set; }

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
