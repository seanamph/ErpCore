namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 員餐卡欄位 DTO (SYSL206/SYSL207)
/// </summary>
public class EmployeeMealCardFieldDto
{
    public long TKey { get; set; }
    public string FieldId { get; set; } = string.Empty;
    public string? FieldName { get; set; }
    public string? CardType { get; set; }
    public string? CardTypeName { get; set; }
    public string? ActionType { get; set; }
    public string? ActionTypeName { get; set; }
    public string? OtherType { get; set; }
    public string? OtherTypeName { get; set; }
    public string? MustKeyinYn { get; set; }
    public string? ReadonlyYn { get; set; }
    public string? BtnEtekYn { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 員餐卡欄位查詢 DTO (SYSL206/SYSL207)
/// </summary>
public class EmployeeMealCardFieldQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? FieldId { get; set; }
    public string? FieldName { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? OtherType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增員餐卡欄位 DTO (SYSL206/SYSL207)
/// </summary>
public class CreateEmployeeMealCardFieldDto
{
    public string FieldId { get; set; } = string.Empty;
    public string? FieldName { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? OtherType { get; set; }
    public string? MustKeyinYn { get; set; }
    public string? ReadonlyYn { get; set; }
    public string? BtnEtekYn { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改員餐卡欄位 DTO (SYSL206/SYSL207)
/// </summary>
public class UpdateEmployeeMealCardFieldDto
{
    public string? FieldName { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? OtherType { get; set; }
    public string? MustKeyinYn { get; set; }
    public string? ReadonlyYn { get; set; }
    public string? BtnEtekYn { get; set; }
    public int? SeqNo { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 切換Y/N值 DTO (SYSL206/SYSL207)
/// </summary>
public class ToggleYnDto
{
    public string FieldType { get; set; } = string.Empty; // mustKeyinYn, readonlyYn, btnEtekYn
    public string CurrentValue { get; set; } = string.Empty;
}

