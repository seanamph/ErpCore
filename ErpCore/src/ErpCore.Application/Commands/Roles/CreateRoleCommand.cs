namespace ErpCore.Application.Commands.Roles;

/// <summary>
/// 建立角色命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class CreateRoleCommand
{
    /// <summary>
    /// 角色編號
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 角色名稱
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }
}

