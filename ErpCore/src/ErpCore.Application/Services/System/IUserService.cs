using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 查詢使用者列表
    /// </summary>
    Task<PagedResult<UserDto>> GetUsersAsync(UserQueryDto query);

    /// <summary>
    /// 查詢單筆使用者
    /// </summary>
    Task<UserDto> GetUserAsync(string userId);

    /// <summary>
    /// 新增使用者
    /// </summary>
    Task<string> CreateUserAsync(CreateUserDto dto);

    /// <summary>
    /// 修改使用者
    /// </summary>
    Task UpdateUserAsync(string userId, UpdateUserDto dto);

    /// <summary>
    /// 刪除使用者
    /// </summary>
    Task DeleteUserAsync(string userId);

    /// <summary>
    /// 批次刪除使用者
    /// </summary>
    Task DeleteUsersBatchAsync(BatchDeleteUserDto dto);

    /// <summary>
    /// 修改密碼
    /// </summary>
    Task ChangePasswordAsync(string userId, ChangePasswordDto dto);

    /// <summary>
    /// 驗證使用者編號和密碼 (SYS0130)
    /// </summary>
    Task<UserValidationResultDto> ValidateUserAsync(string userId, string password);

    /// <summary>
    /// 更新使用者帳戶原則 (SYS0130)
    /// </summary>
    Task UpdateAccountPolicyAsync(string userId, string? newPassword, DateTime? endDate);

    /// <summary>
    /// 更新帳號終止日 (SYS0130)
    /// </summary>
    Task UpdateEndDateAsync(string userId, DateTime? endDate);

    /// <summary>
    /// 匯出使用者查詢結果 (SYS0910)
    /// </summary>
    Task<byte[]> ExportUserQueryAsync(UserQueryDto query, string exportFormat);

    /// <summary>
    /// 取得當前登入使用者資訊
    /// </summary>
    Task<UserDto> GetCurrentUserAsync();

    /// <summary>
    /// 重設密碼 (SYS0110)
    /// </summary>
    Task ResetPasswordAsync(string userId, ResetPasswordDto dto);

    /// <summary>
    /// 更新使用者狀態 (SYS0110)
    /// </summary>
    Task UpdateStatusAsync(string userId, UpdateStatusDto dto);
}

