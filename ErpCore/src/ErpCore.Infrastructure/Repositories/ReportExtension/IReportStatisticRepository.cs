using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.ReportExtension;

/// <summary>
/// 報表統計記錄儲存庫介面
/// </summary>
public interface IReportStatisticRepository
{
    Task<ReportStatistic?> GetByIdAsync(long statisticId);
    Task<PagedResult<ReportStatistic>> QueryAsync(ReportStatisticQuery query);
    Task<ReportStatistic> CreateAsync(ReportStatistic entity);
}

/// <summary>
/// 報表統計查詢參數
/// </summary>
public class ReportStatisticQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportCode { get; set; }
    public string? StatisticType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

