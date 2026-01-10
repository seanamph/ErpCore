namespace ErpCore.Application.Commands.Users;

/// <summary>
/// 重設密碼命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class ResetPasswordCommand
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 新密碼
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 執行重設的使用者
    /// </summary>
    public string? UpdatedBy { get; set; }
}

