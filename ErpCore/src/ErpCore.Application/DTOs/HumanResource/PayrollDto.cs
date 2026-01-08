namespace ErpCore.Application.DTOs.HumanResource;

/// <summary>
/// 薪資資料 DTO (SYSH210)
/// </summary>
public class PayrollDto
{
    public string PayrollId { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public int PayrollYear { get; set; }
    public int PayrollMonth { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Allowance { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deduction { get; set; }
    public decimal TotalSalary { get; set; }
    public string Status { get; set; } = "D";
    public DateTime? PayDate { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 薪資查詢 DTO
/// </summary>
public class PayrollQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? EmployeeId { get; set; }
    public int? PayrollYear { get; set; }
    public int? PayrollMonth { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增薪資 DTO
/// </summary>
public class CreatePayrollDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public int PayrollYear { get; set; }
    public int PayrollMonth { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Allowance { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deduction { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改薪資 DTO
/// </summary>
public class UpdatePayrollDto
{
    public decimal BaseSalary { get; set; }
    public decimal Allowance { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deduction { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 計算薪資 DTO
/// </summary>
public class CalculatePayrollDto
{
    public decimal BaseSalary { get; set; }
    public decimal Allowance { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deduction { get; set; }
}

/// <summary>
/// 薪資計算結果 DTO
/// </summary>
public class PayrollCalculationResultDto
{
    public decimal TotalSalary { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalDeduction { get; set; }
}

