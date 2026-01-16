using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 工務維修件數統計表 DTO (SYSA1018)
/// </summary>
public class SYSA1018ReportDto
{
    public string OrgId { get; set; } = string.Empty;
    public string? OrgName { get; set; }
    public string ReportName { get; set; } = "工務維修件數統計表";
    public string YearMonth { get; set; } = string.Empty;
    public string? MaintenanceType { get; set; } // 維修類型 (對應 ApplyType)
    public string? MaintenanceStatus { get; set; } // 維修狀態
    public int ItemCount { get; set; } // 維修件數
    public int TotalCount { get; set; } // 總件數
}

/// <summary>
/// 工務維修件數統計表查詢 DTO (SYSA1018)
/// </summary>
public class SYSA1018QueryDto : PagedQuery
{
    public string? OrgId { get; set; } // 組織單位代號
    public string? YearMonth { get; set; } // 年月 (YYYY-MM)
    public string? FilterType { get; set; } // 篩選類型
}
