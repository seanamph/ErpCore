namespace ErpCore.Application.DTOs.UiComponent;

/// <summary>
/// UI組件 DTO
/// </summary>
public class UIComponentDto
{
    public long ComponentId { get; set; }
    public string ComponentCode { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentType { get; set; } = string.Empty;
    public string ComponentVersion { get; set; } = "V1";
    public string? ConfigJson { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 新增UI組件 DTO
/// </summary>
public class CreateUIComponentDto
{
    public string ComponentCode { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentType { get; set; } = string.Empty;
    public string ComponentVersion { get; set; } = "V1";
    public string? ConfigJson { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 修改UI組件 DTO
/// </summary>
public class UpdateUIComponentDto
{
    public string ComponentName { get; set; } = string.Empty;
    public string? ConfigJson { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// UI組件查詢 DTO
/// </summary>
public class UIComponentQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ComponentCode { get; set; }
    public string? ComponentType { get; set; }
    public string? ComponentVersion { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// UI組件使用記錄 DTO
/// </summary>
public class UIComponentUsageDto
{
    public long UsageId { get; set; }
    public long ComponentId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string? ModuleName { get; set; }
    public int UsageCount { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// UI組件使用統計 DTO
/// </summary>
public class UIComponentUsageStatsDto
{
    public long ComponentId { get; set; }
    public string ComponentCode { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentType { get; set; } = string.Empty;
    public string ComponentVersion { get; set; } = string.Empty;
    public int TotalUsageCount { get; set; }
    public int UsedModuleCount { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime? FirstUsedAt { get; set; }
}

/// <summary>
/// UI組件使用查詢 DTO
/// </summary>
public class UIComponentUsageQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ComponentCode { get; set; }
    public string? ComponentType { get; set; }
    public string? ModuleCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

