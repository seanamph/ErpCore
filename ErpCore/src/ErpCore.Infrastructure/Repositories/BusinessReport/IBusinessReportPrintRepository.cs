using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表列印 Repository 介面 (SYSL150)
/// </summary>
public interface IBusinessReportPrintRepository
{
    /// <summary>
    /// 根據主鍵查詢
    /// </summary>
    Task<BusinessReportPrint?> GetByIdAsync(long tKey);

    /// <summary>
    /// 查詢業務報表列印列表
    /// </summary>
    Task<PagedResult<BusinessReportPrint>> QueryAsync(BusinessReportPrintQuery query);

    /// <summary>
    /// 新增
    /// </summary>
    Task<BusinessReportPrint> CreateAsync(BusinessReportPrint entity);

    /// <summary>
    /// 修改
    /// </summary>
    Task<BusinessReportPrint> UpdateAsync(BusinessReportPrint entity);

    /// <summary>
    /// 刪除
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 批次刪除
    /// </summary>
    Task<int> BatchDeleteAsync(List<long> tKeys);

    /// <summary>
    /// 批次審核
    /// </summary>
    Task<int> BatchAuditAsync(List<long> tKeys, string status, string verifier, DateTime verifyDate, string? notes = null);

    /// <summary>
    /// 複製下一年度資料
    /// </summary>
    Task<int> CopyNextYearAsync(int sourceYear, int targetYear, string? siteId = null);

    /// <summary>
    /// 檢查年度是否已審核（用於年度修改唯讀控制）
    /// </summary>
    Task<bool> IsYearAuditedAsync(int giveYear, string? siteId = null);
}

/// <summary>
/// 業務報表列印查詢條件
/// </summary>
public class BusinessReportPrintQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public int? GiveYear { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? EmpId { get; set; }
    public string? Status { get; set; }
}
