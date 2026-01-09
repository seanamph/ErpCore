namespace ErpCore.Application.DTOs.MshModule;

/// <summary>
/// MSH3000 資料 DTO
/// </summary>
public class Msh3000DataDto
{
    public long TKey { get; set; }
    public string DataId { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string? ImagePath { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// MSH3000 資料查詢 DTO
/// </summary>
public class Msh3000DataQueryDto
{
    public string? DataId { get; set; }
    public string? DataName { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// MSH3000 資料建立 DTO
/// </summary>
public class CreateMsh3000DataDto
{
    public string DataId { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string? ImagePath { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// MSH3000 資料修改 DTO
/// </summary>
public class UpdateMsh3000DataDto
{
    public string DataName { get; set; } = string.Empty;
    public string? DataValue { get; set; }
    public string? DataType { get; set; }
    public string? ImagePath { get; set; }
    public string Status { get; set; } = "A";
    public int SortOrder { get; set; }
    public string? Memo { get; set; }
}

