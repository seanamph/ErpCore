namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 可管控欄位查詢 DTO (SYS0510)
/// </summary>
public class ControllableFieldQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? FieldId { get; set; }
    public string? FieldName { get; set; }
    public string? DbName { get; set; }
    public string? TableName { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// 可管控欄位 DTO (SYS0510)
/// </summary>
public class ControllableFieldDto
{
    public string FieldId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string FieldNameInDb { get; set; } = string.Empty;
    public string? FieldType { get; set; }
    public string? FieldDescription { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 新增可管控欄位 DTO (SYS0510)
/// </summary>
public class CreateControllableFieldDto
{
    public string FieldId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string FieldNameInDb { get; set; } = string.Empty;
    public string? FieldType { get; set; }
    public string? FieldDescription { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// 修改可管控欄位 DTO (SYS0510)
/// </summary>
public class UpdateControllableFieldDto
{
    public string FieldName { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string FieldNameInDb { get; set; } = string.Empty;
    public string? FieldType { get; set; }
    public string? FieldDescription { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}

