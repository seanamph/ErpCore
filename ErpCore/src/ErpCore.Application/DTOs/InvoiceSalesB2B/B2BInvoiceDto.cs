namespace ErpCore.Application.DTOs.InvoiceSalesB2B;

/// <summary>
/// B2B發票 DTO (SYSG000_B2B - B2B發票資料維護)
/// </summary>
public class B2BInvoiceDto
{
    public long TKey { get; set; }
    public string InvoiceId { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    public int InvoiceYear { get; set; }
    public int InvoiceMonth { get; set; }
    public string InvoiceYm { get; set; } = string.Empty;
    public string? Track { get; set; }
    public string? InvoiceNoB { get; set; }
    public string? InvoiceNoE { get; set; }
    public string? InvoiceFormat { get; set; }
    public string? TaxId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyNameEn { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? SiteId { get; set; }
    public string? SubCopy { get; set; }
    public string? SubCopyValue { get; set; }
    public string B2BFlag { get; set; } = "Y";
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// B2B發票查詢 DTO
/// </summary>
public class B2BInvoiceQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? InvoiceId { get; set; }
    public string? InvoiceType { get; set; }
    public string? InvoiceYm { get; set; }
    public string? TaxId { get; set; }
    public string? SiteId { get; set; }
    public string? Status { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

/// <summary>
/// 建立B2B發票 DTO
/// </summary>
public class CreateB2BInvoiceDto
{
    public string InvoiceId { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    public int InvoiceYear { get; set; }
    public int InvoiceMonth { get; set; }
    public string InvoiceYm { get; set; } = string.Empty;
    public string? Track { get; set; }
    public string? InvoiceNoB { get; set; }
    public string? InvoiceNoE { get; set; }
    public string? InvoiceFormat { get; set; }
    public string? TaxId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyNameEn { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? SiteId { get; set; }
    public string? SubCopy { get; set; }
    public string? SubCopyValue { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改B2B發票 DTO
/// </summary>
public class UpdateB2BInvoiceDto : CreateB2BInvoiceDto
{
    public long TKey { get; set; }
}

