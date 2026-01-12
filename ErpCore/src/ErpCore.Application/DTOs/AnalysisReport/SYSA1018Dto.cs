using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 工務維修件數統計報表 DTO (SYSA1018)
/// </summary>
public class SYSA1018ReportDto
{
    public string OrgId { get; set; } = string.Empty;
    public string? OrgName { get; set; }
    public string ReportName { get; set; } = "工務維修件數統計表";
    public string YearMonth { get; set; } = string.Empty;
    public string? MaintenanceType { get; set; }
    public string MaintenanceStatus { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalCount { get; set; }
}

/// <summary>
/// 工務維修件數統計報表查詢 DTO (SYSA1018)
/// </summary>
public class SYSA1018QueryDto : PagedQuery
{
    public string? OrgId { get; set; }
    public string? YearMonth { get; set; } // YYYY-MM
    public string? FilterType { get; set; } // 篩選類型 (全部、待處理、處理中、已完成、已取消)
}
