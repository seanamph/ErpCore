using ErpCore.Domain.Entities.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者儲位權限 Repository 介面 (SYS0111)
/// </summary>
public interface IUserWarehouseAreaRepository
{
    /// <summary>
    /// 根據使用者編號查詢儲位權限列表
    /// </summary>
    Task<List<UserWarehouseArea>> GetByUserIdAsync(string userId);

    /// <summary>
    /// 新增儲位權限
    /// </summary>
    Task<UserWarehouseArea> CreateAsync(UserWarehouseArea userWarehouseArea);

    /// <summary>
    /// 批次新增儲位權限
    /// </summary>
    Task CreateBatchAsync(List<UserWarehouseArea> userWarehouseAreas);

    /// <summary>
    /// 刪除使用者的所有儲位權限
    /// </summary>
    Task DeleteByUserIdAsync(string userId);

    /// <summary>
    /// 刪除儲位權限
    /// </summary>
    Task DeleteAsync(long id);
}
