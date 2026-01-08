using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.ReportExtension;

/// <summary>
/// 報表列印記錄儲存庫介面
/// </summary>
public interface IReportPrintLogRepository
{
    Task<ReportPrintLog?> GetByIdAsync(long printLogId);
    Task<PagedResult<ReportPrintLog>> QueryAsync(ReportPrintLogQuery query);
    Task<ReportPrintLog> CreateAsync(ReportPrintLog entity);
    Task<bool> UpdateAsync(ReportPrintLog entity);
}

/// <summary>
/// 報表列印記錄查詢參數
/// </summary>
public class ReportPrintLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportCode { get; set; }
    public string? PrintStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

