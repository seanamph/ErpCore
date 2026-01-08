using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者權限代理服務介面
/// </summary>
public interface IUserAgentService
{
    /// <summary>
    /// 查詢代理列表
    /// </summary>
    Task<PagedResult<UserAgentDto>> GetUserAgentsAsync(UserAgentQueryDto query);

    /// <summary>
    /// 查詢單筆代理記錄
    /// </summary>
    Task<UserAgentDto> GetUserAgentAsync(Guid agentId);

    /// <summary>
    /// 查詢委託人的代理記錄
    /// </summary>
    Task<PagedResult<UserAgentDto>> GetUserAgentsByPrincipalAsync(string userId, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 查詢代理人的代理記錄
    /// </summary>
    Task<PagedResult<UserAgentDto>> GetUserAgentsByAgentAsync(string userId, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 查詢有效代理記錄
    /// </summary>
    Task<PagedResult<UserAgentDto>> GetActiveUserAgentsAsync(string? principalUserId = null, string? agentUserId = null, DateTime? currentTime = null);

    /// <summary>
    /// 新增代理記錄
    /// </summary>
    Task<Guid> CreateUserAgentAsync(CreateUserAgentDto dto);

    /// <summary>
    /// 修改代理記錄
    /// </summary>
    Task UpdateUserAgentAsync(Guid agentId, UpdateUserAgentDto dto);

    /// <summary>
    /// 刪除代理記錄
    /// </summary>
    Task DeleteUserAgentAsync(Guid agentId);

    /// <summary>
    /// 批次刪除代理記錄
    /// </summary>
    Task DeleteUserAgentsBatchAsync(BatchDeleteUserAgentDto dto);

    /// <summary>
    /// 更新代理記錄狀態
    /// </summary>
    Task UpdateUserAgentStatusAsync(Guid agentId, UpdateUserAgentStatusDto dto);

    /// <summary>
    /// 檢查代理權限
    /// </summary>
    Task<CheckAgentPermissionResultDto> CheckAgentPermissionAsync(CheckAgentPermissionDto dto);
}

