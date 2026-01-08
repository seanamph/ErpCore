using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表管理 Repository 介面 (SYSL145)
/// </summary>
public interface IBusinessReportManagementRepository
{
    /// <summary>
    /// 根據主鍵查詢
    /// </summary>
    Task<BusinessReportManagement?> GetByIdAsync(long tKey);

    /// <summary>
    /// 查詢業務報表管理列表
    /// </summary>
    Task<PagedResult<BusinessReportManagement>> QueryAsync(BusinessReportManagementQuery query);

    /// <summary>
    /// 新增
    /// </summary>
    Task<BusinessReportManagement> CreateAsync(BusinessReportManagement entity);

    /// <summary>
    /// 修改
    /// </summary>
    Task<BusinessReportManagement> UpdateAsync(BusinessReportManagement entity);

    /// <summary>
    /// 刪除
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 批次刪除
    /// </summary>
    Task<int> BatchDeleteAsync(List<long> tKeys);

    /// <summary>
    /// 檢查重複資料（店別+類型+ID）
    /// </summary>
    Task<BusinessReportManagement?> CheckDuplicateAsync(string siteId, string type, string id, long? excludeTKey = null);
}

/// <summary>
/// 業務報表管理查詢條件
/// </summary>
public class BusinessReportManagementQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SiteId { get; set; }
    public string? Type { get; set; }
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? Status { get; set; }
}

