using ErpCore.Domain.Entities.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者組織權限 Repository 介面 (SYS0114)
/// </summary>
public interface IUserOrganizationRepository
{
    /// <summary>
    /// 根據使用者編號查詢組織權限列表
    /// </summary>
    Task<List<UserOrganization>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 新增組織權限
    /// </summary>
    Task<UserOrganization> CreateAsync(UserOrganization userOrganization);

    /// <summary>
    /// 批次新增組織權限
    /// </summary>
    Task CreateBatchAsync(List<UserOrganization> userOrganizations);

    /// <summary>
    /// 刪除使用者的所有組織權限
    /// </summary>
    Task DeleteByUserIdAsync(string userId);

    /// <summary>
    /// 刪除組織權限
    /// </summary>
    Task DeleteAsync(long id);
}
