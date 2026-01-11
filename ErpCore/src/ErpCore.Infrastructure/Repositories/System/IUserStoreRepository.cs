using ErpCore.Domain.Entities.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者7X承租分店權限 Repository 介面 (SYS0111)
/// </summary>
public interface IUserStoreRepository
{
    /// <summary>
    /// 根據使用者編號查詢7X承租分店權限列表
    /// </summary>
    Task<List<UserStore>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 新增7X承租分店權限
    /// </summary>
    Task<UserStore> CreateAsync(UserStore userStore);

    /// <summary>
    /// 批次新增7X承租分店權限
    /// </summary>
    Task CreateBatchAsync(List<UserStore> userStores);

    /// <summary>
    /// 刪除使用者的所有7X承租分店權限
    /// </summary>
    Task DeleteByUserIdAsync(string userId);

    /// <summary>
    /// 刪除7X承租分店權限
    /// </summary>
    Task DeleteAsync(long id);
}
