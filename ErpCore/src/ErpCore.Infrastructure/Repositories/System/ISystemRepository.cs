using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 主系統項目 Repository 介面 (SYS0410)
/// </summary>
public interface ISystemRepository
{
    /// <summary>
    /// 根據主系統代碼查詢
    /// </summary>
    Task<Systems?> GetByIdAsync(string systemId);

    /// <summary>
    /// 查詢主系統列表（分頁）
    /// </summary>
    Task<PagedResult<Systems>> QueryAsync(SystemQuery query);

    /// <summary>
    /// 新增主系統
    /// </summary>
    Task<Systems> CreateAsync(Systems system);

    /// <summary>
    /// 修改主系統
    /// </summary>
    Task<Systems> UpdateAsync(Systems system);

    /// <summary>
    /// 刪除主系統
    /// </summary>
    Task DeleteAsync(string systemId);

    /// <summary>
    /// 檢查主系統是否存在
    /// </summary>
    Task<bool> ExistsAsync(string systemId);

    /// <summary>
    /// 檢查是否有子系統關聯
    /// </summary>
    Task<bool> HasSubSystemsAsync(string systemId);

    /// <summary>
    /// 檢查是否有作業關聯
    /// </summary>
    Task<bool> HasProgramsAsync(string systemId);
}

/// <summary>
/// 主系統查詢條件
/// </summary>
public class SystemQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SystemId { get; set; }
    public string? SystemName { get; set; }
    public string? SystemType { get; set; }
    public string? ServerIp { get; set; }
    public string? Status { get; set; }
}
