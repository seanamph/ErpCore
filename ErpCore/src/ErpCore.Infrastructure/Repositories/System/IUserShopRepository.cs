using ErpCore.Domain.Entities.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者總公司/分店權限 Repository 介面 (SYS0113)
/// </summary>
public interface IUserShopRepository
{
    /// <summary>
    /// 根據使用者編號查詢總公司/分店權限列表
    /// </summary>
    Task<List<UserShop>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 新增總公司/分店權限
    /// </summary>
    Task<UserShop> CreateAsync(UserShop userShop);

    /// <summary>
    /// 批次新增總公司/分店權限
    /// </summary>
    Task CreateBatchAsync(List<UserShop> userShops);

    /// <summary>
    /// 刪除使用者的所有總公司/分店權限
    /// </summary>
    Task DeleteByUserIdAsync(string userId);

    /// <summary>
    /// 刪除總公司/分店權限
    /// </summary>
    Task DeleteAsync(long id);
}
