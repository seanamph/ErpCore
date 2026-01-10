namespace ErpCore.Application.Commands.Roles;

/// <summary>
/// 刪除角色命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class DeleteRoleCommand
{
    /// <summary>
    /// 角色編號
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 更新者（執行刪除的使用者）
    /// </summary>
    public string? UpdatedBy { get; set; }
}

