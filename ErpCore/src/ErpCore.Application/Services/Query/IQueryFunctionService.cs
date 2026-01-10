using ErpCore.Application.DTOs.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 查詢功能服務介面 (SYSQ000)
/// </summary>
public interface IQueryFunctionService
{
    Task<PagedResult<QueryFunctionDto>> GetQueryFunctionsAsync(QueryFunctionQueryDto query);
    Task<QueryFunctionDto> GetQueryFunctionByIdAsync(long tKey);
    Task<QueryFunctionDto> GetQueryFunctionByQueryIdAsync(string queryId);
    Task<long> CreateQueryFunctionAsync(CreateQueryFunctionDto dto);
    Task UpdateQueryFunctionAsync(long tKey, UpdateQueryFunctionDto dto);
    Task DeleteQueryFunctionAsync(long tKey);
    Task<QueryResultDto> ExecuteQueryFunctionAsync(long tKey, ExecuteQueryDto dto);
    Task UpdateStatusAsync(long tKey, string status);
}

