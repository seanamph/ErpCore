using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 員餐卡欄位 Repository 介面 (SYSL206/SYSL207)
/// </summary>
public interface IEmployeeMealCardFieldRepository
{
    /// <summary>
    /// 查詢員餐卡欄位列表
    /// </summary>
    Task<PagedResult<EmployeeMealCardField>> QueryAsync(EmployeeMealCardFieldQuery query);

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    Task<EmployeeMealCardField?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據欄位ID查詢單筆資料
    /// </summary>
    Task<EmployeeMealCardField?> GetByFieldIdAsync(string fieldId);

    /// <summary>
    /// 查詢上一筆資料（根據欄位ID）
    /// </summary>
    Task<EmployeeMealCardField?> GetPreviousAsync(string fieldId);

    /// <summary>
    /// 新增員餐卡欄位
    /// </summary>
    Task<long> CreateAsync(EmployeeMealCardField entity);

    /// <summary>
    /// 修改員餐卡欄位
    /// </summary>
    Task<bool> UpdateAsync(EmployeeMealCardField entity);

    /// <summary>
    /// 刪除員餐卡欄位
    /// </summary>
    Task<bool> DeleteAsync(long tKey);
}

/// <summary>
/// 員餐卡欄位查詢條件
/// </summary>
public class EmployeeMealCardFieldQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? FieldId { get; set; }
    public string? FieldName { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? OtherType { get; set; }
    public string? Status { get; set; }
}

