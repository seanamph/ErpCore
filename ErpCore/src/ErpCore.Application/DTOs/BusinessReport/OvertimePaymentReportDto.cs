namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 加班發放報表 DTO (SYSL510)
/// </summary>
public class OvertimePaymentReportDto
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
}

/// <summary>
/// 加班發放報表查詢 DTO (SYSL510)
/// </summary>
public class OvertimePaymentReportQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? DepartmentId { get; set; }
    public string? EmployeeId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 加班發放報表結果 DTO (SYSL510)
/// </summary>
public class OvertimePaymentReportResultDto
{
    public List<OvertimePaymentReportDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public OvertimePaymentReportSummaryDto? Summary { get; set; }
}

/// <summary>
/// 加班發放報表統計資訊 DTO (SYSL510)
/// </summary>
public class OvertimePaymentReportSummaryDto
{
    public int TotalCount { get; set; }
    public decimal TotalOvertimeHours { get; set; }
    public decimal TotalOvertimeAmount { get; set; }
}

