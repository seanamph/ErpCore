namespace ErpCore.Application.Queries.Users;

/// <summary>
/// 取得使用者查詢
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class GetUserQuery
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}

