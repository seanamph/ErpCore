using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.ReportManagement;

/// <summary>
/// 收款擴展功能服務介面 (SYSR310-SYSR450)
/// </summary>
public interface IReceivingExtensionService
{
    Task<ReceiptVoucherTransferDto> GetReceiptVoucherTransferByIdAsync(long tKey);
    Task<PagedResult<ReceiptVoucherTransferDto>> QueryReceiptVoucherTransferAsync(ReceiptVoucherTransferQueryDto query);
    Task<ReceiptVoucherTransferDto> CreateReceiptVoucherTransferAsync(CreateReceiptVoucherTransferDto dto);
    Task<ReceiptVoucherTransferDto> UpdateReceiptVoucherTransferAsync(long tKey, UpdateReceiptVoucherTransferDto dto);
    Task DeleteReceiptVoucherTransferAsync(long tKey);
    Task<ReceiptVoucherTransferDto> TransferReceiptVoucherAsync(long tKey);
    Task<BatchTransferResultDto> BatchTransferReceiptVoucherAsync(BatchTransferReceiptVoucherDto dto);
}

