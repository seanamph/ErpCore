using ErpCore.Domain.Entities.Procurement;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 供應商 Repository 介面 (SYSP210-SYSP260)
/// </summary>
public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(string supplierId);
    Task<IEnumerable<Supplier>> QueryAsync(SupplierQuery query);
    Task<int> GetCountAsync(SupplierQuery query);
    Task<bool> ExistsAsync(string supplierId);
    Task<Supplier> CreateAsync(Supplier supplier);
    Task<Supplier> UpdateAsync(Supplier supplier);
    Task DeleteAsync(string supplierId);
}

/// <summary>
/// 供應商查詢條件
/// </summary>
public class SupplierQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string? Status { get; set; }
    public string? Rating { get; set; }
}

