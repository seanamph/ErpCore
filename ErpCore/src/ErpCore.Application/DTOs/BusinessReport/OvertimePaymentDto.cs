namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 加班發放 DTO (SYSL510)
/// </summary>
public class OvertimePaymentDto
{
    public long Id { get; set; }
    public string PaymentNo { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public string? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal OvertimeAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Draft";
    public string? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 加班發放查詢 DTO (SYSL510)
/// </summary>
public class OvertimePaymentQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PaymentNo { get; set; }
    public string? EmployeeId { get; set; }
    public string? DepartmentId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增加班發放 DTO (SYSL510)
/// </summary>
public class CreateOvertimePaymentDto
{
    public DateTime PaymentDate { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public string? DepartmentId { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal OvertimeAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改加班發放 DTO (SYSL510)
/// </summary>
public class UpdateOvertimePaymentDto
{
    public string? EmployeeName { get; set; }
    public string? DepartmentId { get; set; }
    public decimal? OvertimeHours { get; set; }
    public decimal? OvertimeAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 審核加班發放 DTO (SYSL510)
/// </summary>
public class ApproveOvertimePaymentDto
{
    public string Status { get; set; } = "Approved";
    public string? Notes { get; set; }
}

