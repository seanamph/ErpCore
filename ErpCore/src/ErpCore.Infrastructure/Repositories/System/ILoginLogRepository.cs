using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者異常登入記錄 Repository 介面 (SYS0760)
/// </summary>
public interface ILoginLogRepository
{
    /// <summary>
    /// 查詢異常登入記錄列表（分頁）
    /// </summary>
    Task<PagedResult<LoginLog>> QueryAsync(LoginLogQuery query);

    /// <summary>
    /// 刪除異常登入記錄
    /// </summary>
    Task<int> DeleteAsync(List<long> tKeys);

    /// <summary>
    /// 取得異常事件代碼選項
    /// </summary>
    Task<List<EventTypeOption>> GetEventTypesAsync();
}

/// <summary>
/// 異常登入記錄查詢條件 (SYS0760)
/// </summary>
public class LoginLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy1 { get; set; }
    public string? SortOrder1 { get; set; } = "ASC";
    public string? SortBy2 { get; set; }
    public string? SortOrder2 { get; set; } = "ASC";
    public string? SortBy3 { get; set; }
    public string? SortOrder3 { get; set; } = "ASC";
    public List<string>? EventIds { get; set; }
    public string? UserId { get; set; }
    public DateTime? EventTimeFrom { get; set; }
    public DateTime? EventTimeTo { get; set; }
}

/// <summary>
/// 異常事件代碼選項
/// </summary>
public class EventTypeOption
{
    public string Tag { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
