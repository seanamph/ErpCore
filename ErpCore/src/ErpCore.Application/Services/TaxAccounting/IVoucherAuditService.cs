using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 暫存傳票審核服務介面 (SYSTA00-SYSTA70)
/// </summary>
public interface IVoucherAuditService
{
    Task<List<ErpCore.Application.DTOs.TaxAccounting.SystemVoucherCountDto>> GetSystemListAsync();
    Task<PagedResult<TmpVoucherDto>> GetTmpVouchersAsync(TmpVoucherQueryDto query);
    Task<TmpVoucherDetailDto> GetTmpVoucherByIdAsync(long tKey);
    Task<TmpVoucherDetailDto> UpdateTmpVoucherAsync(long tKey, UpdateTmpVoucherDto dto);
    Task DeleteTmpVoucherAsync(long tKey);
    Task<TransferVoucherResultDto> TransferTmpVoucherAsync(long tKey, TransferVoucherDto dto);
    Task<BatchTransferResultDto> BatchTransferTmpVouchersAsync(BatchTransferVoucherDto dto);
    Task<UnreviewedCountDto> GetUnreviewedCountAsync(string? typeId = null, string? sysId = null);
}

