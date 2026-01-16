namespace ErpCore.Shared.Common;

/// <summary>
/// 零用金查詢參數 (SYSQ210)
/// </summary>
public class PcCashQueryParams
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public DateTime? AppleDateFrom { get; set; }
    public DateTime? AppleDateTo { get; set; }
    public string? AppleName { get; set; }
    public string? KeepEmpId { get; set; }
    public string? CashStatus { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDirection { get; set; }
}

/// <summary>
/// 零用金請款查詢參數 (SYSQ220)
/// </summary>
public class PcCashRequestQueryParams
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public DateTime? RequestDateFrom { get; set; }
    public DateTime? RequestDateTo { get; set; }
    public string? RequestStatus { get; set; }
}

/// <summary>
/// 零用金拋轉查詢參數 (SYSQ230)
/// </summary>
public class PcCashTransferQueryParams
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public DateTime? TransferDateFrom { get; set; }
    public DateTime? TransferDateTo { get; set; }
    public string? TransferStatus { get; set; }
    public string? VoucherId { get; set; }
}

/// <summary>
/// 零用金盤點查詢參數 (SYSQ241, SYSQ242)
/// </summary>
public class PcCashInventoryQueryParams
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public DateTime? InventoryDateFrom { get; set; }
    public DateTime? InventoryDateTo { get; set; }
    public string? KeepEmpId { get; set; }
    public string? InventoryStatus { get; set; }
}

/// <summary>
/// 傳票審核查詢參數 (SYSQ250)
/// </summary>
public class VoucherAuditQueryParams
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? VoucherId { get; set; }
    public DateTime? AuditDateFrom { get; set; }
    public DateTime? AuditDateTo { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public string? AuditStatus { get; set; }
    public string? SendStatus { get; set; }
}
