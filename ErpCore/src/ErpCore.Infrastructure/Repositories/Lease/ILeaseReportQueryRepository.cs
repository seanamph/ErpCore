using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃報表查詢記錄 Repository 介面 (SYSM141-SYSM144)
/// </summary>
public interface ILeaseReportQueryRepository
{
    Task<LeaseReportQuery?> GetByIdAsync(string queryId);
    Task<IEnumerable<LeaseReportQuery>> QueryAsync(LeaseReportQueryQuery query);
    Task<int> GetCountAsync(LeaseReportQueryQuery query);
    Task<bool> ExistsAsync(string queryId);
    Task<LeaseReportQuery> CreateAsync(LeaseReportQuery reportQuery);
    Task DeleteAsync(string queryId);
}

/// <summary>
/// 租賃報表查詢記錄查詢條件
/// </summary>
public class LeaseReportQueryQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? QueryId { get; set; }
    public string? ReportType { get; set; }
    public string? QueryName { get; set; }
    public DateTime? QueryDateFrom { get; set; }
    public DateTime? QueryDateTo { get; set; }
}

