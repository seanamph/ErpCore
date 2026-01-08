using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 加班發放報表 Repository 介面 (SYSL510)
/// </summary>
public interface IOvertimePaymentReportRepository
{
    /// <summary>
    /// 查詢加班發放報表資料
    /// </summary>
    Task<PagedResult<OvertimePaymentReportEntity>> QueryReportAsync(OvertimePaymentReportQuery query);

    /// <summary>
    /// 查詢加班發放報表統計資訊
    /// </summary>
    Task<OvertimePaymentReportSummary> GetSummaryAsync(OvertimePaymentReportQuery query);
}

/// <summary>
/// 加班發放報表查詢條件
/// </summary>
public class OvertimePaymentReportQuery
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
/// 加班發放報表實體
/// </summary>
public class OvertimePaymentReportEntity
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
/// 加班發放報表統計資訊
/// </summary>
public class OvertimePaymentReportSummary
{
    public int TotalCount { get; set; }
    public decimal TotalOvertimeHours { get; set; }
    public decimal TotalOvertimeAmount { get; set; }
}

