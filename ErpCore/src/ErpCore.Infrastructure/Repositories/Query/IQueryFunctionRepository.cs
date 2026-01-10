using ErpCore.Domain.Entities.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 查詢功能 Repository 介面 (SYSQ000)
/// </summary>
public interface IQueryFunctionRepository
{
    Task<QueryFunction?> GetByIdAsync(long tKey);
    Task<QueryFunction?> GetByQueryIdAsync(string queryId);
    Task<PagedResult<QueryFunction>> GetPagedAsync(QueryFunctionQuery query);
    Task<QueryFunction> CreateAsync(QueryFunction queryFunction);
    Task<QueryFunction> UpdateAsync(QueryFunction queryFunction);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string queryId);
}

/// <summary>
/// 查詢功能查詢條件
/// </summary>
public class QueryFunctionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? QueryId { get; set; }
    public string? QueryName { get; set; }
    public string? QueryType { get; set; }
    public string? Status { get; set; }
}

