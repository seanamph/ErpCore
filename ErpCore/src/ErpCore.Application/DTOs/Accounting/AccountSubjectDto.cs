namespace ErpCore.Application.DTOs.Accounting;

/// <summary>
/// 會計科目 DTO (SYSN110)
/// </summary>
public class AccountSubjectDto
{
    public long TKey { get; set; }
    public string StypeId { get; set; } = string.Empty;
    public string StypeName { get; set; } = string.Empty;
    public string? StypeNameE { get; set; }
    public string? Dc { get; set; }
    public string? LedgerMd { get; set; }
    public string? MtypeId { get; set; }
    public string? AbatYn { get; set; }
    public string? VoucherType { get; set; }
    public string? BudgetYn { get; set; }
    public string? OrgYn { get; set; }
    public decimal? ExpYear { get; set; }
    public decimal? ResiValue { get; set; }
    public string? DepreLid { get; set; }
    public string? AccudepreLid { get; set; }
    public string? StypeYn { get; set; }
    public string? IfrsStypeId { get; set; }
    public string? RocStypeId { get; set; }
    public string? SapStypeId { get; set; }
    public string? StypeClass { get; set; }
    public int? StypeOrder { get; set; }
    public decimal? Amt0 { get; set; }
    public decimal? Amt1 { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 會計科目查詢 DTO
/// </summary>
public class AccountSubjectQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? StypeId { get; set; }
    public string? StypeName { get; set; }
    public string? Dc { get; set; }
    public string? LedgerMd { get; set; }
    public string? VoucherType { get; set; }
    public string? BudgetYn { get; set; }
    public string? StypeClass { get; set; }
}

/// <summary>
/// 新增會計科目 DTO
/// </summary>
public class CreateAccountSubjectDto
{
    public string StypeId { get; set; } = string.Empty;
    public string StypeName { get; set; } = string.Empty;
    public string? StypeNameE { get; set; }
    public string? Dc { get; set; }
    public string? LedgerMd { get; set; }
    public string? MtypeId { get; set; }
    public string? AbatYn { get; set; }
    public string? VoucherType { get; set; }
    public string? BudgetYn { get; set; }
    public string? OrgYn { get; set; }
    public decimal? ExpYear { get; set; }
    public decimal? ResiValue { get; set; }
    public string? DepreLid { get; set; }
    public string? AccudepreLid { get; set; }
    public string? StypeYn { get; set; }
    public string? IfrsStypeId { get; set; }
    public string? RocStypeId { get; set; }
    public string? SapStypeId { get; set; }
    public string? StypeClass { get; set; }
    public int? StypeOrder { get; set; }
    public decimal? Amt0 { get; set; }
    public decimal? Amt1 { get; set; }
}

/// <summary>
/// 修改會計科目 DTO
/// </summary>
public class UpdateAccountSubjectDto
{
    public string StypeName { get; set; } = string.Empty;
    public string? StypeNameE { get; set; }
    public string? Dc { get; set; }
    public string? LedgerMd { get; set; }
    public string? MtypeId { get; set; }
    public string? AbatYn { get; set; }
    public string? VoucherType { get; set; }
    public string? BudgetYn { get; set; }
    public string? OrgYn { get; set; }
    public decimal? ExpYear { get; set; }
    public decimal? ResiValue { get; set; }
    public string? DepreLid { get; set; }
    public string? AccudepreLid { get; set; }
    public string? StypeYn { get; set; }
    public string? IfrsStypeId { get; set; }
    public string? RocStypeId { get; set; }
    public string? SapStypeId { get; set; }
    public string? StypeClass { get; set; }
    public int? StypeOrder { get; set; }
    public decimal? Amt0 { get; set; }
    public decimal? Amt1 { get; set; }
}

/// <summary>
/// 未沖帳餘額 DTO
/// </summary>
public class UnsettledBalanceDto
{
    public bool HasUnsettledBalance { get; set; }
    public decimal Balance { get; set; }
}

