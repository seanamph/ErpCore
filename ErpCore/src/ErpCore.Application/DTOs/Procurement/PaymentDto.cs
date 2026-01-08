namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 付款單 DTO (SYSP271-SYSP2B0)
/// </summary>
public class PaymentDto
{
    public long TKey { get; set; }
    public string PaymentId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? CurrencyId { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? BankAccountId { get; set; }
    public string? BankAccountName { get; set; }
    public string? CheckNumber { get; set; }
    public string Status { get; set; } = "D";
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立付款單 DTO
/// </summary>
public class CreatePaymentDto
{
    public string PaymentId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? CurrencyId { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? BankAccountId { get; set; }
    public string? CheckNumber { get; set; }
    public string Status { get; set; } = "D";
    public string? Memo { get; set; }
}

/// <summary>
/// 修改付款單 DTO
/// </summary>
public class UpdatePaymentDto
{
    public DateTime PaymentDate { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? CurrencyId { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? BankAccountId { get; set; }
    public string? CheckNumber { get; set; }
    public string Status { get; set; } = "D";
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢付款單 DTO
/// </summary>
public class PaymentQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? PaymentId { get; set; }
    public DateTime? PaymentDateFrom { get; set; }
    public DateTime? PaymentDateTo { get; set; }
    public string? SupplierId { get; set; }
    public string? PaymentType { get; set; }
    public string? Status { get; set; }
}

