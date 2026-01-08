using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.SystemConfig;

/// <summary>
/// 系統作業 Repository 介面
/// </summary>
public interface IConfigProgramRepository
{
    /// <summary>
    /// 根據作業代碼查詢
    /// </summary>
    Task<ConfigProgram?> GetByIdAsync(string programId);

    /// <summary>
    /// 查詢作業列表（分頁）
    /// </summary>
    Task<PagedResult<ConfigProgram>> QueryAsync(ConfigProgramQuery query);

    /// <summary>
    /// 新增作業
    /// </summary>
    Task<ConfigProgram> CreateAsync(ConfigProgram configProgram);

    /// <summary>
    /// 修改作業
    /// </summary>
    Task<ConfigProgram> UpdateAsync(ConfigProgram configProgram);

    /// <summary>
    /// 刪除作業
    /// </summary>
    Task DeleteAsync(string programId);

    /// <summary>
    /// 檢查作業是否存在
    /// </summary>
    Task<bool> ExistsAsync(string programId);

    /// <summary>
    /// 檢查是否有按鈕
    /// </summary>
    Task<bool> HasButtonsAsync(string programId);
}

/// <summary>
/// 系統作業查詢條件
/// </summary>
public class ConfigProgramQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ProgramId { get; set; }
    public string? ProgramName { get; set; }
    public string? SystemId { get; set; }
    public string? SubSystemId { get; set; }
    public string? Status { get; set; }
}

