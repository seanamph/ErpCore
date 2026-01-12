using ErpCore.Domain.Entities.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者按鈕權限 Repository 介面 (SYS0113)
/// </summary>
public interface IUserButtonRepository
{
    /// <summary>
    /// 根據使用者編號查詢按鈕權限列表
    /// </summary>
    Task<List<UserButton>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 新增按鈕權限
    /// </summary>
    Task<UserButton> CreateAsync(UserButton userButton);

    /// <summary>
    /// 批次新增按鈕權限
    /// </summary>
    Task CreateBatchAsync(List<UserButton> userButtons);

    /// <summary>
    /// 刪除使用者的所有按鈕權限
    /// </summary>
    Task DeleteByUserIdAsync(string userId);

    /// <summary>
    /// 刪除按鈕權限
    /// </summary>
    Task DeleteAsync(long tKey);
}
