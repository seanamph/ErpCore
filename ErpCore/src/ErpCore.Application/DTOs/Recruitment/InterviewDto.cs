namespace ErpCore.Application.DTOs.Recruitment;

/// <summary>
/// 訪談 DTO (SYSC222)
/// </summary>
public class InterviewDto
{
    public long InterviewId { get; set; }
    public string ProspectId { get; set; } = string.Empty;
    public string? ProspectName { get; set; }
    public DateTime InterviewDate { get; set; }
    public TimeSpan? InterviewTime { get; set; }
    public string? InterviewType { get; set; }
    public string? Interviewer { get; set; }
    public string? InterviewLocation { get; set; }
    public string? InterviewContent { get; set; }
    public string? InterviewResult { get; set; }
    public string? NextAction { get; set; }
    public DateTime? NextActionDate { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 訪談查詢 DTO
/// </summary>
public class InterviewQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ProspectId { get; set; }
    public DateTime? InterviewDateFrom { get; set; }
    public DateTime? InterviewDateTo { get; set; }
    public string? InterviewResult { get; set; }
    public string? Status { get; set; }
    public string? Interviewer { get; set; }
}

/// <summary>
/// 新增訪談 DTO
/// </summary>
public class CreateInterviewDto
{
    public string ProspectId { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public TimeSpan? InterviewTime { get; set; }
    public string? InterviewType { get; set; }
    public string? Interviewer { get; set; }
    public string? InterviewLocation { get; set; }
    public string? InterviewContent { get; set; }
    public string? InterviewResult { get; set; }
    public string? NextAction { get; set; }
    public DateTime? NextActionDate { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改訪談 DTO
/// </summary>
public class UpdateInterviewDto
{
    public string ProspectId { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public TimeSpan? InterviewTime { get; set; }
    public string? InterviewType { get; set; }
    public string? Interviewer { get; set; }
    public string? InterviewLocation { get; set; }
    public string? InterviewContent { get; set; }
    public string? InterviewResult { get; set; }
    public string? NextAction { get; set; }
    public DateTime? NextActionDate { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除訪談 DTO
/// </summary>
public class BatchDeleteInterviewDto
{
    public List<long> InterviewIds { get; set; } = new();
}

/// <summary>
/// 更新訪談狀態 DTO
/// </summary>
public class UpdateInterviewStatusDto
{
    public string Status { get; set; } = string.Empty;
}

