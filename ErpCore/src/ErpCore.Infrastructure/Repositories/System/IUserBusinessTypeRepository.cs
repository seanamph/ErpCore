using ErpCore.Domain.Entities.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者業種權限 Repository 介面 (SYS0111)
/// </summary>
public interface IUserBusinessTypeRepository
{
    /// <summary>
    /// 根據使用者編號查詢業種權限列表
    /// </summary>
    Task<List<UserBusinessType>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 新增業種權限
    /// </summary>
    Task<UserBusinessType> CreateAsync(UserBusinessType userBusinessType);

    /// <summary>
    /// 批次新增業種權限
    /// </summary>
    Task CreateBatchAsync(List<UserBusinessType> userBusinessTypes);

    /// <summary>
    /// 刪除使用者的所有業種權限
    /// </summary>
    Task DeleteByUserIdAsync(string userId);

    /// <summary>
    /// 刪除業種權限
    /// </summary>
    Task DeleteAsync(long id);
}
