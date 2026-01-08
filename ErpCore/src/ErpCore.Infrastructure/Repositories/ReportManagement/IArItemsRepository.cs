using ErpCore.Domain.Entities.ReportManagement;

namespace ErpCore.Infrastructure.Repositories.ReportManagement;

/// <summary>
/// 收款項目 Repository 介面 (SYSR110-SYSR120)
/// </summary>
public interface IArItemsRepository
{
    Task<ArItems?> GetByIdAsync(long tKey);
    Task<ArItems?> GetBySiteIdAndAritemIdAsync(string siteId, string aritemId);
    Task<IEnumerable<ArItems>> GetAllAsync();
    Task<IEnumerable<ArItems>> GetBySiteIdAsync(string siteId);
    Task<ArItems> CreateAsync(ArItems entity);
    Task<ArItems> UpdateAsync(ArItems entity);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string siteId, string aritemId);
}

