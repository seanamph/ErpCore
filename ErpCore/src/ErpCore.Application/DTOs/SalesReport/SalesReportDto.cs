using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.SalesReport;

/// <summary>
/// 銷售報表 DTO (SYS1000 - 銷售報表模組系列)
/// </summary>
public class SalesReportDto
{
    public string ReportId { get; set; } = string.Empty;
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string? ReportType { get; set; }
    public string? ShopId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ReportData { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 銷售報表查詢 DTO
/// </summary>
public class SalesReportQueryDto : PagedQuery
{
    public string? ReportCode { get; set; }
    public string? ShopId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增銷售報表 DTO
/// </summary>
public class CreateSalesReportDto
{
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string? ReportType { get; set; }
    public string? ShopId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ReportData { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改銷售報表 DTO
/// </summary>
public class UpdateSalesReportDto
{
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string? ReportType { get; set; }
    public string? ShopId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ReportData { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 生成報表 DTO
/// </summary>
public class GenerateReportDto
{
    public string ReportCode { get; set; } = string.Empty;
    public string? ShopId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// 生成報表回應 DTO
/// </summary>
public class GenerateReportResponseDto
{
    public string ReportId { get; set; } = string.Empty;
    public string ReportUrl { get; set; } = string.Empty;
}

