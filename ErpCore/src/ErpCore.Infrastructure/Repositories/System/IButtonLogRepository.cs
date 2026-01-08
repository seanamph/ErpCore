using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 按鈕操作記錄 Repository 介面 (SYS0790)
/// </summary>
public interface IButtonLogRepository
{
    /// <summary>
    /// 查詢按鈕操作記錄列表（分頁）
    /// </summary>
    Task<PagedResult<ButtonLog>> QueryAsync(ButtonLogQuery query);

    /// <summary>
    /// 新增按鈕操作記錄
    /// </summary>
    Task<ButtonLog> CreateAsync(ButtonLog buttonLog);
}

/// <summary>
/// 按鈕操作記錄查詢條件 (SYS0790)
/// </summary>
public class ButtonLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public List<string>? UserIds { get; set; }
    public string? ProgId { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}

