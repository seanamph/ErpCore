using ErpCore.Domain.Entities.Lease;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 費用項目主檔 Repository 介面 (SYSE310-SYSE430)
/// </summary>
public interface ILeaseFeeItemRepository
{
    Task<LeaseFeeItem?> GetByIdAsync(string feeItemId);
    Task<IEnumerable<LeaseFeeItem>> QueryAsync(LeaseFeeItemQuery query);
    Task<int> GetCountAsync(LeaseFeeItemQuery query);
    Task<bool> ExistsAsync(string feeItemId);
    Task<LeaseFeeItem> CreateAsync(LeaseFeeItem feeItem);
    Task<LeaseFeeItem> UpdateAsync(LeaseFeeItem feeItem);
    Task DeleteAsync(string feeItemId);
    Task UpdateStatusAsync(string feeItemId, string status);
}

/// <summary>
/// 費用項目查詢條件
/// </summary>
public class LeaseFeeItemQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? FeeItemId { get; set; }
    public string? FeeItemName { get; set; }
    public string? FeeType { get; set; }
    public string? Status { get; set; }
}

