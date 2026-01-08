namespace ErpCore.Application.DTOs.HumanResource;

/// <summary>
/// 員工 DTO (SYSH110)
/// </summary>
public class EmployeeDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string? IdNumber { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? ResignDate { get; set; }
    public string Status { get; set; } = "A";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 員工查詢 DTO
/// </summary>
public class EmployeeQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增員工 DTO
/// </summary>
public class CreateEmployeeDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string? IdNumber { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public DateTime? HireDate { get; set; }
    public string Status { get; set; } = "A";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改員工 DTO
/// </summary>
public class UpdateEmployeeDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string? IdNumber { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? ResignDate { get; set; }
    public string Status { get; set; } = "A";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Notes { get; set; }
}

