using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 系統作業 Repository 介面 (SYS0430)
/// </summary>
public interface IProgramRepository
{
    /// <summary>
    /// 根據作業代碼查詢
    /// </summary>
    Task<Program?> GetByIdAsync(string programId);

    /// <summary>
    /// 查詢作業列表（分頁）
    /// </summary>
    Task<PagedResult<Program>> QueryAsync(ProgramQuery query);

    /// <summary>
    /// 新增作業
    /// </summary>
    Task<Program> CreateAsync(Program program);

    /// <summary>
    /// 修改作業
    /// </summary>
    Task<Program> UpdateAsync(Program program);

    /// <summary>
    /// 刪除作業
    /// </summary>
    Task DeleteAsync(string programId);

    /// <summary>
    /// 檢查作業是否存在
    /// </summary>
    Task<bool> ExistsAsync(string programId);

    /// <summary>
    /// 檢查是否有按鈕關聯
    /// </summary>
    Task<bool> HasButtonsAsync(string programId);
}

/// <summary>
/// 作業查詢條件
/// </summary>
public class ProgramQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ProgramId { get; set; }
    public string? ProgramName { get; set; }
    public string? MenuId { get; set; }
    public string? ProgramType { get; set; }
}
