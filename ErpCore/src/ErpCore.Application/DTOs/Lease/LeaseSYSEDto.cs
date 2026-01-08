namespace ErpCore.Application.DTOs.Lease;

/// <summary>
/// 租賃條件 DTO (SYSE110-SYSE140)
/// </summary>
public class LeaseTermDto
{
    public long TKey { get; set; }
    public string LeaseId { get; set; } = string.Empty;
    public string Version { get; set; } = "1";
    public string TermType { get; set; } = string.Empty;
    public string? TermName { get; set; }
    public string? TermValue { get; set; }
    public decimal? TermAmount { get; set; }
    public DateTime? TermDate { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立租賃條件 DTO
/// </summary>
public class CreateLeaseTermDto
{
    public string LeaseId { get; set; } = string.Empty;
    public string Version { get; set; } = "1";
    public string TermType { get; set; } = string.Empty;
    public string? TermName { get; set; }
    public string? TermValue { get; set; }
    public decimal? TermAmount { get; set; }
    public DateTime? TermDate { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改租賃條件 DTO
/// </summary>
public class UpdateLeaseTermDto
{
    public string TermType { get; set; } = string.Empty;
    public string? TermName { get; set; }
    public string? TermValue { get; set; }
    public decimal? TermAmount { get; set; }
    public DateTime? TermDate { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 租賃會計分類 DTO (SYSE110-SYSE140)
/// </summary>
public class LeaseAccountingCategoryDto
{
    public long TKey { get; set; }
    public string LeaseId { get; set; } = string.Empty;
    public string Version { get; set; } = "1";
    public string CategoryId { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public decimal? Amount { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立租賃會計分類 DTO
/// </summary>
public class CreateLeaseAccountingCategoryDto
{
    public string LeaseId { get; set; } = string.Empty;
    public string Version { get; set; } = "1";
    public string CategoryId { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public decimal? Amount { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改租賃會計分類 DTO
/// </summary>
public class UpdateLeaseAccountingCategoryDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public decimal? Amount { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 費用主檔 DTO (SYSE310-SYSE430)
/// </summary>
public class LeaseFeeDto
{
    public long TKey { get; set; }
    public string FeeId { get; set; } = string.Empty;
    public string LeaseId { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public string? FeeItemId { get; set; }
    public string? FeeItemName { get; set; }
    public decimal FeeAmount { get; set; } = 0;
    public DateTime FeeDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public decimal PaidAmount { get; set; } = 0;
    public string Status { get; set; } = "P";
    public string? StatusName { get; set; }
    public string? CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public decimal TaxRate { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; } = 0;
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立費用 DTO
/// </summary>
public class CreateLeaseFeeDto
{
    public string FeeId { get; set; } = string.Empty;
    public string LeaseId { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public string? FeeItemId { get; set; }
    public string? FeeItemName { get; set; }
    public decimal FeeAmount { get; set; } = 0;
    public DateTime FeeDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "P";
    public string? CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public decimal TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 修改費用 DTO
/// </summary>
public class UpdateLeaseFeeDto
{
    public string FeeType { get; set; } = string.Empty;
    public string? FeeItemId { get; set; }
    public string? FeeItemName { get; set; }
    public decimal FeeAmount { get; set; } = 0;
    public DateTime FeeDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "P";
    public string? CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public decimal TaxRate { get; set; } = 0;
    public string? Memo { get; set; }
}

/// <summary>
/// 費用項目主檔 DTO (SYSE310-SYSE430)
/// </summary>
public class LeaseFeeItemDto
{
    public long TKey { get; set; }
    public string FeeItemId { get; set; } = string.Empty;
    public string FeeItemName { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; } = 0;
    public string Status { get; set; } = "A";
    public string? StatusName { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立費用項目 DTO
/// </summary>
public class CreateLeaseFeeItemDto
{
    public string FeeItemId { get; set; } = string.Empty;
    public string FeeItemName { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; } = 0;
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
}

/// <summary>
/// 修改費用項目 DTO
/// </summary>
public class UpdateLeaseFeeItemDto
{
    public string FeeItemName { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; } = 0;
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢條件 DTO
/// </summary>
public class LeaseTermQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LeaseId { get; set; }
    public string? Version { get; set; }
    public string? TermType { get; set; }
}

/// <summary>
/// 查詢會計分類 DTO
/// </summary>
public class LeaseAccountingCategoryQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LeaseId { get; set; }
    public string? Version { get; set; }
    public string? CategoryId { get; set; }
}

/// <summary>
/// 查詢費用 DTO
/// </summary>
public class LeaseFeeQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? FeeId { get; set; }
    public string? LeaseId { get; set; }
    public string? FeeType { get; set; }
    public string? Status { get; set; }
    public DateTime? FeeDateFrom { get; set; }
    public DateTime? FeeDateTo { get; set; }
}

/// <summary>
/// 查詢費用項目 DTO
/// </summary>
public class LeaseFeeItemQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? FeeItemId { get; set; }
    public string? FeeItemName { get; set; }
    public string? FeeType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 更新費用狀態 DTO
/// </summary>
public class UpdateLeaseFeeStatusDto
{
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 更新費用已付金額 DTO
/// </summary>
public class UpdateLeaseFeePaidAmountDto
{
    public decimal PaidAmount { get; set; }
    public DateTime? PaidDate { get; set; }
}

/// <summary>
/// 更新費用項目狀態 DTO
/// </summary>
public class UpdateLeaseFeeItemStatusDto
{
    public string Status { get; set; } = string.Empty;
}

