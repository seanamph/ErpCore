using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 部別 Repository 介面
/// </summary>
public interface IDepartmentRepository
{
    /// <summary>
    /// 根據部別代碼查詢部別
    /// </summary>
    Task<Department?> GetByIdAsync(string deptId);

    /// <summary>
    /// 查詢部別列表（分頁）
    /// </summary>
    Task<PagedResult<Department>> QueryAsync(DepartmentQuery query);

    /// <summary>
    /// 新增部別
    /// </summary>
    Task<Department> CreateAsync(Department department);

    /// <summary>
    /// 修改部別
    /// </summary>
    Task<Department> UpdateAsync(Department department);

    /// <summary>
    /// 刪除部別
    /// </summary>
    Task DeleteAsync(string deptId);

    /// <summary>
    /// 檢查部別是否存在
    /// </summary>
    Task<bool> ExistsAsync(string deptId);
}

/// <summary>
/// 部別查詢條件
/// </summary>
public class DepartmentQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? DeptId { get; set; }
    public string? DeptName { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; }
}

