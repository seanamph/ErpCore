namespace ErpCore.Application.DTOs.MirModule;

/// <summary>
/// MIRV000 資料 DTO
/// </summary>
public class MirV000DataDto
{
    public long TKey { get; set; }
    public string DataId { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// MIRV000 資料查詢 DTO
/// </summary>
public class MirV000DataQueryDto
{
    public string? DataId { get; set; }
    public string? DataName { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// MIRV000 資料建立 DTO
/// </summary>
public class CreateMirV000DataDto
{
    public string DataId { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// MIRV000 資料修改 DTO
/// </summary>
public class UpdateMirV000DataDto
{
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
}

