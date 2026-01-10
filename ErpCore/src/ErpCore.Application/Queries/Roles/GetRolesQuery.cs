namespace ErpCore.Application.Queries.Roles;

/// <summary>
/// 取得角色列表查詢
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class GetRolesQuery
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序欄位
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向
    /// </summary>
    public string SortOrder { get; set; } = "ASC";

    /// <summary>
    /// 角色編號（篩選）
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// 角色名稱（篩選）
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 狀態（篩選）
    /// </summary>
    public string? Status { get; set; }
}

