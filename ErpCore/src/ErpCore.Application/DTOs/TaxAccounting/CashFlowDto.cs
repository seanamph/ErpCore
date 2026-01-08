namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// 現金流量大分類 DTO (SYST131)
/// </summary>
public class CashFlowLargeTypeDto
{
    public long TKey { get; set; }
    public string CashLTypeId { get; set; } = string.Empty;
    public string CashLTypeName { get; set; } = string.Empty;
    public string? AbItem { get; set; }
    public string? Sn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 現金流量大分類查詢 DTO
/// </summary>
public class CashFlowLargeTypeQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CashLTypeId { get; set; }
    public string? CashLTypeName { get; set; }
    public string? AbItem { get; set; }
}

/// <summary>
/// 新增現金流量大分類 DTO
/// </summary>
public class CreateCashFlowLargeTypeDto
{
    public string CashLTypeId { get; set; } = string.Empty;
    public string CashLTypeName { get; set; } = string.Empty;
    public string? AbItem { get; set; }
    public string? Sn { get; set; }
}

/// <summary>
/// 修改現金流量大分類 DTO
/// </summary>
public class UpdateCashFlowLargeTypeDto
{
    public string CashLTypeName { get; set; } = string.Empty;
    public string? AbItem { get; set; }
    public string? Sn { get; set; }
}

/// <summary>
/// 現金流量中分類 DTO (SYST132)
/// </summary>
public class CashFlowMediumTypeDto
{
    public long TKey { get; set; }
    public string CashLTypeId { get; set; } = string.Empty;
    public string CashMTypeId { get; set; } = string.Empty;
    public string? CashMTypeName { get; set; }
    public string? AbItem { get; set; }
    public string? Sn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 現金流量中分類查詢 DTO
/// </summary>
public class CashFlowMediumTypeQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CashLTypeId { get; set; }
    public string? CashMTypeId { get; set; }
    public string? CashMTypeName { get; set; }
    public string? AbItem { get; set; }
}

/// <summary>
/// 新增現金流量中分類 DTO
/// </summary>
public class CreateCashFlowMediumTypeDto
{
    public string CashLTypeId { get; set; } = string.Empty;
    public string CashMTypeId { get; set; } = string.Empty;
    public string? CashMTypeName { get; set; }
    public string? AbItem { get; set; }
    public string? Sn { get; set; }
}

/// <summary>
/// 修改現金流量中分類 DTO
/// </summary>
public class UpdateCashFlowMediumTypeDto
{
    public string? CashMTypeName { get; set; }
    public string? AbItem { get; set; }
    public string? Sn { get; set; }
}

/// <summary>
/// 現金流量科目設定 DTO (SYST133)
/// </summary>
public class CashFlowSubjectTypeDto
{
    public long TKey { get; set; }
    public string CashMTypeId { get; set; } = string.Empty;
    public string CashSTypeId { get; set; } = string.Empty;
    public string? AbItem { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 現金流量科目設定查詢 DTO
/// </summary>
public class CashFlowSubjectTypeQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CashMTypeId { get; set; }
    public string? CashSTypeId { get; set; }
}

/// <summary>
/// 新增現金流量科目設定 DTO
/// </summary>
public class CreateCashFlowSubjectTypeDto
{
    public string CashMTypeId { get; set; } = string.Empty;
    public string CashSTypeId { get; set; } = string.Empty;
    public string? AbItem { get; set; }
}

/// <summary>
/// 修改現金流量科目設定 DTO
/// </summary>
public class UpdateCashFlowSubjectTypeDto
{
    public string? AbItem { get; set; }
}

/// <summary>
/// 現金流量小計設定 DTO (SYST134)
/// </summary>
public class CashFlowSubTotalDto
{
    public long TKey { get; set; }
    public string CashLTypeId { get; set; } = string.Empty;
    public string CashSubId { get; set; } = string.Empty;
    public string CashSubName { get; set; } = string.Empty;
    public string? CashMTypeIdB { get; set; }
    public string? CashMTypeIdE { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 現金流量小計設定查詢 DTO
/// </summary>
public class CashFlowSubTotalQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CashLTypeId { get; set; }
    public string? CashSubId { get; set; }
}

/// <summary>
/// 新增現金流量小計設定 DTO
/// </summary>
public class CreateCashFlowSubTotalDto
{
    public string CashLTypeId { get; set; } = string.Empty;
    public string CashSubId { get; set; } = string.Empty;
    public string CashSubName { get; set; } = string.Empty;
    public string? CashMTypeIdB { get; set; }
    public string? CashMTypeIdE { get; set; }
}

/// <summary>
/// 修改現金流量小計設定 DTO
/// </summary>
public class UpdateCashFlowSubTotalDto
{
    public string CashSubName { get; set; } = string.Empty;
    public string? CashMTypeIdB { get; set; }
    public string? CashMTypeIdE { get; set; }
}

