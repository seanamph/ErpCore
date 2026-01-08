using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者權限代理 Repository 介面
/// </summary>
public interface IUserAgentRepository
{
    /// <summary>
    /// 根據代理編號查詢
    /// </summary>
    Task<UserAgent?> GetByIdAsync(Guid agentId);

    /// <summary>
    /// 查詢代理列表（分頁）
    /// </summary>
    Task<PagedResult<UserAgent>> QueryAsync(UserAgentQuery query);

    /// <summary>
    /// 查詢委託人的代理記錄
    /// </summary>
    Task<PagedResult<UserAgent>> GetByPrincipalUserIdAsync(string principalUserId, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 查詢代理人的代理記錄
    /// </summary>
    Task<PagedResult<UserAgent>> GetByAgentUserIdAsync(string agentUserId, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 查詢有效的代理記錄
    /// </summary>
    Task<List<UserAgent>> GetActiveAgentsAsync(string? principalUserId = null, string? agentUserId = null, DateTime? currentTime = null);

    /// <summary>
    /// 新增代理記錄
    /// </summary>
    Task<UserAgent> CreateAsync(UserAgent userAgent);

    /// <summary>
    /// 修改代理記錄
    /// </summary>
    Task<UserAgent> UpdateAsync(UserAgent userAgent);

    /// <summary>
    /// 刪除代理記錄
    /// </summary>
    Task DeleteAsync(Guid agentId);

    /// <summary>
    /// 批次刪除代理記錄
    /// </summary>
    Task DeleteBatchAsync(List<Guid> agentIds);

    /// <summary>
    /// 檢查代理記錄是否存在
    /// </summary>
    Task<bool> ExistsAsync(Guid agentId);
}

/// <summary>
/// 使用者權限代理查詢條件
/// </summary>
public class UserAgentQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PrincipalUserId { get; set; }
    public string? AgentUserId { get; set; }
    public string? Status { get; set; }
    public DateTime? BeginTimeFrom { get; set; }
    public DateTime? BeginTimeTo { get; set; }
    public DateTime? EndTimeFrom { get; set; }
    public DateTime? EndTimeTo { get; set; }
}

