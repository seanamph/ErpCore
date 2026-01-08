using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.ReportManagement;

/// <summary>
/// 收款其他功能服務介面 (SYSR510-SYSR570)
/// </summary>
public interface IReceivingOtherService
{
    Task<DepositsDto> GetDepositByIdAsync(long tKey);
    Task<PagedResult<DepositsDto>> QueryDepositsAsync(DepositsQueryDto query);
    Task<DepositsDto> CreateDepositAsync(CreateDepositsDto dto);
    Task<DepositsDto> UpdateDepositAsync(long tKey, UpdateDepositsDto dto);
    Task DeleteDepositAsync(long tKey);
    Task<DepositsDto> ReturnDepositAsync(long tKey, ReturnDepositsDto dto);
    Task<DepositsDto> DepositAmountAsync(long tKey, DepositAmountDto dto);
    Task<bool> ExistsDepositNoAsync(string depositNo);
}

