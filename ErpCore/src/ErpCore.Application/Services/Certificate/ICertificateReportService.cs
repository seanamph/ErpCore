using ErpCore.Application.DTOs.Certificate;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Certificate;

/// <summary>
/// 憑證報表服務介面 (SYSK310-SYSK500)
/// </summary>
public interface ICertificateReportService
{
    /// <summary>
    /// 憑證明細報表查詢
    /// </summary>
    Task<PagedResult<VoucherDto>> GetVoucherDetailReportAsync(VoucherReportQueryDto query);

    /// <summary>
    /// 憑證統計報表查詢
    /// </summary>
    Task<VoucherStatisticsReportDto> GetVoucherStatisticsReportAsync(VoucherReportQueryDto query);

    /// <summary>
    /// 憑證分析報表查詢
    /// </summary>
    Task<PagedResult<VoucherDto>> GetVoucherAnalysisReportAsync(VoucherReportQueryDto query);
}

