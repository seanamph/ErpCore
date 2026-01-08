using ErpCore.Domain.Entities.Pos;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Pos;

/// <summary>
/// POS同步記錄 Repository 介面
/// </summary>
public interface IPosSyncLogRepository
{
    /// <summary>
    /// 根據ID查詢
    /// </summary>
    Task<PosSyncLog?> GetByIdAsync(long id);

    /// <summary>
    /// 查詢POS同步記錄列表（分頁）
    /// </summary>
    Task<PagedResult<PosSyncLog>> QueryAsync(PosSyncLogQuery query);

    /// <summary>
    /// 新增POS同步記錄
    /// </summary>
    Task<PosSyncLog> CreateAsync(PosSyncLog log);

    /// <summary>
    /// 修改POS同步記錄
    /// </summary>
    Task<PosSyncLog> UpdateAsync(PosSyncLog log);
}

/// <summary>
/// POS同步記錄查詢條件
/// </summary>
public class PosSyncLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SyncType { get; set; }
    public string? SyncDirection { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

