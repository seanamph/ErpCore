using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 供應商服務介面 (SYSP210-SYSP260)
/// </summary>
public interface ISupplierService
{
    Task<PagedResult<SupplierDto>> GetSuppliersAsync(SupplierQueryDto query);
    Task<SupplierDto> GetSupplierByIdAsync(string supplierId);
    Task<string> CreateSupplierAsync(CreateSupplierDto dto);
    Task UpdateSupplierAsync(string supplierId, UpdateSupplierDto dto);
    Task DeleteSupplierAsync(string supplierId);
    Task<bool> ExistsAsync(string supplierId);
}

