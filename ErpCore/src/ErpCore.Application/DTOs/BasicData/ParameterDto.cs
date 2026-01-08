using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 參數 DTO
/// </summary>
public class ParameterDto
{
    public long TKey { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string? Content { get; set; }
    public string? Content2 { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "1";
    public string? ReadOnly { get; set; }
    public string? SystemId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 參數查詢 DTO
/// </summary>
public class ParameterQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? Title { get; set; }
    public string? Tag { get; set; }
    public string? Status { get; set; }
    public string? SystemId { get; set; }
}

/// <summary>
/// 新增參數 DTO
/// </summary>
public class CreateParameterDto
{
    public string Title { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string? Content { get; set; }
    public string? Content2 { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "1";
    public string? ReadOnly { get; set; }
    public string? SystemId { get; set; }
}

/// <summary>
/// 修改參數 DTO
/// </summary>
public class UpdateParameterDto
{
    public int? SeqNo { get; set; }
    public string? Content { get; set; }
    public string? Content2 { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "1";
    public string? ReadOnly { get; set; }
    public string? SystemId { get; set; }
}

/// <summary>
/// 參數鍵值 DTO
/// </summary>
public class ParameterKeyDto
{
    public string Title { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
}

/// <summary>
/// 參數值 DTO
/// </summary>
public class ParameterValueDto
{
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// 批次刪除參數 DTO
/// </summary>
public class BatchDeleteParameterDto
{
    public List<ParameterKeyDto> Items { get; set; } = new();
}

