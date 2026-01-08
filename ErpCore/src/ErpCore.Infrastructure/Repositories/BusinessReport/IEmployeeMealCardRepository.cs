using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 員工餐卡申請 Repository 介面 (SYSL130)
/// </summary>
public interface IEmployeeMealCardRepository
{
    /// <summary>
    /// 根據主鍵查詢員工餐卡申請
    /// </summary>
    Task<EmployeeMealCard?> GetByIdAsync(long tKey);

    /// <summary>
    /// 查詢員工餐卡申請列表（分頁）
    /// </summary>
    Task<PagedResult<EmployeeMealCard>> QueryAsync(EmployeeMealCardQuery query);

    /// <summary>
    /// 新增員工餐卡申請
    /// </summary>
    Task<EmployeeMealCard> CreateAsync(EmployeeMealCard mealCard);

    /// <summary>
    /// 修改員工餐卡申請
    /// </summary>
    Task<EmployeeMealCard> UpdateAsync(EmployeeMealCard mealCard);

    /// <summary>
    /// 刪除員工餐卡申請
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 批次審核員工餐卡申請
    /// </summary>
    Task<int> BatchVerifyAsync(List<long> tKeys, string action, string verifier, string? notes);
}

/// <summary>
/// 員工餐卡申請查詢條件
/// </summary>
public class EmployeeMealCardQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? EmpId { get; set; }
    public string? EmpName { get; set; }
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
}

