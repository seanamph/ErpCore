namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者排程資料傳輸物件 (SYS0116)
/// </summary>
public class UserScheduleDto
{
    public Guid ScheduleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime ScheduleDate { get; set; }
    public string ScheduleType { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public string? ScheduleData { get; set; }
    public string? ExecuteResult { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 使用者排程查詢 DTO
/// </summary>
public class UserScheduleQueryDto
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

/// <summary>
/// 新增使用者排程 DTO
/// </summary>
public class CreateUserScheduleDto
{
    public string UserId { get; set; } = string.Empty;
    public DateTime ScheduleDate { get; set; }
    public string ScheduleType { get; set; } = string.Empty;
    public object? ScheduleData { get; set; }
}

/// <summary>
/// 修改使用者排程 DTO
/// </summary>
public class UpdateUserScheduleDto
{
    public DateTime? ScheduleDate { get; set; }
    public string? ScheduleType { get; set; }
    public object? ScheduleData { get; set; }
}
