namespace ErpCore.Application.Commands.Users;

/// <summary>
/// 變更密碼命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class ChangePasswordCommand
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 舊密碼
    /// </summary>
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密碼
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}

