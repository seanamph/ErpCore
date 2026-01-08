namespace ErpCore.Application.DTOs.Accounting;

/// <summary>
/// 財務交易 DTO (SYSN210)
/// </summary>
public class FinancialTransactionDto
{
    public long TKey { get; set; }
    public string TxnNo { get; set; } = string.Empty;
    public DateTime TxnDate { get; set; }
    public string TxnType { get; set; } = string.Empty;
    public string StypeId { get; set; } = string.Empty;
    public string Dc { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string? Verifier { get; set; }
    public DateTime? VerifyDate { get; set; }
    public string? PostedBy { get; set; }
    public DateTime? PostedDate { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 財務交易查詢 DTO
/// </summary>
public class FinancialTransactionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? TxnNo { get; set; }
    public DateTime? TxnDateFrom { get; set; }
    public DateTime? TxnDateTo { get; set; }
    public string? TxnType { get; set; }
    public string? StypeId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增財務交易 DTO
/// </summary>
public class CreateFinancialTransactionDto
{
    public string TxnNo { get; set; } = string.Empty;
    public DateTime TxnDate { get; set; }
    public string TxnType { get; set; } = string.Empty;
    public string StypeId { get; set; } = string.Empty;
    public string Dc { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改財務交易 DTO
/// </summary>
public class UpdateFinancialTransactionDto
{
    public DateTime TxnDate { get; set; }
    public string TxnType { get; set; } = string.Empty;
    public string StypeId { get; set; } = string.Empty;
    public string Dc { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 借貸平衡檢查 DTO
/// </summary>
public class BalanceCheckDto
{
    public bool IsBalanced { get; set; }
    public decimal DebitTotal { get; set; }
    public decimal CreditTotal { get; set; }
    public decimal Difference { get; set; }
}

