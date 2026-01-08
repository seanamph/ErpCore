namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 部別 DTO
/// </summary>
public class DepartmentDto
{
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
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
/// 部別查詢 DTO
/// </summary>
public class DepartmentQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? DeptId { get; set; }
    public string? DeptName { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增部別 DTO
/// </summary>
public class CreateDepartmentDto
{
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改部別 DTO
/// </summary>
public class UpdateDepartmentDto
{
    public string DeptName { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除部別 DTO
/// </summary>
public class BatchDeleteDepartmentDto
{
    public List<string> DeptIds { get; set; } = new();
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateDepartmentStatusDto
{
    public string Status { get; set; } = "A";
}

