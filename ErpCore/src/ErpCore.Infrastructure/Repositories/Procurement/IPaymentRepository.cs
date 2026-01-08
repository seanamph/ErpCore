using ErpCore.Domain.Entities.Procurement;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 付款單 Repository 介面 (SYSP271-SYSP2B0)
/// </summary>
public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(string paymentId);
    Task<IEnumerable<Payment>> QueryAsync(PaymentQuery query);
    Task<int> GetCountAsync(PaymentQuery query);
    Task<bool> ExistsAsync(string paymentId);
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
    Task DeleteAsync(string paymentId);
}

/// <summary>
/// 付款單查詢條件
/// </summary>
public class PaymentQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? PaymentId { get; set; }
    public DateTime? PaymentDateFrom { get; set; }
    public DateTime? PaymentDateTo { get; set; }
    public string? SupplierId { get; set; }
    public string? PaymentType { get; set; }
    public string? Status { get; set; }
}

