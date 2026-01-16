namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 業務報表列印 DTO (SYSL150)
/// </summary>
public class BusinessReportPrintDto
{
    public long TKey { get; set; }
    public int GiveYear { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string EmpId { get; set; } = string.Empty;
    public string? EmpName { get; set; }
    public decimal? Qty { get; set; }
    public string Status { get; set; } = "P";
    public string? StatusName { get; set; }
    public string? Verifier { get; set; }
    public string? VerifierName { get; set; }
    public DateTime? VerifyDate { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 業務報表列印查詢 DTO (SYSL150)
/// </summary>
public class BusinessReportPrintQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public int? GiveYear { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? EmpId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增業務報表列印 DTO (SYSL150)
/// </summary>
public class CreateBusinessReportPrintDto
{
    public int GiveYear { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string EmpId { get; set; } = string.Empty;
    public string? EmpName { get; set; }
    public decimal? Qty { get; set; }
    public string Status { get; set; } = "P";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改業務報表列印 DTO (SYSL150)
/// </summary>
public class UpdateBusinessReportPrintDto
{
    public int GiveYear { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string EmpId { get; set; } = string.Empty;
    public string? EmpName { get; set; }
    public decimal? Qty { get; set; }
    public string Status { get; set; } = "P";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次審核 DTO (SYSL150)
/// </summary>
public class BatchAuditDto
{
    public List<long> TKeys { get; set; } = new();
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次審核結果 DTO (SYSL150)
/// </summary>
public class BatchAuditResultDto
{
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
}

/// <summary>
/// 複製下一年度資料 DTO (SYSL150)
/// </summary>
public class CopyNextYearDto
{
    public int SourceYear { get; set; }
    public int TargetYear { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 複製下一年度資料結果 DTO (SYSL150)
/// </summary>
public class CopyNextYearResultDto
{
    public int CopiedCount { get; set; }
}

/// <summary>
/// 計算數量 DTO (SYSL150)
/// </summary>
public class CalculateQtyDto
{
    public long TKey { get; set; }
    public Dictionary<string, object>? CalculationRules { get; set; }
}

/// <summary>
/// 計算數量結果 DTO (SYSL150)
/// </summary>
public class CalculateQtyResultDto
{
    public decimal Qty { get; set; }
}
