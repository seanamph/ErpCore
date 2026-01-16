using ErpCore.Domain.Entities.Procurement;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 採購擴展功能 Repository 介面 (SYSP610)
/// </summary>
public interface IPurchaseExtendedFunctionRepository
{
    Task<PurchaseExtendedFunction?> GetByIdAsync(long tKey);
    Task<PurchaseExtendedFunction?> GetByExtFunctionIdAsync(string extFunctionId);
    Task<IEnumerable<PurchaseExtendedFunction>> QueryAsync(PurchaseExtendedFunctionQuery query);
    Task<int> GetCountAsync(PurchaseExtendedFunctionQuery query);
    Task<bool> ExistsAsync(string extFunctionId);
    Task<PurchaseExtendedFunction> CreateAsync(PurchaseExtendedFunction purchaseExtendedFunction);
    Task<PurchaseExtendedFunction> UpdateAsync(PurchaseExtendedFunction purchaseExtendedFunction);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 採購擴展功能查詢條件
/// </summary>
public class PurchaseExtendedFunctionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ExtFunctionId { get; set; }
    public string? ExtFunctionName { get; set; }
    public string? ExtFunctionType { get; set; }
    public string? Status { get; set; }
}
