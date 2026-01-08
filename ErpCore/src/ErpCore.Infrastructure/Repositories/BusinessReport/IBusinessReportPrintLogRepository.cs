using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表列印記錄 Repository 介面 (SYSL161)
/// </summary>
public interface IBusinessReportPrintLogRepository
{
    /// <summary>
    /// 查詢業務報表列印記錄列表
    /// </summary>
    Task<PagedResult<BusinessReportPrintLog>> QueryAsync(BusinessReportPrintLogQuery query);

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    Task<BusinessReportPrintLog?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據 ReportId 查詢列印記錄列表
    /// </summary>
    Task<List<BusinessReportPrintLog>> GetByReportIdAsync(string reportId);

    /// <summary>
    /// 新增業務報表列印記錄
    /// </summary>
    Task<long> CreateAsync(BusinessReportPrintLog entity);

    /// <summary>
    /// 修改業務報表列印記錄
    /// </summary>
    Task<bool> UpdateAsync(BusinessReportPrintLog entity);

    /// <summary>
    /// 刪除業務報表列印記錄
    /// </summary>
    Task<bool> DeleteAsync(long tKey);
}

/// <summary>
/// 業務報表列印記錄查詢條件
/// </summary>
public class BusinessReportPrintLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ReportId { get; set; }
    public string? ReportName { get; set; }
    public string? PrintUserId { get; set; }
    public DateTime? PrintDateFrom { get; set; }
    public DateTime? PrintDateTo { get; set; }
    public string? Status { get; set; }
}

