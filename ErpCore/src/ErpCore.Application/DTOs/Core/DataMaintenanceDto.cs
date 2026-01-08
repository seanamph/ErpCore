namespace ErpCore.Application.DTOs.Core;

/// <summary>
/// 資料瀏覽設定 DTO (IMS30_FB)
/// </summary>
public class DataBrowseConfigDto
{
    public long ConfigId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? DisplayFields { get; set; }
    public string? FilterFields { get; set; }
    public string? SortFields { get; set; }
    public int PageSize { get; set; } = 20;
    public string? DefaultSort { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 資料瀏覽查詢 DTO
/// </summary>
public class DataBrowseQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public Dictionary<string, object>? Filters { get; set; }
}

/// <summary>
/// 新增資料瀏覽設定 DTO
/// </summary>
public class CreateDataBrowseConfigDto
{
    public string ModuleCode { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? DisplayFields { get; set; }
    public string? FilterFields { get; set; }
    public string? SortFields { get; set; }
    public int PageSize { get; set; } = 20;
    public string? DefaultSort { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 修改資料瀏覽設定 DTO
/// </summary>
public class UpdateDataBrowseConfigDto
{
    public string? DisplayFields { get; set; }
    public string? FilterFields { get; set; }
    public string? SortFields { get; set; }
    public int? PageSize { get; set; }
    public string? DefaultSort { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 資料新增設定 DTO (IMS30_FI)
/// </summary>
public class DataInsertConfigDto
{
    public long ConfigId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? FormFields { get; set; }
    public string? DefaultValues { get; set; }
    public string? ValidationRules { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 新增資料新增設定 DTO
/// </summary>
public class CreateDataInsertConfigDto
{
    public string ModuleCode { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? FormFields { get; set; }
    public string? DefaultValues { get; set; }
    public string? ValidationRules { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 資料查詢設定 DTO (IMS30_FQ)
/// </summary>
public class DataQueryConfigDto
{
    public long ConfigId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? QueryFields { get; set; }
    public string? DisplayFields { get; set; }
    public string? SortFields { get; set; }
    public string? DefaultQuery { get; set; }
    public int PageSize { get; set; } = 20;
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 儲存的查詢條件 DTO
/// </summary>
public class SavedQueryDto
{
    public long QueryId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string QueryName { get; set; } = string.Empty;
    public string QueryConditions { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 資料排序設定 DTO (IMS30_FS)
/// </summary>
public class DataSortConfigDto
{
    public long ConfigId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? SortFields { get; set; }
    public string? DefaultSort { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 排序規則 DTO
/// </summary>
public class SortRuleDto
{
    public string Field { get; set; } = string.Empty;
    public string Order { get; set; } = "ASC";
}

/// <summary>
/// 儲存的排序規則 DTO
/// </summary>
public class SavedSortDto
{
    public long SortId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string SortName { get; set; } = string.Empty;
    public string SortRules { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 資料修改設定 DTO (IMS30_FU)
/// </summary>
public class DataUpdateConfigDto
{
    public long ConfigId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string? FormFields { get; set; }
    public string? ReadOnlyFields { get; set; }
    public string? ValidationRules { get; set; }
    public bool UseOptimisticLock { get; set; } = true;
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 資料列印設定 DTO (IMS30_PR)
/// </summary>
public class DataPrintConfigDto
{
    public long ConfigId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string? TemplatePath { get; set; }
    public string TemplateType { get; set; } = string.Empty;
    public string? PrintFields { get; set; }
    public string? PrintSettings { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

