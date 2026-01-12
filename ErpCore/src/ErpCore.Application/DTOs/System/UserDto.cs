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
    
    /// <summary>
    /// 是否使用Active Directory (SYS0114)
    /// </summary>
    public bool UseActiveDirectory { get; set; }
    
    /// <summary>
    /// AD網域 (SYS0114)
    /// </summary>
    public string? AdDomain { get; set; }
    
    /// <summary>
    /// AD使用者主體名稱 (SYS0114)
    /// </summary>
    public string? AdUserPrincipalName { get; set; }
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
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public DateTime? LastLoginDateFrom { get; set; }
    public DateTime? LastLoginDateTo { get; set; }
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

/// <summary>
/// 使用者組織權限 DTO (SYS0114)
/// </summary>
public class UserOrganizationDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public string? OrgName { get; set; }
}

/// <summary>
/// 新增使用者組織權限 DTO (SYS0114)
/// </summary>
public class CreateUserOrganizationDto
{
    public string OrgId { get; set; } = string.Empty;
}

/// <summary>
/// 新增使用者（含AD和組織設定）DTO (SYS0114)
/// </summary>
public class CreateUserWithAdOrgsDto : CreateUserDto
{
    public bool UseActiveDirectory { get; set; }
    public string? AdDomain { get; set; }
    public string? AdUserPrincipalName { get; set; }
    public List<CreateUserBusinessTypeDto> BusinessTypes { get; set; } = new();
    public List<CreateUserOrganizationDto> Organizations { get; set; } = new();
}

/// <summary>
/// 修改使用者（含AD和組織設定）DTO (SYS0114)
/// </summary>
public class UpdateUserWithAdOrgsDto : UpdateUserDto
{
    public bool UseActiveDirectory { get; set; }
    public string? AdDomain { get; set; }
    public string? AdUserPrincipalName { get; set; }
    public List<CreateUserBusinessTypeDto> BusinessTypes { get; set; } = new();
    public List<CreateUserOrganizationDto> Organizations { get; set; } = new();
}

/// <summary>
/// 使用者詳細資料（含AD和組織）DTO (SYS0114)
/// </summary>
public class UserDetailWithAdOrgsDto : UserDetailDto
{
    public bool UseActiveDirectory { get; set; }
    public string? AdDomain { get; set; }
    public string? AdUserPrincipalName { get; set; }
    public List<UserOrganizationDto> Organizations { get; set; } = new();
}

/// <summary>
/// 驗證Active Directory使用者請求 DTO (SYS0114)
/// </summary>
public class ValidateAdUserDto
{
    public string AdDomain { get; set; } = string.Empty;
    public string AdUserPrincipalName { get; set; } = string.Empty;
}

/// <summary>
/// 驗證Active Directory使用者回應 DTO (SYS0114)
/// </summary>
public class ValidateAdUserResultDto
{
    public bool Exists { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
}

/// <summary>
/// 組織 DTO (SYS0114)
/// </summary>
public class OrganizationDto
{
    public string OrgId { get; set; } = string.Empty;
    public string OrgName { get; set; } = string.Empty;
}

