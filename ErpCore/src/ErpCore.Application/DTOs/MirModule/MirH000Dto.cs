namespace ErpCore.Application.DTOs.MirModule;

/// <summary>
/// MIRH000 人事 DTO
/// </summary>
public class MirH000PersonnelDto
{
    public long TKey { get; set; }
    public string PersonnelId { get; set; } = string.Empty;
    public string PersonnelName { get; set; } = string.Empty;
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? ResignDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// MIRH000 人事查詢 DTO
/// </summary>
public class MirH000PersonnelQueryDto
{
    public string? PersonnelId { get; set; }
    public string? PersonnelName { get; set; }
    public string? DepartmentId { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// MIRH000 人事建立 DTO
/// </summary>
public class CreateMirH000PersonnelDto
{
    public string PersonnelId { get; set; } = string.Empty;
    public string PersonnelName { get; set; } = string.Empty;
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? ResignDate { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// MIRH000 人事修改 DTO
/// </summary>
public class UpdateMirH000PersonnelDto
{
    public string PersonnelName { get; set; } = string.Empty;
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? ResignDate { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// MIRH000 薪資 DTO
/// </summary>
public class MirH000SalaryDto
{
    public long TKey { get; set; }
    public string SalaryId { get; set; } = string.Empty;
    public string PersonnelId { get; set; } = string.Empty;
    public string SalaryMonth { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal TotalSalary { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// MIRH000 薪資查詢 DTO
/// </summary>
public class MirH000SalaryQueryDto
{
    public string? SalaryId { get; set; }
    public string? PersonnelId { get; set; }
    public string? SalaryMonth { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// MIRH000 薪資建立 DTO
/// </summary>
public class CreateMirH000SalaryDto
{
    public string SalaryId { get; set; } = string.Empty;
    public string PersonnelId { get; set; } = string.Empty;
    public string SalaryMonth { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal TotalSalary { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// MIRH000 薪資修改 DTO
/// </summary>
public class UpdateMirH000SalaryDto
{
    public string PersonnelId { get; set; } = string.Empty;
    public string SalaryMonth { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal TotalSalary { get; set; }
    public string Status { get; set; } = "A";
}

