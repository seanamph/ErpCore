using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 進銷存分析報表服務介面 (SYSA1000)
/// </summary>
public interface IAnalysisReportService
{
    /// <summary>
    /// 查詢進銷存分析報表
    /// </summary>
    Task<AnalysisReportDto> GetAnalysisReportAsync(string reportId, AnalysisReportQueryDto query);

    /// <summary>
    /// 匯出進銷存分析報表
    /// </summary>
    Task<byte[]> ExportAnalysisReportAsync(string reportId, ExportAnalysisReportDto dto);

    /// <summary>
    /// 列印進銷存分析報表
    /// </summary>
    Task<byte[]> PrintAnalysisReportAsync(string reportId, PrintAnalysisReportDto dto);

    /// <summary>
    /// 查詢耗材列表（用於列印）
    /// </summary>
    Task<List<Consumable>> GetConsumablesForPrintAsync(ConsumablePrintQueryDto query);
}

