namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 組別 DTO
/// </summary>
public class GroupDto
{
    public string GroupId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string? DeptId { get; set; }
    public string? DeptName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 組別查詢 DTO
/// </summary>
public class GroupQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }
    public string? DeptId { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增組別 DTO
/// </summary>
public class CreateGroupDto
{
    public string GroupId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string? DeptId { get; set; }
    public string? OrgId { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改組別 DTO
/// </summary>
public class UpdateGroupDto
{
    public string GroupName { get; set; } = string.Empty;
    public string? DeptId { get; set; }
    public string? OrgId { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除組別 DTO
/// </summary>
public class BatchDeleteGroupDto
{
    public List<string> GroupIds { get; set; } = new();
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateGroupStatusDto
{
    public string Status { get; set; } = "A";
}
