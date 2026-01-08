namespace ErpCore.Application.DTOs.Accounting;

/// <summary>
/// 傳票 DTO (SYSN120)
/// </summary>
public class VoucherDto
{
    public long TKey { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string? VoucherTypeId { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "D";
    public string? PostedBy { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<VoucherDetailDto>? Details { get; set; }
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
    public string? Description { get; set; }
}

/// <summary>
/// 傳票查詢 DTO
/// </summary>
public class VoucherQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VoucherId { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public string? VoucherTypeId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增傳票 DTO
/// </summary>
public class CreateVoucherDto
{
    public string VoucherId { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string? VoucherTypeId { get; set; }
    public string? Description { get; set; }
    public List<CreateVoucherDetailDto> Details { get; set; } = new();
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
    public string? Description { get; set; }
}

/// <summary>
/// 修改傳票 DTO
/// </summary>
public class UpdateVoucherDto
{
    public DateTime VoucherDate { get; set; }
    public string? VoucherTypeId { get; set; }
    public string? Description { get; set; }
    public List<CreateVoucherDetailDto> Details { get; set; } = new();
}

