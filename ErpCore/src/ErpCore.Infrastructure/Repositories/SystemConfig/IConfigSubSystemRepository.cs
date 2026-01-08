using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.SystemConfig;

/// <summary>
/// 子系統項目 Repository 介面
/// </summary>
public interface IConfigSubSystemRepository
{
    /// <summary>
    /// 根據子系統代碼查詢
    /// </summary>
    Task<ConfigSubSystem?> GetByIdAsync(string subSystemId);

    /// <summary>
    /// 查詢子系統列表（分頁）
    /// </summary>
    Task<PagedResult<ConfigSubSystem>> QueryAsync(ConfigSubSystemQuery query);

    /// <summary>
    /// 新增子系統
    /// </summary>
    Task<ConfigSubSystem> CreateAsync(ConfigSubSystem configSubSystem);

    /// <summary>
    /// 修改子系統
    /// </summary>
    Task<ConfigSubSystem> UpdateAsync(ConfigSubSystem configSubSystem);

    /// <summary>
    /// 刪除子系統
    /// </summary>
    Task DeleteAsync(string subSystemId);

    /// <summary>
    /// 檢查子系統是否存在
    /// </summary>
    Task<bool> ExistsAsync(string subSystemId);

    /// <summary>
    /// 檢查是否有下層子系統
    /// </summary>
    Task<bool> HasChildrenAsync(string subSystemId);

    /// <summary>
    /// 檢查是否有作業
    /// </summary>
    Task<bool> HasProgramsAsync(string subSystemId);
}

/// <summary>
/// 子系統查詢條件
/// </summary>
public class ConfigSubSystemQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SubSystemId { get; set; }
    public string? SubSystemName { get; set; }
    public string? SystemId { get; set; }
    public string? ParentSubSystemId { get; set; }
    public string? Status { get; set; }
}

