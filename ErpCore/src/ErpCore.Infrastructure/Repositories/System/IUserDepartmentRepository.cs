using ErpCore.Domain.Entities.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者部門權限 Repository 介面 (SYS0113)
/// </summary>
public interface IUserDepartmentRepository
{
    /// <summary>
    /// 根據使用者編號查詢部門權限列表
    /// </summary>
    Task<List<UserDepartment>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 新增部門權限
    /// </summary>
    Task<UserDepartment> CreateAsync(UserDepartment userDepartment);

    /// <summary>
    /// 批次新增部門權限
    /// </summary>
    Task CreateBatchAsync(List<UserDepartment> userDepartments);

    /// <summary>
    /// 刪除使用者的所有部門權限
    /// </summary>
    Task DeleteByUserIdAsync(string userId);

    /// <summary>
    /// 刪除部門權限
    /// </summary>
    Task DeleteAsync(long id);
}
