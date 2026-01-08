namespace ErpCore.Application.DTOs.Lease;

/// <summary>
/// 租賃處理 DTO (SYS8B50-SYS8B90)
/// </summary>
public class LeaseProcessDto
{
    public long TKey { get; set; }
    public string ProcessId { get; set; } = string.Empty;
    public string LeaseId { get; set; } = string.Empty;
    public string ProcessType { get; set; } = string.Empty;
    public string? ProcessTypeName { get; set; }
    public DateTime ProcessDate { get; set; }
    public string ProcessStatus { get; set; } = "P";
    public string? ProcessStatusName { get; set; }
    public string? ProcessResult { get; set; }
    public string? ProcessResultName { get; set; }
    public string? ProcessUserId { get; set; }
    public string? ProcessUserName { get; set; }
    public string? ProcessMemo { get; set; }
    public string? ApprovalUserId { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovalStatus { get; set; }
    public string? ApprovalStatusName { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<LeaseProcessDetailDto>? Details { get; set; }
    public List<LeaseProcessLogDto>? Logs { get; set; }
}

/// <summary>
/// 租賃處理明細 DTO
/// </summary>
public class LeaseProcessDetailDto
{
    public long TKey { get; set; }
    public string ProcessId { get; set; } = string.Empty;
    public int LineNum { get; set; }
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? FieldType { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 租賃處理日誌 DTO
/// </summary>
public class LeaseProcessLogDto
{
    public long TKey { get; set; }
    public string ProcessId { get; set; } = string.Empty;
    public DateTime LogDate { get; set; }
    public string? LogType { get; set; }
    public string? LogMessage { get; set; }
    public string? LogUserId { get; set; }
}

/// <summary>
/// 查詢租賃處理 DTO
/// </summary>
public class LeaseProcessQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProcessId { get; set; }
    public string? LeaseId { get; set; }
    public string? ProcessType { get; set; }
    public string? ProcessStatus { get; set; }
    public DateTime? ProcessDateFrom { get; set; }
    public DateTime? ProcessDateTo { get; set; }
}

/// <summary>
/// 新增租賃處理 DTO
/// </summary>
public class CreateLeaseProcessDto
{
    public string ProcessId { get; set; } = string.Empty;
    public string LeaseId { get; set; } = string.Empty;
    public string ProcessType { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public string ProcessStatus { get; set; } = "P";
    public string? ProcessMemo { get; set; }
    public List<LeaseProcessDetailDto>? Details { get; set; }
}

/// <summary>
/// 修改租賃處理 DTO
/// </summary>
public class UpdateLeaseProcessDto
{
    public string LeaseId { get; set; } = string.Empty;
    public string ProcessType { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public string ProcessStatus { get; set; } = "P";
    public string? ProcessMemo { get; set; }
    public List<LeaseProcessDetailDto>? Details { get; set; }
}

/// <summary>
/// 更新租賃處理狀態 DTO
/// </summary>
public class UpdateLeaseProcessStatusDto
{
    public string ProcessStatus { get; set; } = "P";
}

/// <summary>
/// 執行租賃處理 DTO
/// </summary>
public class ExecuteLeaseProcessDto
{
    public string ProcessResult { get; set; } = string.Empty;
    public string? ProcessMemo { get; set; }
}

/// <summary>
/// 審核租賃處理 DTO
/// </summary>
public class ApproveLeaseProcessDto
{
    public string ApprovalStatus { get; set; } = string.Empty;
    public string? ApprovalMemo { get; set; }
}

