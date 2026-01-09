namespace ErpCore.Application.DTOs.CustomerInvoice;

/// <summary>
/// 客戶資料 DTO (SYS2000 - 客戶資料維護)
/// </summary>
public class CustomerDataDto
{
    public long TKey { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerType { get; set; }
    public string? TaxId { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactFax { get; set; }
    public string? Address { get; set; }
    public string? CityId { get; set; }
    public string? ZoneId { get; set; }
    public string? ZipCode { get; set; }
    public string? PaymentTerm { get; set; }
    public decimal CreditLimit { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CustomerContactDto>? Contacts { get; set; }
    public List<CustomerAddressDto>? Addresses { get; set; }
    public List<CustomerBankAccountDto>? BankAccounts { get; set; }
}

/// <summary>
/// 客戶聯絡人 DTO
/// </summary>
public class CustomerContactDto
{
    public long TKey { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string? ContactTitle { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactMobile { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactFax { get; set; }
    public bool IsPrimary { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 客戶地址 DTO
/// </summary>
public class CustomerAddressDto
{
    public long TKey { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string AddressType { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? CityId { get; set; }
    public string? ZoneId { get; set; }
    public string? ZipCode { get; set; }
    public bool IsDefault { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 客戶銀行帳戶 DTO
/// </summary>
public class CustomerBankAccountDto
{
    public long TKey { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string BankId { get; set; } = string.Empty;
    public string AccountNo { get; set; } = string.Empty;
    public string? AccountName { get; set; }
    public bool IsDefault { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 客戶資料查詢 DTO
/// </summary>
public class CustomerDataQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerType { get; set; }
    public string? TaxId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增客戶資料 DTO
/// </summary>
public class CreateCustomerDataDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerType { get; set; }
    public string? TaxId { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactFax { get; set; }
    public string? Address { get; set; }
    public string? CityId { get; set; }
    public string? ZoneId { get; set; }
    public string? ZipCode { get; set; }
    public string? PaymentTerm { get; set; }
    public decimal CreditLimit { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public List<CreateCustomerContactDto>? Contacts { get; set; }
    public List<CreateCustomerAddressDto>? Addresses { get; set; }
    public List<CreateCustomerBankAccountDto>? BankAccounts { get; set; }
}

/// <summary>
/// 新增客戶聯絡人 DTO
/// </summary>
public class CreateCustomerContactDto
{
    public string ContactName { get; set; } = string.Empty;
    public string? ContactTitle { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactMobile { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactFax { get; set; }
    public bool IsPrimary { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 新增客戶地址 DTO
/// </summary>
public class CreateCustomerAddressDto
{
    public string AddressType { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? CityId { get; set; }
    public string? ZoneId { get; set; }
    public string? ZipCode { get; set; }
    public bool IsDefault { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 新增客戶銀行帳戶 DTO
/// </summary>
public class CreateCustomerBankAccountDto
{
    public string BankId { get; set; } = string.Empty;
    public string AccountNo { get; set; } = string.Empty;
    public string? AccountName { get; set; }
    public bool IsDefault { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改客戶資料 DTO
/// </summary>
public class UpdateCustomerDataDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerType { get; set; }
    public string? TaxId { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactFax { get; set; }
    public string? Address { get; set; }
    public string? CityId { get; set; }
    public string? ZoneId { get; set; }
    public string? ZipCode { get; set; }
    public string? PaymentTerm { get; set; }
    public decimal CreditLimit { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public List<CreateCustomerContactDto>? Contacts { get; set; }
    public List<CreateCustomerAddressDto>? Addresses { get; set; }
    public List<CreateCustomerBankAccountDto>? BankAccounts { get; set; }
}

/// <summary>
/// 批次刪除客戶 DTO
/// </summary>
public class BatchDeleteCustomerDataDto
{
    public List<string> CustomerIds { get; set; } = new();
}

/// <summary>
/// 發票 DTO (SYS2000 - 發票列印作業)
/// </summary>
public class InvoiceDto
{
    public long TKey { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string? CustomerId { get; set; }
    public string? StoreId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public string Status { get; set; } = "DRAFT";
    public int PrintCount { get; set; }
    public DateTime? LastPrintDate { get; set; }
    public string? LastPrintUser { get; set; }
    public string? PrintFormat { get; set; }
    public string? Memo { get; set; }
    public List<InvoiceDetailDto>? Details { get; set; }
}

/// <summary>
/// 發票明細 DTO
/// </summary>
public class InvoiceDetailDto
{
    public long TKey { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int LineNum { get; set; }
    public string? GoodsId { get; set; }
    public string GoodsName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public string? UnitId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 發票查詢 DTO
/// </summary>
public class InvoiceQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? InvoiceNo { get; set; }
    public string? CustomerId { get; set; }
    public DateTime? InvoiceDateFrom { get; set; }
    public DateTime? InvoiceDateTo { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 發票列印請求 DTO
/// </summary>
public class InvoicePrintRequestDto
{
    public string PrintFormat { get; set; } = "STANDARD";
    public bool IncludeCover { get; set; }
    public PrinterSettingsDto? PrinterSettings { get; set; }
}

/// <summary>
/// 印表機設定 DTO
/// </summary>
public class PrinterSettingsDto
{
    public string? PrinterName { get; set; }
    public string PaperSize { get; set; } = "A4";
}

/// <summary>
/// 批次列印發票 DTO
/// </summary>
public class BatchPrintInvoiceDto
{
    public List<string> InvoiceNos { get; set; } = new();
    public string PrintFormat { get; set; } = "STANDARD";
    public bool IncludeCover { get; set; }
}

/// <summary>
/// 郵件傳真作業 DTO (SYS2000 - 郵件傳真作業)
/// </summary>
public class EmailFaxJobDto
{
    public long TKey { get; set; }
    public string JobId { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string? Content { get; set; }
    public string? AttachmentPath { get; set; }
    public string Status { get; set; } = "PENDING";
    public DateTime? SendDate { get; set; }
    public string? SendUser { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetry { get; set; }
    public DateTime? ScheduleDate { get; set; }
    public string? TemplateId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 發送郵件請求 DTO
/// </summary>
public class SendEmailRequestDto
{
    public string Recipient { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string>? AttachmentPaths { get; set; }
    public string? TemplateId { get; set; }
    public DateTime? ScheduleDate { get; set; }
}

/// <summary>
/// 發送傳真請求 DTO
/// </summary>
public class SendFaxRequestDto
{
    public string FaxNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string>? AttachmentPaths { get; set; }
    public string? TemplateId { get; set; }
    public DateTime? ScheduleDate { get; set; }
}

/// <summary>
/// 郵件傳真作業查詢 DTO
/// </summary>
public class EmailFaxJobQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? JobId { get; set; }
    public string? JobType { get; set; }
    public string? Status { get; set; }
    public DateTime? SendDateFrom { get; set; }
    public DateTime? SendDateTo { get; set; }
}

/// <summary>
/// 總帳 DTO (SYS2000 - 總帳資料維護)
/// </summary>
public class GeneralLedgerDto
{
    public long TKey { get; set; }
    public string LedgerId { get; set; } = string.Empty;
    public DateTime LedgerDate { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public string? VoucherNo { get; set; }
    public string? Description { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal Balance { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public string Period { get; set; } = string.Empty;
    public string Status { get; set; } = "DRAFT";
    public string? Memo { get; set; }
}

/// <summary>
/// 總帳查詢 DTO
/// </summary>
public class GeneralLedgerQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? LedgerId { get; set; }
    public string? AccountId { get; set; }
    public string? Period { get; set; }
    public DateTime? LedgerDateFrom { get; set; }
    public DateTime? LedgerDateTo { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增總帳 DTO
/// </summary>
public class CreateGeneralLedgerDto
{
    public string LedgerId { get; set; } = string.Empty;
    public DateTime LedgerDate { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public string? VoucherNo { get; set; }
    public string? Description { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public string Period { get; set; } = string.Empty;
    public string? Memo { get; set; }
}

/// <summary>
/// 修改總帳 DTO
/// </summary>
public class UpdateGeneralLedgerDto
{
    public DateTime LedgerDate { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public string? VoucherNo { get; set; }
    public string? Description { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public string Period { get; set; } = string.Empty;
    public string? Memo { get; set; }
}

/// <summary>
/// 科目餘額查詢 DTO
/// </summary>
public class AccountBalanceQueryDto
{
    public string? AccountId { get; set; }
    public string? Period { get; set; }
    public DateTime? PeriodFrom { get; set; }
    public DateTime? PeriodTo { get; set; }
}

/// <summary>
/// 科目餘額 DTO
/// </summary>
public class AccountBalanceDto
{
    public string AccountId { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal ClosingBalance { get; set; }
}

/// <summary>
/// 發票列印記錄 DTO
/// </summary>
public class InvoicePrintLogDto
{
    public long TKey { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime PrintDate { get; set; }
    public string? PrintUser { get; set; }
    public string? PrintFormat { get; set; }
    public string? PrintType { get; set; }
    public string? PrinterName { get; set; }
    public int PrintCount { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 郵件傳真發送記錄 DTO
/// </summary>
public class EmailFaxLogDto
{
    public long TKey { get; set; }
    public string JobId { get; set; } = string.Empty;
    public DateTime LogDate { get; set; }
    public string LogType { get; set; } = string.Empty;
    public string? LogMessage { get; set; }
}

