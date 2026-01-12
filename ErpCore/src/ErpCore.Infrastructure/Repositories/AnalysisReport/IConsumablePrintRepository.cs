using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 耗材列印 Repository 介面 (SYSA254)
/// </summary>
public interface IConsumablePrintRepository
{
    /// <summary>
    /// 查詢耗材列表（用於列印）
    /// </summary>
    Task<List<Consumable>> GetConsumablesForPrintAsync(ConsumablePrintQuery query);

    /// <summary>
    /// 建立列印記錄
    /// </summary>
    Task<ConsumablePrintLog> CreateLogAsync(ConsumablePrintLog log);

    /// <summary>
    /// 查詢列印記錄列表
    /// </summary>
    Task<PagedResult<ConsumablePrintLog>> GetLogsAsync(ConsumablePrintLogQuery query);
}

/// <summary>
/// 耗材列印查詢條件
/// </summary>
public class ConsumablePrintQuery
{
    public string? Type { get; set; } // 1:耗材管理報表, 2:耗材標籤列印
    public string? Status { get; set; }
    public string? SiteId { get; set; }
    public string? AssetStatus { get; set; }
    public List<string>? ConsumableIds { get; set; }
}

/// <summary>
/// 耗材列印記錄查詢條件
/// </summary>
public class ConsumablePrintLogQuery : PagedQuery
{
    public string? ConsumableId { get; set; }
    public string? PrintType { get; set; }
    public string? SiteId { get; set; }
    public string? PrintedBy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
