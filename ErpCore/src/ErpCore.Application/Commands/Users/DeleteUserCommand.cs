namespace ErpCore.Application.Commands.Users;

/// <summary>
/// 刪除使用者命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class DeleteUserCommand
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 更新者（執行刪除的使用者）
    /// </summary>
    public string? UpdatedBy { get; set; }
}

