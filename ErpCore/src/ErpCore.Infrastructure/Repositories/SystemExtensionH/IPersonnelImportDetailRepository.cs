using ErpCore.Domain.Entities.SystemExtensionH;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionH;

/// <summary>
/// 人事匯入明細 Repository 介面 (SYSH3D0_FMI - 人事批量新增)
/// </summary>
public interface IPersonnelImportDetailRepository
{
    Task<IEnumerable<PersonnelImportDetail>> GetByImportIdAsync(string importId);
    Task CreateBatchAsync(IEnumerable<PersonnelImportDetail> details);
    Task<int> GetSuccessCountAsync(string importId);
    Task<int> GetFailCountAsync(string importId);
}

