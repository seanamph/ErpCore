using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.SystemConfig;

/// <summary>
/// 系統功能按鈕 Repository 介面
/// </summary>
public interface IConfigButtonRepository
{
    /// <summary>
    /// 根據按鈕代碼查詢
    /// </summary>
    Task<ConfigButton?> GetByIdAsync(string buttonId);

    /// <summary>
    /// 查詢按鈕列表（分頁）
    /// </summary>
    Task<PagedResult<ConfigButton>> QueryAsync(ConfigButtonQuery query);

    /// <summary>
    /// 新增按鈕
    /// </summary>
    Task<ConfigButton> CreateAsync(ConfigButton configButton);

    /// <summary>
    /// 修改按鈕
    /// </summary>
    Task<ConfigButton> UpdateAsync(ConfigButton configButton);

    /// <summary>
    /// 刪除按鈕
    /// </summary>
    Task DeleteAsync(string buttonId);

    /// <summary>
    /// 檢查按鈕是否存在
    /// </summary>
    Task<bool> ExistsAsync(string buttonId);
}

/// <summary>
/// 系統功能按鈕查詢條件
/// </summary>
public class ConfigButtonQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ButtonId { get; set; }
    public string? ButtonName { get; set; }
    public string? ProgramId { get; set; }
    public string? ButtonType { get; set; }
    public string? Status { get; set; }
}

