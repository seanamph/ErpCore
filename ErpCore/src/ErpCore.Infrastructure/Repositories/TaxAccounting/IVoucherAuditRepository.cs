using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 暫存傳票審核 Repository 介面 (SYSTA00-SYSTA70)
/// </summary>
public interface IVoucherAuditRepository
{
    // TmpVoucherM 相關操作
    Task<TmpVoucherM?> GetTmpVoucherByIdAsync(long tKey);
    Task<PagedResult<TmpVoucherM>> GetTmpVouchersPagedAsync(TmpVoucherQuery query);
    Task<TmpVoucherM> UpdateTmpVoucherAsync(TmpVoucherM voucher);
    Task DeleteTmpVoucherAsync(long tKey);
    Task<bool> TmpVoucherExistsAsync(long tKey);

    // TmpVoucherD 相關操作
    Task<List<TmpVoucherD>> GetTmpVoucherDetailsAsync(long voucherTKey);
    Task CreateTmpVoucherDetailAsync(TmpVoucherD detail);
    Task UpdateTmpVoucherDetailAsync(TmpVoucherD detail);
    Task DeleteTmpVoucherDetailsAsync(long voucherTKey);

    // 統計相關操作
    Task<int> GetUnreviewedCountAsync(string? typeId = null, string? sysId = null);
    Task<List<SystemVoucherCountDto>> GetSystemVoucherCountsAsync();

    // 拋轉相關操作
    Task<string> GenerateVoucherIdAsync(DateTime voucherDate);
    Task<bool> IsVoucherDateValidAsync(DateTime voucherDate);
}

/// <summary>
/// 暫存傳票查詢條件
/// </summary>
public class TmpVoucherQuery : PagedQuery
{
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
/// 系統傳票統計
/// </summary>
public class SystemVoucherCountDto
{
    public string SysId { get; set; } = string.Empty;
    public string SysName { get; set; } = string.Empty;
    public string ProgId { get; set; } = string.Empty;
    public int UnreviewedCount { get; set; }
}

