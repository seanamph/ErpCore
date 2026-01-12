using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Shared.Common;
using System.Data;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 單位領用申請單 Repository 介面 (SYSA210)
/// </summary>
public interface IMaterialApplyRepository
{
    /// <summary>
    /// 查詢領用申請列表
    /// </summary>
    Task<PagedResult<MaterialApplyMaster>> GetListAsync(MaterialApplyQuery query);

    /// <summary>
    /// 根據領用單號查詢領用申請（含明細）
    /// </summary>
    Task<MaterialApplyMaster?> GetByApplyIdAsync(string applyId);

    /// <summary>
    /// 新增領用申請
    /// </summary>
    Task<MaterialApplyMaster> CreateAsync(MaterialApplyMaster master);

    /// <summary>
    /// 更新領用申請
    /// </summary>
    Task<MaterialApplyMaster> UpdateAsync(MaterialApplyMaster master);

    /// <summary>
    /// 刪除領用申請
    /// </summary>
    Task DeleteAsync(string applyId);

    /// <summary>
    /// 檢查領用單號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string applyId);

    /// <summary>
    /// 產生領用單號
    /// </summary>
    Task<string> GenerateApplyIdAsync();
}

/// <summary>
/// 單位領用申請單查詢條件
/// </summary>
public class MaterialApplyQuery : PagedQuery
{
    public string? ApplyId { get; set; }
    public string? EmpId { get; set; }
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public DateTime? ApplyDateFrom { get; set; }
    public DateTime? ApplyDateTo { get; set; }
    public DateTime? AprvDateFrom { get; set; }
    public DateTime? AprvDateTo { get; set; }
    public DateTime? CheckDate { get; set; }
    public string? ApplyStatus { get; set; }
    public string? GoodsId { get; set; }
    public string? WhId { get; set; }
    public string? StoreId { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}
