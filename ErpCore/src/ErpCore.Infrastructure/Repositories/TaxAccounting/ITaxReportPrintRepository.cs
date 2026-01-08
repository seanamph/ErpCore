using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 稅務報表列印 Repository 介面 (SYST510-SYST530)
/// </summary>
public interface ITaxReportPrintRepository
{
    /// <summary>
    /// 查詢SAP銀行往來資料（分頁）
    /// </summary>
    Task<PagedResult<SapBankTotal>> GetSapBankTotalPagedAsync(SapBankTotalQuery query);

    /// <summary>
    /// 查詢SAP銀行往來資料（列表）
    /// </summary>
    Task<List<SapBankTotal>> GetSapBankTotalListAsync(SapBankTotalQuery query);

    /// <summary>
    /// 根據TKey查詢列印記錄
    /// </summary>
    Task<TaxReportPrint?> GetPrintLogByIdAsync(long tKey);

    /// <summary>
    /// 查詢列印記錄列表（分頁）
    /// </summary>
    Task<PagedResult<TaxReportPrint>> GetPrintLogsPagedAsync(TaxReportPrintQuery query);

    /// <summary>
    /// 新增列印記錄
    /// </summary>
    Task<TaxReportPrint> CreatePrintLogAsync(TaxReportPrint printLog);

    /// <summary>
    /// 修改列印記錄
    /// </summary>
    Task<TaxReportPrint> UpdatePrintLogAsync(TaxReportPrint printLog);

    /// <summary>
    /// 刪除列印記錄
    /// </summary>
    Task DeletePrintLogAsync(long tKey);
}

/// <summary>
/// SAP銀行往來查詢條件
/// </summary>
public class SapBankTotalQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? CompId { get; set; }
    public string? SapStypeId { get; set; }
}

/// <summary>
/// 列印記錄查詢條件
/// </summary>
public class TaxReportPrintQuery : PagedQuery
{
    public string? ReportType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? PrintStatus { get; set; }
}

