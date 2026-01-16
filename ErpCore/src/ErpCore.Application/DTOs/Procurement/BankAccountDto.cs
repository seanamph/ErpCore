namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 銀行帳戶 DTO (銀行帳戶維護)
/// </summary>
public class BankAccountDto
{
    public long TKey { get; set; }
    public string BankAccountId { get; set; } = string.Empty;
    public string BankId { get; set; } = string.Empty;
    public string? BankName { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public string? AccountTypeName { get; set; }
    public string? CurrencyId { get; set; }
    public string Status { get; set; } = "1";
    public string? StatusName { get; set; }
    public decimal? Balance { get; set; }
    public DateTime? OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? BranchName { get; set; }
    public string? BranchCode { get; set; }
    public string? SwiftCode { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 建立銀行帳戶 DTO
/// </summary>
public class CreateBankAccountDto
{
    public string BankAccountId { get; set; } = string.Empty;
    public string BankId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public string? CurrencyId { get; set; }
    public string Status { get; set; } = "1";
    public decimal? Balance { get; set; }
    public DateTime? OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? BranchName { get; set; }
    public string? BranchCode { get; set; }
    public string? SwiftCode { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改銀行帳戶 DTO
/// </summary>
public class UpdateBankAccountDto
{
    public string BankId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public string? CurrencyId { get; set; }
    public string Status { get; set; } = "1";
    public decimal? Balance { get; set; }
    public DateTime? OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? BranchName { get; set; }
    public string? BranchCode { get; set; }
    public string? SwiftCode { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 銀行帳戶查詢 DTO
/// </summary>
public class BankAccountQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? BankAccountId { get; set; }
    public string? BankId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public string? CurrencyId { get; set; }
    public string? Status { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 銀行帳戶餘額 DTO
/// </summary>
public class BankAccountBalanceDto
{
    public string BankAccountId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal? Balance { get; set; }
    public string? CurrencyId { get; set; }
    public DateTime? LastUpdateDate { get; set; }
}
