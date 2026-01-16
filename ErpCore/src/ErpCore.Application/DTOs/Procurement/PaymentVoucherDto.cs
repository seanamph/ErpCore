namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 付款單 DTO (SYSP271-SYSP2B0)
/// </summary>
public class PaymentVoucherDto
{
    public long TKey { get; set; }
    public string PaymentNo { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public decimal PaymentAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? BankAccount { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string? Verifier { get; set; }
    public DateTime? VerifyDate { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立付款單 DTO
/// </summary>
public class CreatePaymentVoucherDto
{
    public string PaymentNo { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public decimal PaymentAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? BankAccount { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改付款單 DTO
/// </summary>
public class UpdatePaymentVoucherDto
{
    public DateTime PaymentDate { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public decimal PaymentAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? BankAccount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 查詢付款單 DTO
/// </summary>
public class PaymentVoucherQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? PaymentNo { get; set; }
    public DateTime? PaymentDateFrom { get; set; }
    public DateTime? PaymentDateTo { get; set; }
    public string? SupplierId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Status { get; set; }
}
