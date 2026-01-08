using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃會計分類服務介面 (SYSE110-SYSE140)
/// </summary>
public interface ILeaseAccountingCategoryService
{
    Task<PagedResult<LeaseAccountingCategoryDto>> GetLeaseAccountingCategoriesAsync(LeaseAccountingCategoryQueryDto query);
    Task<LeaseAccountingCategoryDto> GetLeaseAccountingCategoryByIdAsync(long tKey);
    Task<IEnumerable<LeaseAccountingCategoryDto>> GetLeaseAccountingCategoriesByLeaseIdAndVersionAsync(string leaseId, string version);
    Task<LeaseAccountingCategoryDto> CreateLeaseAccountingCategoryAsync(CreateLeaseAccountingCategoryDto dto);
    Task UpdateLeaseAccountingCategoryAsync(long tKey, UpdateLeaseAccountingCategoryDto dto);
    Task DeleteLeaseAccountingCategoryAsync(long tKey);
    Task DeleteLeaseAccountingCategoriesByLeaseIdAndVersionAsync(string leaseId, string version);
    Task<bool> ExistsAsync(long tKey);
}

