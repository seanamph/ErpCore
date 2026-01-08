using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 費用主檔 Repository 介面 (SYSE310-SYSE430)
/// </summary>
public interface ILeaseFeeRepository
{
    Task<LeaseFee?> GetByIdAsync(string feeId);
    Task<IEnumerable<LeaseFee>> GetByLeaseIdAsync(string leaseId);
    Task<IEnumerable<LeaseFee>> QueryAsync(LeaseFeeQuery query);
    Task<int> GetCountAsync(LeaseFeeQuery query);
    Task<bool> ExistsAsync(string feeId);
    Task<LeaseFee> CreateAsync(LeaseFee fee);
    Task<LeaseFee> UpdateAsync(LeaseFee fee);
    Task DeleteAsync(string feeId);
    Task UpdateStatusAsync(string feeId, string status);
    Task UpdatePaidAmountAsync(string feeId, decimal paidAmount, DateTime? paidDate);
}

/// <summary>
/// 費用查詢條件
/// </summary>
public class LeaseFeeQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? FeeId { get; set; }
    public string? LeaseId { get; set; }
    public string? FeeType { get; set; }
    public string? Status { get; set; }
    public DateTime? FeeDateFrom { get; set; }
    public DateTime? FeeDateTo { get; set; }
}

