using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 付款單服務介面 (SYSP271-SYSP2B0)
/// </summary>
public interface IPaymentVoucherService
{
    Task<PagedResult<PaymentVoucherDto>> GetPaymentVouchersAsync(PaymentVoucherQueryDto query);
    Task<PaymentVoucherDto> GetPaymentVoucherByIdAsync(string paymentNo);
    Task<string> CreatePaymentVoucherAsync(CreatePaymentVoucherDto dto);
    Task UpdatePaymentVoucherAsync(string paymentNo, UpdatePaymentVoucherDto dto);
    Task DeletePaymentVoucherAsync(string paymentNo);
    Task ConfirmPaymentVoucherAsync(string paymentNo);
    Task<bool> ExistsAsync(string paymentNo);
}
