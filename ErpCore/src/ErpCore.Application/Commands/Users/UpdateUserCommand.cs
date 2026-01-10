namespace ErpCore.Application.Commands.Users;

/// <summary>
/// 更新使用者命令
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class UpdateUserCommand
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 職稱
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 帳號有效起始日
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 帳號終止日
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 帳號狀態
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }
}

