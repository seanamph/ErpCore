namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// 傳票 DTO (SYST211)
/// </summary>
public class InvoiceVoucherDto
{
    public long TKey { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string? VoucherKind { get; set; }
    public string? VoucherStatus { get; set; }
    public string? Notes { get; set; }
    public string? InvYn { get; set; }
    public string? TypeId { get; set; }
    public string? SiteId { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public List<InvoiceVoucherDetailDto>? InvoiceVouchers { get; set; }
    public List<VoucherDetailDto>? Details { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 發票傳票 DTO
/// </summary>
public class InvoiceVoucherDetailDto
{
    public long TKey { get; set; }
    public long VoucherTKey { get; set; }
    public string InvoiceType { get; set; } = string.Empty;
    public string? InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? InvoiceFormat { get; set; }
    public decimal? InvoiceAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? DeductCode { get; set; }
    public string? CategoryType { get; set; }
    public string? VoucherNo { get; set; }
}

/// <summary>
/// 傳票明細 DTO
/// </summary>
public class VoucherDetailDto
{
    public long TKey { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public int SeqNo { get; set; }
    public string? StypeId { get; set; }
    public string Dc { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? DAmt { get; set; }
    public decimal? CAmt { get; set; }
    public string? Description { get; set; }
    public string? OrgId { get; set; }
    public string? ActId { get; set; }
    public string? AbatId { get; set; }
    public string? VendorId { get; set; }
    public string? CustomField1 { get; set; }
}

/// <summary>
/// 傳票查詢 DTO
/// </summary>
public class InvoiceVoucherQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VoucherId { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public string? VoucherStatus { get; set; }
    public string? VoucherKind { get; set; }
    public string? TypeId { get; set; }
    public string? SiteId { get; set; }
    public string? VendorId { get; set; }
}

/// <summary>
/// 新增傳票 DTO
/// </summary>
public class CreateInvoiceVoucherDto
{
    public DateTime VoucherDate { get; set; }
    public string? VoucherKind { get; set; }
    public string? TypeId { get; set; }
    public string? Notes { get; set; }
    public string? SiteId { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? InvYn { get; set; }
    public List<CreateVoucherDetailDto>? Details { get; set; }
    public List<CreateInvoiceVoucherDetailDto>? InvoiceVouchers { get; set; }
}

/// <summary>
/// 新增傳票明細 DTO
/// </summary>
public class CreateVoucherDetailDto
{
    public int SeqNo { get; set; }
    public string? StypeId { get; set; }
    public string Dc { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? DAmt { get; set; }
    public decimal? CAmt { get; set; }
    public string? Description { get; set; }
    public string? OrgId { get; set; }
    public string? ActId { get; set; }
    public string? AbatId { get; set; }
    public string? VendorId { get; set; }
    public string? CustomField1 { get; set; }
}

/// <summary>
/// 新增發票傳票 DTO
/// </summary>
public class CreateInvoiceVoucherDetailDto
{
    public string InvoiceType { get; set; } = string.Empty;
    public string? InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? InvoiceFormat { get; set; }
    public decimal? InvoiceAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? DeductCode { get; set; }
    public string? CategoryType { get; set; }
    public string? VoucherNo { get; set; }
}

/// <summary>
/// 修改傳票 DTO
/// </summary>
public class UpdateInvoiceVoucherDto
{
    public DateTime VoucherDate { get; set; }
    public string? VoucherKind { get; set; }
    public string? TypeId { get; set; }
    public string? Notes { get; set; }
    public string? SiteId { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? InvYn { get; set; }
    public List<UpdateVoucherDetailDto>? Details { get; set; }
    public List<UpdateInvoiceVoucherDetailDto>? InvoiceVouchers { get; set; }
}

/// <summary>
/// 修改傳票明細 DTO
/// </summary>
public class UpdateVoucherDetailDto
{
    public long? TKey { get; set; }
    public int SeqNo { get; set; }
    public string? StypeId { get; set; }
    public string Dc { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? DAmt { get; set; }
    public decimal? CAmt { get; set; }
    public string? Description { get; set; }
    public string? OrgId { get; set; }
    public string? ActId { get; set; }
    public string? AbatId { get; set; }
    public string? VendorId { get; set; }
    public string? CustomField1 { get; set; }
}

/// <summary>
/// 修改發票傳票 DTO
/// </summary>
public class UpdateInvoiceVoucherDetailDto
{
    public long? TKey { get; set; }
    public string InvoiceType { get; set; } = string.Empty;
    public string? InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? InvoiceFormat { get; set; }
    public decimal? InvoiceAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? DeductCode { get; set; }
    public string? CategoryType { get; set; }
    public string? VoucherNo { get; set; }
}

/// <summary>
/// 借貸平衡檢查結果 DTO
/// </summary>
public class BalanceCheckDto
{
    public bool IsBalanced { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal Difference { get; set; }
}

/// <summary>
/// 費用/收入分攤比率 DTO (SYST212)
/// </summary>
public class AllocationRatioDto
{
    public long TKey { get; set; }
    public string DisYm { get; set; } = string.Empty;
    public string StypeId { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public decimal Ratio { get; set; }
    public long? VoucherTKey { get; set; }
    public long? VoucherDTKey { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 分攤比率查詢 DTO
/// </summary>
public class AllocationRatioQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? DisYm { get; set; }
    public string? StypeId { get; set; }
    public string? OrgId { get; set; }
}

/// <summary>
/// 新增分攤比率 DTO
/// </summary>
public class CreateAllocationRatioDto
{
    public string DisYm { get; set; } = string.Empty;
    public string StypeId { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public decimal Ratio { get; set; }
    public long? VoucherTKey { get; set; }
    public long? VoucherDTKey { get; set; }
}

/// <summary>
/// 修改分攤比率 DTO
/// </summary>
public class UpdateAllocationRatioDto
{
    public string DisYm { get; set; } = string.Empty;
    public string StypeId { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public decimal Ratio { get; set; }
    public long? VoucherTKey { get; set; }
    public long? VoucherDTKey { get; set; }
}

