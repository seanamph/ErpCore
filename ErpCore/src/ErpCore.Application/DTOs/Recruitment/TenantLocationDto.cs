namespace ErpCore.Application.DTOs.Recruitment;

/// <summary>
/// 租戶位置 DTO (SYSC999)
/// </summary>
public class TenantLocationDto
{
    public long TKey { get; set; }
    public long AgmTKey { get; set; }
    public string LocationId { get; set; } = string.Empty;
    public string? AreaId { get; set; }
    public string? FloorId { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 租戶位置查詢 DTO
/// </summary>
public class TenantLocationQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public long? AgmTKey { get; set; }
    public string? LocationId { get; set; }
    public string? AreaId { get; set; }
    public string? FloorId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增租戶位置 DTO
/// </summary>
public class CreateTenantLocationDto
{
    public long AgmTKey { get; set; }
    public string LocationId { get; set; } = string.Empty;
    public string? AreaId { get; set; }
    public string? FloorId { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改租戶位置 DTO
/// </summary>
public class UpdateTenantLocationDto
{
    public string LocationId { get; set; } = string.Empty;
    public string? AreaId { get; set; }
    public string? FloorId { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 租戶位置主鍵 DTO
/// </summary>
public class TenantLocationKeyDto
{
    public long TKey { get; set; }
}

/// <summary>
/// 批次刪除租戶位置 DTO
/// </summary>
public class BatchDeleteTenantLocationDto
{
    public List<long> Items { get; set; } = new();
}

