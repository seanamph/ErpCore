namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 銷退卡 DTO (SYSL310)
/// </summary>
public class ReturnCardDto
{
    public long TKey { get; set; }
    public Guid Uuid { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public int CardYear { get; set; }
    public int CardMonth { get; set; }
    public string? CardType { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 銷退卡查詢 DTO (SYSL310)
/// </summary>
public class ReturnCardQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public int? CardYear { get; set; }
    public int? CardMonth { get; set; }
}

/// <summary>
/// 新增銷退卡 DTO (SYSL310)
/// </summary>
public class CreateReturnCardDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public int CardYear { get; set; }
    public int CardMonth { get; set; }
    public string? CardType { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改銷退卡 DTO (SYSL310)
/// </summary>
public class UpdateReturnCardDto
{
    public string? OrgId { get; set; }
    public int? CardYear { get; set; }
    public int? CardMonth { get; set; }
    public string? CardType { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

