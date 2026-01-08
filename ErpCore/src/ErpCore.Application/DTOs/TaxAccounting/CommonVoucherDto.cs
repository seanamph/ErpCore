namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// 常用傳票 DTO (SYST123)
/// </summary>
public class CommonVoucherDto
{
    public long TKey { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public string VoucherName { get; set; } = string.Empty;
    public string? VoucherType { get; set; }
    public string? SiteId { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Notes { get; set; }
    public string? CustomField1 { get; set; }
    public List<CommonVoucherDetailDto>? Details { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 常用傳票明細 DTO
/// </summary>
public class CommonVoucherDetailDto
{
    public long TKey { get; set; }
    public long VoucherTKey { get; set; }
    public int SeqNo { get; set; }
    public string? StypeId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? OrgId { get; set; }
    public string? Notes { get; set; }
    public string? VendorId { get; set; }
    public string? CustomField1 { get; set; }
}

/// <summary>
/// 常用傳票查詢 DTO
/// </summary>
public class CommonVoucherQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VoucherId { get; set; }
    public string? VoucherName { get; set; }
    public string? VoucherType { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 新增常用傳票 DTO
/// </summary>
public class CreateCommonVoucherDto
{
    public string VoucherId { get; set; } = string.Empty;
    public string VoucherName { get; set; } = string.Empty;
    public string? VoucherType { get; set; }
    public string? SiteId { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Notes { get; set; }
    public string? CustomField1 { get; set; }
    public List<CreateCommonVoucherDetailDto>? Details { get; set; }
}

/// <summary>
/// 新增常用傳票明細 DTO
/// </summary>
public class CreateCommonVoucherDetailDto
{
    public int SeqNo { get; set; }
    public string? StypeId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? OrgId { get; set; }
    public string? Notes { get; set; }
    public string? VendorId { get; set; }
    public string? CustomField1 { get; set; }
}

/// <summary>
/// 修改常用傳票 DTO
/// </summary>
public class UpdateCommonVoucherDto
{
    public string VoucherName { get; set; } = string.Empty;
    public string? VoucherType { get; set; }
    public string? SiteId { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? Notes { get; set; }
    public string? CustomField1 { get; set; }
    public List<UpdateCommonVoucherDetailDto>? Details { get; set; }
}

/// <summary>
/// 修改常用傳票明細 DTO
/// </summary>
public class UpdateCommonVoucherDetailDto
{
    public long? TKey { get; set; }
    public int SeqNo { get; set; }
    public string? StypeId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? OrgId { get; set; }
    public string? Notes { get; set; }
    public string? VendorId { get; set; }
    public string? CustomField1 { get; set; }
}

