using ErpCore.Domain.Entities.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 零用金請款檔 Repository 接口 (SYSQ220)
/// </summary>
public interface IPcCashRequestRepository
{
    Task<PcCashRequest?> GetByIdAsync(long tKey);
    Task<PcCashRequest?> GetByRequestIdAsync(string requestId);
    Task<PagedResult<PcCashRequest>> QueryAsync(PcCashRequestQueryParams query);
    Task<PcCashRequest> CreateAsync(PcCashRequest entity);
    Task<PcCashRequest> UpdateAsync(PcCashRequest entity);
    Task DeleteAsync(long tKey);
    Task<string> GenerateRequestIdAsync(string? siteId);
}

