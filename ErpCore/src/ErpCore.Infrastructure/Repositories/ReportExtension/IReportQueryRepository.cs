using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.ReportExtension;

/// <summary>
/// 報表查詢儲存庫介面
/// </summary>
public interface IReportQueryRepository
{
    Task<ReportQuery?> GetByIdAsync(Guid queryId);
    Task<ReportQuery?> GetByReportCodeAsync(string reportCode);
    Task<PagedResult<ReportQuery>> QueryAsync(ReportQueryListQuery query);
    Task<ReportQuery> CreateAsync(ReportQuery entity);
    Task<bool> UpdateAsync(ReportQuery entity);
    Task<bool> DeleteAsync(Guid queryId);
    Task<ReportQueryLog> CreateLogAsync(ReportQueryLog log);
    Task<PagedResult<ReportQueryLog>> QueryLogsAsync(ReportQueryLogQuery query);
}

/// <summary>
/// 報表查詢列表查詢參數
/// </summary>
public class ReportQueryListQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportCode { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 報表查詢記錄查詢參數
/// </summary>
public class ReportQueryLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportCode { get; set; }
    public string? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

