namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 業務報表 DTO (SYSL135)
/// </summary>
public class BusinessReportDto
{
    public long ReportId { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string CardType { get; set; } = string.Empty;
    public string? CardTypeName { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? StoreId { get; set; }
    public string? StoreName { get; set; }
    public string? AgreementId { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string? ActionType { get; set; }
    public string? ActionTypeName { get; set; }
    public DateTime ReportDate { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 業務報表查詢 DTO (SYSL135)
/// </summary>
public class BusinessReportQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SiteId { get; set; }
    public string? CardType { get; set; }
    public string? VendorId { get; set; }
    public string? StoreId { get; set; }
    public string? OrgId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

