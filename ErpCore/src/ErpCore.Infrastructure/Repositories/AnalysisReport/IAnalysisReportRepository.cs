using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 進銷存分析報表 Repository 介面 (SYSA1000)
/// </summary>
public interface IAnalysisReportRepository
{
    /// <summary>
    /// 查詢進銷存分析報表資料
    /// </summary>
    Task<PagedResult<AnalysisReportItemDto>> GetAnalysisReportAsync(string reportId, AnalysisReportQueryDto query);

    /// <summary>
    /// 查詢耗材列表（用於列印）
    /// </summary>
    Task<List<Consumable>> GetConsumablesForPrintAsync(ConsumablePrintQueryDto query);
}

/// <summary>
/// 耗材列印查詢 DTO
/// </summary>
public class ConsumablePrintQueryDto
{
    public string? Type { get; set; } // 1:耗材管理報表, 2:耗材標籤列印
    public string? Status { get; set; }
    public string? SiteId { get; set; }
    public string? AssetStatus { get; set; }
    public List<string>? ConsumableIds { get; set; }
}

