using ErpCore.Shared.Common;
using SystemExtensionEntity = ErpCore.Domain.Entities.SystemExtension.SystemExtension;

namespace ErpCore.Infrastructure.Repositories.SystemExtension;

/// <summary>
/// 系統擴展 Repository 介面 (SYSX110, SYSX120, SYSX140)
/// </summary>
public interface ISystemExtensionRepository
{
    /// <summary>
    /// 根據主鍵查詢系統擴展
    /// </summary>
    Task<SystemExtensionEntity?> GetByTKeyAsync(long tKey);

    /// <summary>
    /// 根據擴展功能代碼查詢系統擴展
    /// </summary>
    Task<SystemExtensionEntity?> GetByExtensionIdAsync(string extensionId);

    /// <summary>
    /// 查詢系統擴展列表（分頁）
    /// </summary>
    Task<PagedResult<SystemExtensionEntity>> QueryAsync(SystemExtensionQuery query);

    /// <summary>
    /// 新增系統擴展
    /// </summary>
    Task<SystemExtensionEntity> CreateAsync(SystemExtensionEntity systemExtension);

    /// <summary>
    /// 修改系統擴展
    /// </summary>
    Task<SystemExtensionEntity> UpdateAsync(SystemExtensionEntity systemExtension);

    /// <summary>
    /// 刪除系統擴展
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 檢查擴展功能代碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string extensionId);

    /// <summary>
    /// 查詢統計資訊
    /// </summary>
    Task<SystemExtensionStatistics> GetStatisticsAsync(SystemExtensionStatisticsQuery query);
}

/// <summary>
/// 系統擴展查詢條件
/// </summary>
public class SystemExtensionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ExtensionId { get; set; }
    public string? ExtensionName { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
}

/// <summary>
/// 系統擴展統計查詢條件
/// </summary>
public class SystemExtensionStatisticsQuery
{
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 系統擴展統計結果
/// </summary>
public class SystemExtensionStatistics
{
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
    public List<SystemExtensionTypeCount> ByType { get; set; } = new();
}

/// <summary>
/// 系統擴展類型統計
/// </summary>
public class SystemExtensionTypeCount
{
    public string ExtensionType { get; set; } = string.Empty;
    public int Count { get; set; }
}

