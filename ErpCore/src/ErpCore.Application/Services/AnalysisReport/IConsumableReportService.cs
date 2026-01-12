using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 耗材管理報表服務介面 (SYSA255)
/// </summary>
public interface IConsumableReportService
{
    /// <summary>
    /// 查詢耗材管理報表
    /// </summary>
    Task<ConsumableReportResponseDto> GetReportAsync(ConsumableReportQueryDto query);

    /// <summary>
    /// 匯出耗材管理報表
    /// </summary>
    Task<byte[]> ExportReportAsync(ConsumableReportExportDto exportDto);

    /// <summary>
    /// 查詢耗材使用明細
    /// </summary>
    Task<PagedResult<ConsumableTransactionDto>> GetTransactionsAsync(ConsumableTransactionQueryDto query);
}
