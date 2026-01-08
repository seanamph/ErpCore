using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者 Repository 介面
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根據使用者編號查詢
    /// </summary>
    Task<User?> GetByIdAsync(string userId);

    /// <summary>
    /// 查詢使用者列表（分頁）
    /// </summary>
    Task<PagedResult<User>> QueryAsync(UserQuery query);

    /// <summary>
    /// 新增使用者
    /// </summary>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// 修改使用者
    /// </summary>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// 刪除使用者
    /// </summary>
    Task DeleteAsync(string userId);

    /// <summary>
    /// 檢查使用者是否存在
    /// </summary>
    Task<bool> ExistsAsync(string userId);

    /// <summary>
    /// 更新密碼
    /// </summary>
    Task UpdatePasswordAsync(string userId, string hashedPassword);

    /// <summary>
    /// 更新登入資訊
    /// </summary>
    Task UpdateLoginInfoAsync(string userId, string ipAddress);
}

/// <summary>
/// 使用者查詢條件
/// </summary>
public class UserQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; }
    public string? UserType { get; set; }
    public string? Title { get; set; }
    public string? ShopId { get; set; }
}

