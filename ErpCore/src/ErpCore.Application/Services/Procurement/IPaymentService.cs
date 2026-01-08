using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 付款單服務介面 (SYSP271-SYSP2B0)
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// 查詢付款單列表
    /// </summary>
    Task<PagedResult<PaymentDto>> GetPaymentsAsync(PaymentQueryDto query);

    /// <summary>
    /// 查詢單筆付款單
    /// </summary>
    Task<PaymentDto> GetPaymentByIdAsync(string paymentId);

    /// <summary>
    /// 新增付款單
    /// </summary>
    Task<string> CreatePaymentAsync(CreatePaymentDto dto);

    /// <summary>
    /// 修改付款單
    /// </summary>
    Task UpdatePaymentAsync(string paymentId, UpdatePaymentDto dto);

    /// <summary>
    /// 刪除付款單
    /// </summary>
    Task DeletePaymentAsync(string paymentId);

    /// <summary>
    /// 確認付款單
    /// </summary>
    Task ConfirmPaymentAsync(string paymentId);

    /// <summary>
    /// 檢查付款單是否存在
    /// </summary>
    Task<bool> ExistsAsync(string paymentId);
}

