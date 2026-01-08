namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// 系統傳票統計 DTO
/// </summary>
public class SystemVoucherCountDto
{
    public string SysId { get; set; } = string.Empty;
    public string SysName { get; set; } = string.Empty;
    public string ProgId { get; set; } = string.Empty;
    public int UnreviewedCount { get; set; }
}

/// <summary>
/// 暫存傳票 DTO
/// </summary>
public class TmpVoucherDto
{
    public long TKey { get; set; }
    public string? VoucherId { get; set; }
    public DateTime? VoucherDate { get; set; }
    public string? TypeId { get; set; }
    public string? SysId { get; set; }
    public string Status { get; set; } = "1";
    public string? UpFlag { get; set; }
    public string? Notes { get; set; }
    public string? VendorId { get; set; }
    public string? StoreId { get; set; }
    public string? SiteId { get; set; }
    public string? SlipType { get; set; }
    public string? SlipNo { get; set; }
    public string? TypeName { get; set; }
    public string? SysName { get; set; }
}

/// <summary>
/// 暫存傳票明細 DTO
/// </summary>
public class TmpVoucherDetailDto : TmpVoucherDto
{
    public List<TmpVoucherDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// 暫存傳票明細項目 DTO
/// </summary>
public class TmpVoucherDetailItemDto
{
    public long? TKey { get; set; }
    public string Sn { get; set; } = string.Empty;
    public string? Dc { get; set; }
    public string? SubN { get; set; }
    public string? OrgId { get; set; }
    public string? ActId { get; set; }
    public string? Notes { get; set; }
    public decimal Val0 { get; set; }
    public decimal Val1 { get; set; }
    public string? VendorId { get; set; }
    public string? AbatId { get; set; }
    public string? ObjectId { get; set; }
}

/// <summary>
/// 暫存傳票查詢 DTO
/// </summary>
public class TmpVoucherQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? TypeId { get; set; }
    public string? SysId { get; set; }
    public string? Status { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public string? SlipType { get; set; }
    public string? VendorId { get; set; }
    public string? StoreId { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 更新暫存傳票 DTO
/// </summary>
public class UpdateTmpVoucherDto
{
    public string? Notes { get; set; }
    public List<TmpVoucherDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// 拋轉傳票 DTO
/// </summary>
public class TransferVoucherDto
{
    public DateTime? VoucherDate { get; set; }
    public bool ValidateCloseDate { get; set; } = true;
}

/// <summary>
/// 批次拋轉傳票 DTO
/// </summary>
public class BatchTransferVoucherDto
{
    public List<long> TKeys { get; set; } = new();
    public bool ValidateCloseDate { get; set; } = true;
}

/// <summary>
/// 拋轉結果 DTO
/// </summary>
public class TransferVoucherResultDto
{
    public long TKey { get; set; }
    public string? VoucherId { get; set; }
    public DateTime TransferDate { get; set; }
}

/// <summary>
/// 批次拋轉結果 DTO
/// </summary>
public class BatchTransferResultDto
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
}

/// <summary>
/// 未審核筆數 DTO
/// </summary>
public class UnreviewedCountDto
{
    public int TotalCount { get; set; }
    public List<SystemVoucherCountDto> BySystem { get; set; } = new();
}

