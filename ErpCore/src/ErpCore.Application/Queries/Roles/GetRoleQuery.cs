namespace ErpCore.Application.Queries.Roles;

/// <summary>
/// 取得角色查詢
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class GetRoleQuery
{
    /// <summary>
    /// 角色編號
    /// </summary>
    public string RoleId { get; set; } = string.Empty;
}

