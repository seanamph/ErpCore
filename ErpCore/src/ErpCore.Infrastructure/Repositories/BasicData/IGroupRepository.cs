using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 組別 Repository 介面
/// </summary>
public interface IGroupRepository
{
    /// <summary>
    /// 根據組別代碼查詢組別
    /// </summary>
    Task<Group?> GetByIdAsync(string groupId);

    /// <summary>
    /// 查詢組別列表（分頁）
    /// </summary>
    Task<PagedResult<Group>> QueryAsync(GroupQuery query);

    /// <summary>
    /// 新增組別
    /// </summary>
    Task<Group> CreateAsync(Group group);

    /// <summary>
    /// 修改組別
    /// </summary>
    Task<Group> UpdateAsync(Group group);

    /// <summary>
    /// 刪除組別
    /// </summary>
    Task DeleteAsync(string groupId);

    /// <summary>
    /// 檢查組別是否存在
    /// </summary>
    Task<bool> ExistsAsync(string groupId);
}

/// <summary>
/// 組別查詢條件
/// </summary>
public class GroupQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }
    public string? DeptId { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; }
}

