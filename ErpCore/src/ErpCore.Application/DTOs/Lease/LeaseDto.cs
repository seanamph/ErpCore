namespace ErpCore.Application.DTOs.Lease;

/// <summary>
/// 租賃 DTO (SYS8110-SYS8220)
/// </summary>
public class LeaseDto
{
    public long TKey { get; set; }
    public string LeaseId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string? TenantName { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? FloorId { get; set; }
    public string? LocationId { get; set; }
    public DateTime LeaseDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "D";
    public string? StatusName { get; set; }
    public decimal? MonthlyRent { get; set; } = 0;
    public decimal? TotalRent { get; set; } = 0;
    public decimal? Deposit { get; set; } = 0;
    public string? CurrencyId { get; set; } = "TWD";
    public string? PaymentMethod { get; set; }
    public int? PaymentDay { get; set; }
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立租賃 DTO
/// </summary>
public class CreateLeaseDto
{
    public string LeaseId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string? TenantName { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? FloorId { get; set; }
    public string? LocationId { get; set; }
    public DateTime LeaseDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "D";
    public decimal? MonthlyRent { get; set; } = 0;
    public decimal? TotalRent { get; set; } = 0;
    public decimal? Deposit { get; set; } = 0;
    public string? CurrencyId { get; set; } = "TWD";
    public string? PaymentMethod { get; set; }
    public int? PaymentDay { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改租賃 DTO
/// </summary>
public class UpdateLeaseDto
{
    public string TenantId { get; set; } = string.Empty;
    public string? TenantName { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? FloorId { get; set; }
    public string? LocationId { get; set; }
    public DateTime LeaseDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "D";
    public decimal? MonthlyRent { get; set; } = 0;
    public decimal? TotalRent { get; set; } = 0;
    public decimal? Deposit { get; set; } = 0;
    public string? CurrencyId { get; set; } = "TWD";
    public string? PaymentMethod { get; set; }
    public int? PaymentDay { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢租賃 DTO
/// </summary>
public class LeaseQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LeaseId { get; set; }
    public string? TenantId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
}

/// <summary>
/// 租賃結果 DTO
/// </summary>
public class LeaseResultDto
{
    public long TKey { get; set; }
    public string LeaseId { get; set; } = string.Empty;
}

/// <summary>
/// 更新租賃狀態 DTO
/// </summary>
public class UpdateLeaseStatusDto
{
    public string Status { get; set; } = "D";
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteLeaseDto
{
    public List<string> LeaseIds { get; set; } = new();
}

