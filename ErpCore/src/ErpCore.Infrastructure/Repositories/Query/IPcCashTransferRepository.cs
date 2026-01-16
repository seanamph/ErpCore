using ErpCore.Domain.Entities.Query;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 零用金拋轉檔 Repository 接口 (SYSQ230)
/// </summary>
public interface IPcCashTransferRepository
{
    Task<PcCashTransfer?> GetByIdAsync(long tKey);
    Task<PcCashTransfer?> GetByTransferIdAsync(string transferId);
    Task<PagedResult<PcCashTransfer>> QueryAsync(PcCashTransferQueryParams query);
    Task<PcCashTransfer> CreateAsync(PcCashTransfer entity);
    Task<PcCashTransfer> UpdateAsync(PcCashTransfer entity);
    Task DeleteAsync(long tKey);
    Task<string> GenerateTransferIdAsync(string? siteId);
}

