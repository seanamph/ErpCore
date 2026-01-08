namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 業務報表管理 DTO (SYSL145)
/// </summary>
public class BusinessReportManagementDto
{
    public long TKey { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? TypeName { get; set; }
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 業務報表管理查詢 DTO (SYSL145)
/// </summary>
public class BusinessReportManagementQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SiteId { get; set; }
    public string? Type { get; set; }
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增業務報表管理 DTO (SYSL145)
/// </summary>
public class CreateBusinessReportManagementDto
{
    public string SiteId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改業務報表管理 DTO (SYSL145)
/// </summary>
public class UpdateBusinessReportManagementDto
{
    public string SiteId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 檢查重複資料 DTO (SYSL145)
/// </summary>
public class CheckDuplicateDto
{
    public string SiteId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public long? ExcludeTKey { get; set; }
}

/// <summary>
/// 檢查重複資料結果 DTO (SYSL145)
/// </summary>
public class CheckDuplicateResultDto
{
    public bool IsDuplicate { get; set; }
    public BusinessReportManagementDto? ExistingRecord { get; set; }
}

/// <summary>
/// 批次刪除 DTO (SYSL145)
/// </summary>
public class BatchDeleteDto
{
    public List<long> TKeys { get; set; } = new();
}

