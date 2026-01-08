using ErpCore.Application.DTOs.Core;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 使用者管理服務介面
/// </summary>
public interface IUserManagementService
{
    /// <summary>
    /// 查詢當前使用者資料
    /// </summary>
    Task<UserProfileDto> GetUserProfileAsync();

    /// <summary>
    /// 更新使用者資料
    /// </summary>
    Task UpdateUserProfileAsync(UpdateUserProfileDto dto);

    /// <summary>
    /// 修改密碼
    /// </summary>
    Task ChangePasswordAsync(ChangePasswordDto dto);

    /// <summary>
    /// 重置所有使用者密碼
    /// </summary>
    Task ResetAllPasswordsAsync(ResetAllPasswordsDto dto);
}

