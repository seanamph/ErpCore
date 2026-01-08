namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// 傳票型態 DTO (SYST121-SYST122)
/// </summary>
public class VoucherTypeDto
{
    public long TKey { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public string VoucherName { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 傳票型態查詢 DTO
/// </summary>
public class VoucherTypeQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VoucherId { get; set; }
    public string? VoucherName { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增傳票型態 DTO
/// </summary>
public class CreateVoucherTypeDto
{
    public string VoucherId { get; set; } = string.Empty;
    public string VoucherName { get; set; } = string.Empty;
    public string? Status { get; set; } = "1";
}

/// <summary>
/// 修改傳票型態 DTO
/// </summary>
public class UpdateVoucherTypeDto
{
    public string VoucherName { get; set; } = string.Empty;
    public string? Status { get; set; }
}

