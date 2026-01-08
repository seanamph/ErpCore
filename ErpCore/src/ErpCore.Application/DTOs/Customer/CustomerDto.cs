namespace ErpCore.Application.DTOs.Customer;

/// <summary>
/// 客戶 DTO
/// </summary>
public class CustomerDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string? GuiId { get; set; }
    public string? GuiType { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerNameE { get; set; }
    public string? ShortName { get; set; }
    public string? ContactStaff { get; set; }
    public string? HomeTel { get; set; }
    public string? CompTel { get; set; }
    public string? Fax { get; set; }
    public string? Cell { get; set; }
    public string? Email { get; set; }
    public string? Sex { get; set; }
    public string? Title { get; set; }
    public string? City { get; set; }
    public string? Canton { get; set; }
    public string? Addr { get; set; }
    public string? TaxAddr { get; set; }
    public string? DelyAddr { get; set; }
    public string? PostId { get; set; }
    public string? DiscountYn { get; set; }
    public string? DiscountNo { get; set; }
    public string? SalesId { get; set; }
    public DateTime? TransDate { get; set; }
    public string? TransNo { get; set; }
    public decimal AccAmt { get; set; }
    public string? MonthlyYn { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CustomerContactDto>? Contacts { get; set; }
}

/// <summary>
/// 客戶聯絡人 DTO
/// </summary>
public class CustomerContactDto
{
    public Guid? ContactId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string? ContactTitle { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactCell { get; set; }
    public string? ContactEmail { get; set; }
    public bool IsPrimary { get; set; }
}

/// <summary>
/// 客戶查詢 DTO
/// </summary>
public class CustomerQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? GuiId { get; set; }
    public string? Status { get; set; }
    public string? SalesId { get; set; }
}

/// <summary>
/// 新增客戶 DTO
/// </summary>
public class CreateCustomerDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string? GuiId { get; set; }
    public string? GuiType { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerNameE { get; set; }
    public string? ShortName { get; set; }
    public string? ContactStaff { get; set; }
    public string? HomeTel { get; set; }
    public string? CompTel { get; set; }
    public string? Fax { get; set; }
    public string? Cell { get; set; }
    public string? Email { get; set; }
    public string? Sex { get; set; }
    public string? Title { get; set; }
    public string? City { get; set; }
    public string? Canton { get; set; }
    public string? Addr { get; set; }
    public string? TaxAddr { get; set; }
    public string? DelyAddr { get; set; }
    public string? PostId { get; set; }
    public string? DiscountYn { get; set; }
    public string? DiscountNo { get; set; }
    public string? SalesId { get; set; }
    public string? MonthlyYn { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public List<CreateCustomerContactDto>? Contacts { get; set; }
}

/// <summary>
/// 新增客戶聯絡人 DTO
/// </summary>
public class CreateCustomerContactDto
{
    public string ContactName { get; set; } = string.Empty;
    public string? ContactTitle { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactCell { get; set; }
    public string? ContactEmail { get; set; }
    public bool IsPrimary { get; set; }
}

/// <summary>
/// 修改客戶 DTO
/// </summary>
public class UpdateCustomerDto
{
    public string? GuiId { get; set; }
    public string? GuiType { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerNameE { get; set; }
    public string? ShortName { get; set; }
    public string? ContactStaff { get; set; }
    public string? HomeTel { get; set; }
    public string? CompTel { get; set; }
    public string? Fax { get; set; }
    public string? Cell { get; set; }
    public string? Email { get; set; }
    public string? Sex { get; set; }
    public string? Title { get; set; }
    public string? City { get; set; }
    public string? Canton { get; set; }
    public string? Addr { get; set; }
    public string? TaxAddr { get; set; }
    public string? DelyAddr { get; set; }
    public string? PostId { get; set; }
    public string? DiscountYn { get; set; }
    public string? DiscountNo { get; set; }
    public string? SalesId { get; set; }
    public string? MonthlyYn { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public List<CreateCustomerContactDto>? Contacts { get; set; }
}

/// <summary>
/// 批次刪除客戶 DTO
/// </summary>
public class BatchDeleteCustomerDto
{
    public List<string> CustomerIds { get; set; } = new();
}

/// <summary>
/// 統一編號驗證 DTO
/// </summary>
public class ValidateGuiIdDto
{
    public string GuiId { get; set; } = string.Empty;
    public string GuiType { get; set; } = "1";
}

/// <summary>
/// 統一編號驗證結果 DTO
/// </summary>
public class GuiIdValidationResultDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 客戶進階查詢 DTO (CUS5120)
/// </summary>
public class CustomerAdvancedQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public CustomerQueryFiltersDto? Filters { get; set; }
    public DateRangeDto? DateRange { get; set; }
    public AmountRangeDto? AmountRange { get; set; }
}

/// <summary>
/// 客戶查詢篩選條件 DTO
/// </summary>
public class CustomerQueryFiltersDto
{
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? GuiId { get; set; }
    public string? GuiType { get; set; }
    public string? ContactStaff { get; set; }
    public string? CompTel { get; set; }
    public string? Cell { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? Canton { get; set; }
    public string? SalesId { get; set; }
    public string? Status { get; set; }
    public string? DiscountYn { get; set; }
    public string? MonthlyYn { get; set; }
    public DateTime? TransDateFrom { get; set; }
    public DateTime? TransDateTo { get; set; }
    public decimal? AccAmtFrom { get; set; }
    public decimal? AccAmtTo { get; set; }
}

/// <summary>
/// 日期範圍 DTO
/// </summary>
public class DateRangeDto
{
    public string? Field { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

/// <summary>
/// 金額範圍 DTO
/// </summary>
public class AmountRangeDto
{
    public string? Field { get; set; }
    public decimal? From { get; set; }
    public decimal? To { get; set; }
}

/// <summary>
/// 客戶快速搜尋 DTO
/// </summary>
public class CustomerSearchDto
{
    public string Keyword { get; set; } = string.Empty;
    public int Limit { get; set; } = 10;
}

/// <summary>
/// 客戶快速搜尋結果 DTO
/// </summary>
public class CustomerSearchResultDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? GuiId { get; set; }
    public string? ShortName { get; set; }
    public string? CompTel { get; set; }
    public string? Cell { get; set; }
}

/// <summary>
/// 客戶交易記錄 DTO
/// </summary>
public class CustomerTransactionDto
{
    public Guid TransactionId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string TransactionNo { get; set; } = string.Empty;
    public string? TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 客戶交易記錄查詢 DTO
/// </summary>
public class CustomerTransactionQueryDto
{
    public string CustomerId { get; set; } = string.Empty;
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// 匯出Excel請求 DTO
/// </summary>
public class CustomerExportDto
{
    public CustomerQueryFiltersDto? Filters { get; set; }
    public List<string>? Columns { get; set; }
}

/// <summary>
/// 查詢歷史記錄 DTO
/// </summary>
public class QueryHistoryDto
{
    public Guid HistoryId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public string? QueryName { get; set; }
    public string? QueryConditions { get; set; }
    public bool IsFavorite { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 儲存查詢條件 DTO
/// </summary>
public class SaveQueryHistoryDto
{
    public string QueryName { get; set; } = string.Empty;
    public CustomerQueryFiltersDto? QueryConditions { get; set; }
    public bool IsFavorite { get; set; }
}

/// <summary>
/// 客戶報表查詢 DTO (CUS5130)
/// </summary>
public class CustomerReportQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public CustomerReportFiltersDto? Filters { get; set; }
}

/// <summary>
/// 客戶報表篩選條件 DTO
/// </summary>
public class CustomerReportFiltersDto
{
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? GuiId { get; set; }
    public string? Status { get; set; }
    public string? City { get; set; }
    public string? Canton { get; set; }
    public string? MonthlyYn { get; set; }
    public DateTime? TransDateFrom { get; set; }
    public DateTime? TransDateTo { get; set; }
}

/// <summary>
/// 客戶報表資料 DTO
/// </summary>
public class CustomerReportDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? GuiId { get; set; }
    public string? GuiType { get; set; }
    public string? ContactStaff { get; set; }
    public string? CompTel { get; set; }
    public string? Cell { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? Canton { get; set; }
    public string? Addr { get; set; }
    public string? Status { get; set; }
    public DateTime? TransDate { get; set; }
    public decimal AccAmt { get; set; }
    public string? MonthlyYn { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}

