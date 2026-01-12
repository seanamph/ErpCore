using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 耗材列印服務介面 (SYSA254)
/// </summary>
public interface IConsumablePrintService
{
    /// <summary>
    /// 查詢耗材列表（用於列印）
    /// </summary>
    Task<ConsumablePrintListDto> GetPrintListAsync(ConsumablePrintQueryDto query);

    /// <summary>
    /// 批次列印耗材標籤
    /// </summary>
    Task<BatchPrintResponseDto> BatchPrintAsync(BatchPrintDto dto, string userId);

    /// <summary>
    /// 查詢列印記錄列表
    /// </summary>
    Task<PagedResult<ConsumablePrintLogDto>> GetPrintLogsAsync(ConsumablePrintLogQueryDto query);
}
