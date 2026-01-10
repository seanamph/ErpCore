namespace ErpCore.Api.ViewModels.Request;

/// <summary>
/// 使用者請求模型
/// </summary>
public class UserRequest
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<string>? RoleIds { get; set; }
}

