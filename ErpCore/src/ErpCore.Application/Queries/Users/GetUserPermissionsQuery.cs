namespace ErpCore.Application.Queries.Users;

/// <summary>
/// 取得使用者權限查詢
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class GetUserPermissionsQuery
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 系統代碼（可選）
    /// </summary>
    public string? SystemId { get; set; }
}

