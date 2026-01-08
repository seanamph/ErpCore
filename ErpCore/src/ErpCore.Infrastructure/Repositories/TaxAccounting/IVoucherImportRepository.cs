using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 傳票轉入 Repository 介面 (SYST002-SYST003)
/// </summary>
public interface IVoucherImportRepository
{
    // VoucherImportLog 相關操作
    Task<VoucherImportLog?> GetImportLogByIdAsync(long tKey);
    Task<PagedResult<VoucherImportLog>> GetImportLogsPagedAsync(VoucherImportLogQuery query);
    Task<long> CreateImportLogAsync(VoucherImportLog log);
    Task UpdateImportLogAsync(VoucherImportLog log);

    // VoucherImportDetail 相關操作
    Task<VoucherImportDetail?> GetImportDetailByIdAsync(long tKey);
    Task<List<VoucherImportDetail>> GetImportDetailsAsync(long importLogTKey, VoucherImportDetailQuery query);
    Task<long> CreateImportDetailAsync(VoucherImportDetail detail);
    Task UpdateImportDetailAsync(VoucherImportDetail detail);
    Task DeleteImportDetailsAsync(long importLogTKey);

    // TmpVoucherM 相關操作（用於轉入）
    Task<long> CreateTmpVoucherAsync(TmpVoucherM voucher);
    Task<bool> CheckDuplicateVoucherAsync(string slipType, string slipNo);
}

/// <summary>
/// 傳票轉入記錄查詢條件
/// </summary>
public class VoucherImportLogQuery : PagedQuery
{
    public string? ImportType { get; set; }
    public string? Status { get; set; }
    public DateTime? ImportDateFrom { get; set; }
    public DateTime? ImportDateTo { get; set; }
    public string? FileName { get; set; }
}

/// <summary>
/// 傳票轉入明細查詢條件
/// </summary>
public class VoucherImportDetailQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
}

