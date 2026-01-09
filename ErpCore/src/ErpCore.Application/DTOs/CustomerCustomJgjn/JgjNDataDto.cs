using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.CustomerCustomJgjn;

/// <summary>
/// JGJN資料 DTO
/// </summary>
public class JgjNDataDto
{
    public long TKey { get; set; }
    public string DataId { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// JGJN資料查詢 DTO
/// </summary>
public class JgjNDataQueryDto
{
    public string? ModuleCode { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// JGJN資料建立 DTO
/// </summary>
public class CreateJgjNDataDto
{
    public string DataId { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// JGJN資料修改 DTO
/// </summary>
public class UpdateJgjNDataDto
{
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
}

