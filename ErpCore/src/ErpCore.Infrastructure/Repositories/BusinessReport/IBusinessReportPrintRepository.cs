using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表列印 Repository 介面 (SYSL150)
/// </summary>
public interface IBusinessReportPrintRepository
{
    /// <summary>
    /// 查詢業務報表列印列表
    /// </summary>
    Task<PagedResult<BusinessReportPrint>> QueryAsync(BusinessReportPrintQuery query);

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    Task<BusinessReportPrint?> GetByIdAsync(long tKey);

    /// <summary>
    /// 新增業務報表列印
    /// </summary>
    Task<long> CreateAsync(BusinessReportPrint entity);

    /// <summary>
    /// 修改業務報表列印
    /// </summary>
    Task<bool> UpdateAsync(BusinessReportPrint entity);

    /// <summary>
    /// 刪除業務報表列印
    /// </summary>
    Task<bool> DeleteAsync(long tKey);

    /// <summary>
    /// 批次審核
    /// </summary>
    Task<int> BatchAuditAsync(List<long> tKeys, string status, string? verifier, DateTime verifyDate, string? notes);

    /// <summary>
    /// 複製下一年度資料
    /// </summary>
    Task<int> CopyNextYearAsync(int sourceYear, int targetYear, string? siteId);

    /// <summary>
    /// 計算數量
    /// </summary>
    Task<decimal> CalculateQtyAsync(long tKey, Dictionary<string, object>? calculationRules);
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

