namespace ErpCore.Application.DTOs.Lease;

/// <summary>
/// 租賃擴展 DTO (SYS8A10-SYS8A45)
/// </summary>
public class LeaseExtensionDto
{
    public long TKey { get; set; }
    public string ExtensionId { get; set; } = string.Empty;
    public string LeaseId { get; set; } = string.Empty;
    public string ExtensionType { get; set; } = string.Empty;
    public string? ExtensionTypeName { get; set; }
    public string? ExtensionName { get; set; }
    public string? ExtensionValue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "A";
    public string? StatusName { get; set; }
    public int? SeqNo { get; set; } = 0;
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<LeaseExtensionDetailDto>? Details { get; set; }
}

/// <summary>
/// 租賃擴展明細 DTO
/// </summary>
public class LeaseExtensionDetailDto
{
    public long TKey { get; set; }
    public string ExtensionId { get; set; } = string.Empty;
    public int LineNum { get; set; }
    public string? FieldName { get; set; }
    public string? FieldValue { get; set; }
    public string? FieldType { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢租賃擴展 DTO
/// </summary>
public class LeaseExtensionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ExtensionId { get; set; }
    public string? LeaseId { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增租賃擴展 DTO
/// </summary>
public class CreateLeaseExtensionDto
{
    public string ExtensionId { get; set; } = string.Empty;
    public string LeaseId { get; set; } = string.Empty;
    public string ExtensionType { get; set; } = string.Empty;
    public string? ExtensionName { get; set; }
    public string? ExtensionValue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "A";
    public int? SeqNo { get; set; } = 0;
    public string? Memo { get; set; }
    public List<LeaseExtensionDetailDto>? Details { get; set; }
}

/// <summary>
/// 修改租賃擴展 DTO
/// </summary>
public class UpdateLeaseExtensionDto
{
    public string LeaseId { get; set; } = string.Empty;
    public string ExtensionType { get; set; } = string.Empty;
    public string? ExtensionName { get; set; }
    public string? ExtensionValue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "A";
    public int? SeqNo { get; set; } = 0;
    public string? Memo { get; set; }
    public List<LeaseExtensionDetailDto>? Details { get; set; }
}

/// <summary>
/// 更新租賃擴展狀態 DTO
/// </summary>
public class UpdateLeaseExtensionStatusDto
{
    public string Status { get; set; } = "A";
}

/// <summary>
/// 批次刪除租賃擴展 DTO
/// </summary>
public class BatchDeleteLeaseExtensionDto
{
    public List<string> ExtensionIds { get; set; } = new();
}

