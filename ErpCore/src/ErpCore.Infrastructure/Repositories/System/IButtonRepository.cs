using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 系統功能按鈕 Repository 介面 (SYS0440)
/// </summary>
public interface IButtonRepository
{
    /// <summary>
    /// 根據主鍵查詢
    /// </summary>
    Task<Button?> GetByIdAsync(long tKey);

    /// <summary>
    /// 查詢按鈕列表（分頁）
    /// </summary>
    Task<PagedResult<Button>> QueryAsync(ButtonQuery query);

    /// <summary>
    /// 新增按鈕
    /// </summary>
    Task<Button> CreateAsync(Button button);

    /// <summary>
    /// 修改按鈕
    /// </summary>
    Task<Button> UpdateAsync(Button button);

    /// <summary>
    /// 刪除按鈕
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 檢查是否有權限設定
    /// </summary>
    Task<bool> HasPermissionsAsync(long tKey);
}

/// <summary>
/// 系統功能按鈕查詢條件
/// </summary>
public class ButtonQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ProgramId { get; set; }
    public string? ButtonId { get; set; }
    public string? ButtonName { get; set; }
    public string? PageId { get; set; }
}
