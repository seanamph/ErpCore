using ErpCore.Domain.Entities.SystemExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.SystemExtension;

/// <summary>
/// 系統擴展報表 Repository 介面 (SYSX140)
/// </summary>
public interface ISystemExtensionReportRepository
{
    /// <summary>
    /// 根據主鍵查詢報表記錄
    /// </summary>
    Task<SystemExtensionReport?> GetByReportIdAsync(long reportId);

    /// <summary>
    /// 查詢報表記錄列表（分頁）
    /// </summary>
    Task<PagedResult<SystemExtensionReport>> QueryAsync(SystemExtensionReportQuery query);

    /// <summary>
    /// 新增報表記錄
    /// </summary>
    Task<SystemExtensionReport> CreateAsync(SystemExtensionReport report);

    /// <summary>
    /// 修改報表記錄
    /// </summary>
    Task<SystemExtensionReport> UpdateAsync(SystemExtensionReport report);

    /// <summary>
    /// 刪除報表記錄
    /// </summary>
    Task DeleteAsync(long reportId);
}

/// <summary>
/// 系統擴展報表查詢條件
/// </summary>
public class SystemExtensionReportQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportName { get; set; }
    public string? ReportType { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? GeneratedBy { get; set; }
}

