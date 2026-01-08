using ErpCore.Domain.Entities.Query;
using ErpCore.Application.DTOs.Query;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 零用金主檔 Repository 接口 (SYSQ210)
/// </summary>
public interface IPcCashRepository
{
    Task<PcCash?> GetByIdAsync(long tKey);
    Task<PcCash?> GetByCashIdAsync(string cashId);
    Task<PagedResult<PcCashDto>> QueryAsync(PcCashQueryDto query);
    Task<PcCash> CreateAsync(PcCash entity);
    Task<PcCash> UpdateAsync(PcCash entity);
    Task DeleteAsync(long tKey);
    Task<string> GenerateCashIdAsync(string? siteId);
}

