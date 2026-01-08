namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者權限代理資料傳輸物件
/// </summary>
public class UserAgentDto
{
    public Guid AgentId { get; set; }
    public string PrincipalUserId { get; set; } = string.Empty;
    public string? PrincipalUserName { get; set; }
    public string AgentUserId { get; set; } = string.Empty;
    public string? AgentUserName { get; set; }
    public DateTime BeginTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 使用者權限代理查詢 DTO
/// </summary>
public class UserAgentQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "DESC";
    public string? PrincipalUserId { get; set; }
    public string? AgentUserId { get; set; }
    public string? Status { get; set; }
    public DateTime? BeginTimeFrom { get; set; }
    public DateTime? BeginTimeTo { get; set; }
    public DateTime? EndTimeFrom { get; set; }
    public DateTime? EndTimeTo { get; set; }
}

/// <summary>
/// 新增使用者權限代理 DTO
/// </summary>
public class CreateUserAgentDto
{
    public string PrincipalUserId { get; set; } = string.Empty;
    public string AgentUserId { get; set; } = string.Empty;
    public DateTime BeginTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改使用者權限代理 DTO
/// </summary>
public class UpdateUserAgentDto
{
    public string PrincipalUserId { get; set; } = string.Empty;
    public string AgentUserId { get; set; } = string.Empty;
    public DateTime BeginTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除使用者權限代理 DTO
/// </summary>
public class BatchDeleteUserAgentDto
{
    public List<Guid> AgentIds { get; set; } = new();
}

/// <summary>
/// 檢查代理權限 DTO
/// </summary>
public class CheckAgentPermissionDto
{
    public string AgentUserId { get; set; } = string.Empty;
    public string PrincipalUserId { get; set; } = string.Empty;
    public DateTime? CheckTime { get; set; }
}

/// <summary>
/// 檢查代理權限回應 DTO
/// </summary>
public class CheckAgentPermissionResultDto
{
    public bool HasPermission { get; set; }
    public Guid? AgentId { get; set; }
    public DateTime? BeginTime { get; set; }
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateUserAgentStatusDto
{
    public string Status { get; set; } = "A";
}

