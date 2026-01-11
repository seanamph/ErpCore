namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者資料傳輸物件
/// </summary>
public class UserDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? OrgId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string? LastLoginIp { get; set; }
    public string Status { get; set; } = "A";
    public string? UserType { get; set; }
    public string? Notes { get; set; }
    public int? UserPriority { get; set; }
    public string? ShopId { get; set; }
    public int? LoginCount { get; set; }
    public DateTime? ChangePwdDate { get; set; }
    public string? FloorId { get; set; }
    public string? AreaId { get; set; }
    public string? BtypeId { get; set; }
    public string? StoreId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 使用者查詢 DTO
/// </summary>
public class UserQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; }
    public string? UserType { get; set; }
    public string? Title { get; set; }
    public string? ShopId { get; set; }
}

/// <summary>
/// 新增使用者 DTO
/// </summary>
public class CreateUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserPassword { get; set; }
    public string? Title { get; set; }
    public string? OrgId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "A";
    public string? UserType { get; set; }
    public string? Notes { get; set; }
    public int? UserPriority { get; set; }
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public string? AreaId { get; set; }
    public string? BtypeId { get; set; }
    public string? StoreId { get; set; }
}

/// <summary>
/// 修改使用者 DTO
/// </summary>
public class UpdateUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? OrgId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "A";
    public string? UserType { get; set; }
    public string? Notes { get; set; }
    public int? UserPriority { get; set; }
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public string? AreaId { get; set; }
    public string? BtypeId { get; set; }
    public string? StoreId { get; set; }
}

/// <summary>
/// 修改密碼 DTO
/// </summary>
public class ChangePasswordDto
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// 批次刪除使用者 DTO
/// </summary>
public class BatchDeleteUserDto
{
    public List<string> UserIds { get; set; } = new();
}

/// <summary>
/// 使用者驗證 DTO (SYS0130)
/// </summary>
public class UserValidationDto
{
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 使用者驗證結果 DTO (SYS0130)
/// </summary>
public class UserValidationResultDto
{
    public bool IsValid { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

/// <summary>
/// 更新帳戶原則 DTO (SYS0130)
/// </summary>
public class UpdateAccountPolicyDto
{
    public string Password { get; set; } = string.Empty;
    public string? NewPassword { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 更新帳號終止日 DTO (SYS0130)
/// </summary>
public class UpdateEndDateDto
{
    public string Password { get; set; } = string.Empty;
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 重設密碼 DTO (SYS0110)
/// </summary>
public class ResetPasswordDto
{
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// 更新狀態 DTO (SYS0110)
/// </summary>
public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty; // A:啟用, I:停用, L:鎖定
}

