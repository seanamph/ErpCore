using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 費用主檔服務介面 (SYSE310-SYSE430)
/// </summary>
public interface ILeaseFeeService
{
    Task<PagedResult<LeaseFeeDto>> GetLeaseFeesAsync(LeaseFeeQueryDto query);
    Task<LeaseFeeDto> GetLeaseFeeByIdAsync(string feeId);
    Task<IEnumerable<LeaseFeeDto>> GetLeaseFeesByLeaseIdAsync(string leaseId);
    Task<LeaseFeeDto> CreateLeaseFeeAsync(CreateLeaseFeeDto dto);
    Task UpdateLeaseFeeAsync(string feeId, UpdateLeaseFeeDto dto);
    Task DeleteLeaseFeeAsync(string feeId);
    Task UpdateLeaseFeeStatusAsync(string feeId, string status);
    Task UpdateLeaseFeePaidAmountAsync(string feeId, decimal paidAmount, DateTime? paidDate);
    Task<bool> ExistsAsync(string feeId);
}

