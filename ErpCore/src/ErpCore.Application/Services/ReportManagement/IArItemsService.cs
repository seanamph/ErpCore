using ErpCore.Application.DTOs.ReportManagement;

namespace ErpCore.Application.Services.ReportManagement;

/// <summary>
/// 收款項目服務介面 (SYSR110-SYSR120)
/// </summary>
public interface IArItemsService
{
    Task<IEnumerable<ArItemsDto>> GetAllAsync();
    Task<ArItemsDto> GetByIdAsync(long tKey);
    Task<ArItemsDto> GetBySiteIdAndAritemIdAsync(string siteId, string aritemId);
    Task<IEnumerable<ArItemsDto>> GetBySiteIdAsync(string siteId);
    Task<ArItemsDto> CreateAsync(CreateArItemsDto dto);
    Task<ArItemsDto> UpdateAsync(long tKey, UpdateArItemsDto dto);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string siteId, string aritemId);
}

