using ErpCore.Domain.Entities.Communication;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 簡訊記錄儲存庫介面
/// </summary>
public interface ISmsLogRepository
{
    /// <summary>
    /// 建立簡訊記錄
    /// </summary>
    Task<SmsLog> CreateAsync(SmsLog entity);

    /// <summary>
    /// 根據ID查詢簡訊記錄
    /// </summary>
    Task<SmsLog?> GetByIdAsync(long id);

    /// <summary>
    /// 查詢簡訊記錄列表（分頁）
    /// </summary>
    Task<PagedResult<SmsLog>> QueryAsync(SmsLogQuery query);

    /// <summary>
    /// 更新簡訊記錄狀態
    /// </summary>
    Task<bool> UpdateStatusAsync(long id, string status, DateTime? sentAt = null, string? errorMessage = null);
}

/// <summary>
/// 簡訊記錄查詢條件
/// </summary>
public class SmsLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

