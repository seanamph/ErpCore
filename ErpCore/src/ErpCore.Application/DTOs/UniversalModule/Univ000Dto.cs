namespace ErpCore.Application.DTOs.UniversalModule;

/// <summary>
/// 通用模組資料 DTO (UNIV000系列)
/// </summary>
public class Univ000Dto
{
    public long TKey { get; set; }
    public string DataId { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string? DataType { get; set; }
    public string? DataValue { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 通用模組資料查詢 DTO
/// </summary>
public class Univ000QueryDto
{
    public string? DataId { get; set; }
    public string? DataName { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立通用模組資料 DTO
/// </summary>
public class CreateUniv000Dto
{
    public string DataId { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string? DataType { get; set; }
    public string? DataValue { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改通用模組資料 DTO
/// </summary>
public class UpdateUniv000Dto
{
    public string DataName { get; set; } = string.Empty;
    public string? DataType { get; set; }
    public string? DataValue { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
}

