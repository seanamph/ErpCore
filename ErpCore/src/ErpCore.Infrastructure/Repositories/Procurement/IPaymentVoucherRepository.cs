using ErpCore.Domain.Entities.Procurement;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 付款單 Repository 介面 (SYSP271-SYSP2B0)
/// </summary>
public interface IPaymentVoucherRepository
{
    Task<PaymentVoucher?> GetByIdAsync(string paymentNo);
    Task<IEnumerable<PaymentVoucher>> QueryAsync(PaymentVoucherQuery query);
    Task<int> GetCountAsync(PaymentVoucherQuery query);
    Task<bool> ExistsAsync(string paymentNo);
    Task<PaymentVoucher> CreateAsync(PaymentVoucher paymentVoucher);
    Task<PaymentVoucher> UpdateAsync(PaymentVoucher paymentVoucher);
    Task DeleteAsync(string paymentNo);
}

/// <summary>
/// 付款單查詢條件
/// </summary>
public class PaymentVoucherQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? PaymentNo { get; set; }
    public DateTime? PaymentDateFrom { get; set; }
    public DateTime? PaymentDateTo { get; set; }
    public string? SupplierId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Status { get; set; }
}
