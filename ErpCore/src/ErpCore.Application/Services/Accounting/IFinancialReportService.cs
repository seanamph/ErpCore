using ErpCore.Application.DTOs.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Accounting;

/// <summary>
/// 財務報表服務介面 (SYSN510-SYSN540)
/// </summary>
public interface IFinancialReportService
{
    /// <summary>
    /// 查詢財務報表
    /// </summary>
    Task<PagedResult<FinancialReportDto>> GetFinancialReportsAsync(FinancialReportQueryDto query);

    /// <summary>
    /// 匯出財務報表
    /// </summary>
    Task<byte[]> ExportFinancialReportsAsync(ExportFinancialReportDto dto);
}

