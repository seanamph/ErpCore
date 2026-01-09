namespace ErpCore.Application.DTOs.SystemExtensionE;

/// <summary>
/// 員工 DTO (SYSPE10-SYSPE11 - 員工資料維護)
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
/// 建立員工 DTO
/// </summary>
public class CreateEmployeeDto
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
}

/// <summary>
/// 修改員工 DTO
/// </summary>
public class UpdateEmployeeDto : CreateEmployeeDto
{
}

/// <summary>
/// 人事 DTO (SYSPED0 - 人事資料維護)
/// </summary>
public class PersonnelDto
{
    public string PersonnelId { get; set; } = string.Empty;
    public string PersonnelName { get; set; } = string.Empty;
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
/// 人事查詢 DTO
/// </summary>
public class PersonnelQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PersonnelId { get; set; }
    public string? PersonnelName { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 建立人事 DTO
/// </summary>
public class CreatePersonnelDto
{
    public string PersonnelId { get; set; } = string.Empty;
    public string PersonnelName { get; set; } = string.Empty;
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

/// <summary>
/// 修改人事 DTO
/// </summary>
public class UpdatePersonnelDto : CreatePersonnelDto
{
}

