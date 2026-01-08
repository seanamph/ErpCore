using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.SystemConfig;

/// <summary>
/// 主系統項目 Repository 介面
/// </summary>
public interface IConfigSystemRepository
{
    /// <summary>
    /// 根據主系統代碼查詢
    /// </summary>
    Task<ConfigSystem?> GetByIdAsync(string systemId);

    /// <summary>
    /// 查詢主系統列表（分頁）
    /// </summary>
    Task<PagedResult<ConfigSystem>> QueryAsync(ConfigSystemQuery query);

    /// <summary>
    /// 新增主系統
    /// </summary>
    Task<ConfigSystem> CreateAsync(ConfigSystem configSystem);

    /// <summary>
    /// 修改主系統
    /// </summary>
    Task<ConfigSystem> UpdateAsync(ConfigSystem configSystem);

    /// <summary>
    /// 刪除主系統
    /// </summary>
    Task DeleteAsync(string systemId);

    /// <summary>
    /// 檢查主系統是否存在
    /// </summary>
    Task<bool> ExistsAsync(string systemId);
}

/// <summary>
/// 主系統查詢條件
/// </summary>
public class ConfigSystemQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SystemId { get; set; }
    public string? SystemName { get; set; }
    public string? SystemType { get; set; }
    public string? Status { get; set; }
}

