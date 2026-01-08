namespace ErpCore.Application.DTOs.Core;

/// <summary>
/// 使用者資料 DTO
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 職稱
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 最後登入IP
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 更新使用者資料 DTO
/// </summary>
public class UpdateUserProfileDto
{
    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 職稱
    /// </summary>
    public string? Title { get; set; }
}

/// <summary>
/// 修改密碼 DTO
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// 舊密碼
    /// </summary>
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密碼
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 確認密碼
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// 重置所有使用者密碼 DTO
/// </summary>
public class ResetAllPasswordsDto
{
    /// <summary>
    /// 管理員密碼
    /// </summary>
    public string AdminPassword { get; set; } = string.Empty;

    /// <summary>
    /// 預設密碼
    /// </summary>
    public string? DefaultPassword { get; set; }
}

