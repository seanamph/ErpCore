namespace ErpCore.Application.DTOs.Query;

/// <summary>
/// 零用金參數 DTO (SYSQ110)
/// </summary>
public class CashParamsDto
{
    public long TKey { get; set; }
    public string? UnitId { get; set; }
    public string ApexpLid { get; set; } = string.Empty;
    public string PtaxLid { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立零用金參數 DTO
/// </summary>
public class CreateCashParamsDto
{
    public string? UnitId { get; set; }
    public string ApexpLid { get; set; } = string.Empty;
    public string PtaxLid { get; set; } = string.Empty;
}

/// <summary>
/// 修改零用金參數 DTO
/// </summary>
public class UpdateCashParamsDto
{
    public string? UnitId { get; set; }
    public string ApexpLid { get; set; } = string.Empty;
    public string PtaxLid { get; set; } = string.Empty;
}

/// <summary>
/// 保管人及額度設定 DTO (SYSQ120)
/// </summary>
public class PcKeepDto
{
    public long TKey { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public string KeepEmpId { get; set; } = string.Empty;
    public string? KeepEmpName { get; set; }
    public decimal? PcQuota { get; set; } = 0;
    public string? Notes { get; set; }
    public string? BUser { get; set; }
    public DateTime BTime { get; set; }
    public string? CUser { get; set; }
    public DateTime? CTime { get; set; }
    public int? CPriority { get; set; }
    public string? CGroup { get; set; }
}

/// <summary>
/// 建立保管人及額度設定 DTO
/// </summary>
public class CreatePcKeepDto
{
    public string? SiteId { get; set; }
    public string KeepEmpId { get; set; } = string.Empty;
    public decimal? PcQuota { get; set; } = 0;
    public string? Notes { get; set; }
}

/// <summary>
/// 修改保管人及額度設定 DTO
/// </summary>
public class UpdatePcKeepDto
{
    public decimal? PcQuota { get; set; } = 0;
    public string? Notes { get; set; }
}

/// <summary>
/// 保管人及額度查詢 DTO
/// </summary>
public class PcKeepQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public string? KeepEmpId { get; set; }
}

// ============================================
// 質量管理處理功能 DTO (SYSQ210-SYSQ250)
// ============================================

/// <summary>
/// 零用金主檔 DTO (SYSQ210)
/// </summary>
public class PcCashDto
{
    public long TKey { get; set; }
    public string CashId { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public DateTime AppleDate { get; set; }
    public string AppleName { get; set; } = string.Empty;
    public string? AppleNameDesc { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string? KeepEmpId { get; set; }
    public string? KeepEmpName { get; set; }
    public decimal CashAmount { get; set; }
    public string? CashStatus { get; set; }
    public string? Notes { get; set; }
    public string? BUser { get; set; }
    public DateTime BTime { get; set; }
    public string? CUser { get; set; }
    public DateTime? CTime { get; set; }
    public int? CPriority { get; set; }
    public string? CGroup { get; set; }
}

/// <summary>
/// 建立零用金主檔 DTO
/// </summary>
public class CreatePcCashDto
{
    public string? SiteId { get; set; }
    public DateTime AppleDate { get; set; }
    public string AppleName { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string? KeepEmpId { get; set; }
    public decimal CashAmount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改零用金主檔 DTO
/// </summary>
public class UpdatePcCashDto
{
    public DateTime AppleDate { get; set; }
    public string AppleName { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string? KeepEmpId { get; set; }
    public decimal CashAmount { get; set; }
    public string? CashStatus { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 零用金查詢 DTO
/// </summary>
public class PcCashQueryDto
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
/// 批量新增零用金 DTO
/// </summary>
public class BatchCreatePcCashDto
{
    public List<CreatePcCashDto> Items { get; set; } = new();
}

/// <summary>
/// 零用金請款檔 DTO (SYSQ220)
/// </summary>
public class PcCashRequestDto
{
    public long TKey { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public DateTime RequestDate { get; set; }
    public string? CashIds { get; set; }
    public List<string>? CashIdList { get; set; }
    public decimal RequestAmount { get; set; }
    public string? RequestStatus { get; set; }
    public string? Notes { get; set; }
    public string? BUser { get; set; }
    public DateTime BTime { get; set; }
    public string? CUser { get; set; }
    public DateTime? CTime { get; set; }
}

/// <summary>
/// 建立零用金請款檔 DTO
/// </summary>
public class CreatePcCashRequestDto
{
    public string? SiteId { get; set; }
    public DateTime RequestDate { get; set; }
    public List<string> CashIds { get; set; } = new();
    public string? Notes { get; set; }
}

/// <summary>
/// 零用金請款查詢 DTO
/// </summary>
public class PcCashRequestQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public DateTime? RequestDateFrom { get; set; }
    public DateTime? RequestDateTo { get; set; }
    public string? RequestStatus { get; set; }
}

/// <summary>
/// 零用金拋轉檔 DTO (SYSQ230)
/// </summary>
public class PcCashTransferDto
{
    public long TKey { get; set; }
    public string TransferId { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public DateTime TransferDate { get; set; }
    public string? VoucherId { get; set; }
    public string? VoucherKind { get; set; }
    public DateTime? VoucherDate { get; set; }
    public decimal TransferAmount { get; set; }
    public string? TransferStatus { get; set; }
    public string? Notes { get; set; }
    public string? BUser { get; set; }
    public DateTime BTime { get; set; }
    public string? CUser { get; set; }
    public DateTime? CTime { get; set; }
}

/// <summary>
/// 建立零用金拋轉檔 DTO
/// </summary>
public class CreatePcCashTransferDto
{
    public string? SiteId { get; set; }
    public DateTime TransferDate { get; set; }
    public string? VoucherId { get; set; }
    public string? VoucherKind { get; set; }
    public DateTime? VoucherDate { get; set; }
    public decimal TransferAmount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 零用金盤點檔 DTO (SYSQ241, SYSQ242)
/// </summary>
public class PcCashInventoryDto
{
    public long TKey { get; set; }
    public string InventoryId { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public DateTime InventoryDate { get; set; }
    public string KeepEmpId { get; set; } = string.Empty;
    public string? KeepEmpName { get; set; }
    public decimal InventoryAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public decimal? DifferenceAmount { get; set; }
    public string? InventoryStatus { get; set; }
    public string? Notes { get; set; }
    public string? BUser { get; set; }
    public DateTime BTime { get; set; }
    public string? CUser { get; set; }
    public DateTime? CTime { get; set; }
}

/// <summary>
/// 建立零用金盤點檔 DTO
/// </summary>
public class CreatePcCashInventoryDto
{
    public string? SiteId { get; set; }
    public DateTime InventoryDate { get; set; }
    public string KeepEmpId { get; set; } = string.Empty;
    public decimal InventoryAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改零用金盤點檔 DTO
/// </summary>
public class UpdatePcCashInventoryDto
{
    public decimal? ActualAmount { get; set; }
    public string? InventoryStatus { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 零用金盤點查詢 DTO
/// </summary>
public class PcCashInventoryQueryDto
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
/// 傳票審核傳送檔 DTO (SYSQ250)
/// </summary>
public class VoucherAuditDto
{
    public long TKey { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public string? VoucherKind { get; set; }
    public DateTime VoucherDate { get; set; }
    public string? AuditStatus { get; set; }
    public string? AuditUser { get; set; }
    public string? AuditUserName { get; set; }
    public DateTime? AuditTime { get; set; }
    public string? AuditNotes { get; set; }
    public string? SendStatus { get; set; }
    public DateTime? SendTime { get; set; }
    public string? Notes { get; set; }
    public string? BUser { get; set; }
    public DateTime BTime { get; set; }
    public string? CUser { get; set; }
    public DateTime? CTime { get; set; }
}

/// <summary>
/// 傳票審核傳送查詢 DTO
/// </summary>
public class VoucherAuditQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public string? AuditStatus { get; set; }
    public string? SendStatus { get; set; }
}

/// <summary>
/// 修改傳票審核傳送 DTO
/// </summary>
public class UpdateVoucherAuditDto
{
    public string? AuditStatus { get; set; }
    public string? AuditNotes { get; set; }
    public string? SendStatus { get; set; }
    public string? Notes { get; set; }
}

