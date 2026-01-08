namespace ErpCore.Application.DTOs.Extension;

/// <summary>
/// 擴展功能 DTO
/// </summary>
public class ExtensionFunctionDto
{
    public long TKey { get; set; }
    public string ExtensionId { get; set; } = string.Empty;
    public string ExtensionName { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string? ExtensionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Version { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 擴展功能查詢 DTO
/// </summary>
public class ExtensionFunctionQueryDto
{
    public string? ExtensionId { get; set; }
    public string? ExtensionName { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立擴展功能 DTO
/// </summary>
public class CreateExtensionFunctionDto
{
    public string ExtensionId { get; set; } = string.Empty;
    public string ExtensionName { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string? ExtensionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Version { get; set; }
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 修改擴展功能 DTO
/// </summary>
public class UpdateExtensionFunctionDto
{
    public string ExtensionName { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string? ExtensionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Version { get; set; }
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 批次建立擴展功能 DTO
/// </summary>
public class BatchCreateExtensionFunctionDto
{
    public List<CreateExtensionFunctionDto> Items { get; set; } = new();
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateStatusDto
{
    public string Status { get; set; } = "1";
}

