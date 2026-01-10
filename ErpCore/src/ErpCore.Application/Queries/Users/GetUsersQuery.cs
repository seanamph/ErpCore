namespace ErpCore.Application.Queries.Users;

/// <summary>
/// 取得使用者列表查詢
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class GetUsersQuery
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
    /// 使用者編號（篩選）
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 使用者名稱（篩選）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 組織代碼（篩選）
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 狀態（篩選）
    /// </summary>
    public string? Status { get; set; }
}

