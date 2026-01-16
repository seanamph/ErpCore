using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 工務維修統計報表(其他) DTO (SYSA1024)
/// </summary>
public class SYSA1024ReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string ReportName { get; set; } = "工務維修統計報表(其他)";
    public string? BelongStatus { get; set; } // 費用負擔
    public string ApplyDateB { get; set; } = string.Empty; // 日統計表起
    public string ApplyDateE { get; set; } = string.Empty; // 日統計表迄
    public string? BelongOrg { get; set; } // 費用歸屬單位
    public string? MaintainEmp { get; set; } // 維保人員
    public string? ApplyType { get; set; } // 請修類別
    public string? OtherCondition1 { get; set; } // 其他查詢條件1
    public string? OtherCondition2 { get; set; } // 其他查詢條件2
    public int RequestCount { get; set; } // 申請件數
    public decimal TotalAmount { get; set; } // 總金額
}

/// <summary>
/// 工務維修統計報表(其他)查詢 DTO (SYSA1024)
/// </summary>
public class SYSA1024QueryDto : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BelongStatus { get; set; } // 費用負擔
    public string? ApplyDateB { get; set; } // 日統計表起 (YYYY-MM-DD)
    public string? ApplyDateE { get; set; } // 日統計表迄 (YYYY-MM-DD)
    public string? BelongOrg { get; set; } // 費用歸屬單位
    public string? MaintainEmp { get; set; } // 維保人員
    public string? ApplyType { get; set; } // 請修類別
    public string? OtherCondition1 { get; set; } // 其他查詢條件1
    public string? OtherCondition2 { get; set; } // 其他查詢條件2
}
