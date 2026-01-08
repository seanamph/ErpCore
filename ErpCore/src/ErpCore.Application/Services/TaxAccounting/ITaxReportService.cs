using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 稅務報表查詢服務介面 (SYST411-SYST452)
/// </summary>
public interface ITaxReportService
{
    /// <summary>
    /// 查詢傳票列表
    /// </summary>
    Task<PagedResult<InvoiceVoucherDto>> GetVouchersAsync(TaxReportVoucherQueryDto query);

    /// <summary>
    /// 查詢傳票明細
    /// </summary>
    Task<List<VoucherDetailDto>> GetVoucherDetailsAsync(string voucherId);

    /// <summary>
    /// 查詢財務報表
    /// </summary>
    Task<object> GetFinancialReportsAsync(FinancialReportQueryDto query);

    /// <summary>
    /// 查詢稅務統計報表
    /// </summary>
    Task<object> GetTaxStatisticsAsync(TaxStatisticsQueryDto query);

    /// <summary>
    /// 列印傳票
    /// </summary>
    Task<PrintResultDto> PrintVouchersAsync(PrintVoucherDto dto);
}

