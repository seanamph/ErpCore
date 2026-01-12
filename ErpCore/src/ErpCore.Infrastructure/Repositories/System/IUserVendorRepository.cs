using ErpCore.Domain.Entities.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者廠商權限 Repository 介面 (SYS0113)
/// </summary>
public interface IUserVendorRepository
{
    /// <summary>
    /// 根據使用者編號查詢廠商權限列表
    /// </summary>
    Task<List<UserVendor>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 新增廠商權限
    /// </summary>
    Task<UserVendor> CreateAsync(UserVendor userVendor);

    /// <summary>
    /// 批次新增廠商權限
    /// </summary>
    Task CreateBatchAsync(List<UserVendor> userVendors);

    /// <summary>
    /// 刪除使用者的所有廠商權限
    /// </summary>
    Task DeleteByUserIdAsync(string userId);

    /// <summary>
    /// 刪除廠商權限
    /// </summary>
    Task DeleteAsync(long id);
}
