using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 使用者管理服務實作
/// </summary>
public class UserManagementService : BaseService, IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public UserManagementService(
        IUserRepository userRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _userRepository = userRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<UserProfileDto> GetUserProfileAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            return new UserProfileDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = null, // User 實體中沒有 Email 欄位
                Phone = null, // User 實體中沒有 Phone 欄位
                Department = null, // User 實體中沒有 Department 欄位
                Title = user.Title,
                Status = user.Status,
                LastLoginAt = user.LastLoginDate,
                LastLoginIp = user.LastLoginIp,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateUserProfileAsync(UpdateUserProfileDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 更新允許修改的欄位
            // 注意：User 實體中沒有 Email、Phone、Department 欄位，這些資訊可能需要儲存在其他資料表
            if (!string.IsNullOrEmpty(dto.Title))
            {
                user.Title = dto.Title;
            }

            user.UpdatedBy = GetCurrentUserId();
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            _logger.LogInfo($"使用者 {userId} 更新個人資料成功");
        }
        catch (Exception ex)
        {
            _logger.LogError("更新使用者資料失敗", ex);
            throw;
        }
    }

    public async Task ChangePasswordAsync(ChangePasswordDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 驗證舊密碼
            var hashedOldPassword = HashPassword(dto.OldPassword);
            if (user.UserPassword != hashedOldPassword)
            {
                throw new InvalidOperationException("舊密碼不正確");
            }

            // 驗證新密碼格式
            if (string.IsNullOrEmpty(dto.NewPassword) || dto.NewPassword.Length < 6)
            {
                throw new InvalidOperationException("新密碼長度至少需要6個字元");
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                throw new InvalidOperationException("新密碼與確認密碼不一致");
            }

            // 更新密碼
            var hashedNewPassword = HashPassword(dto.NewPassword);
            await _userRepository.UpdatePasswordAsync(userId, hashedNewPassword);
            _logger.LogInfo($"使用者 {userId} 修改密碼成功");
        }
        catch (Exception ex)
        {
            _logger.LogError("修改密碼失敗", ex);
            throw;
        }
    }

    public async Task ResetAllPasswordsAsync(ResetAllPasswordsDto dto)
    {
        try
        {
            // 驗證權限（只有管理員可以執行此操作）
            var currentUser = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(currentUser);
            
            if (user == null || user.UserType != "ADMIN")
            {
                throw new UnauthorizedAccessException("只有管理員可以執行此操作");
            }

            // 驗證管理員密碼
            var hashedAdminPassword = HashPassword(dto.AdminPassword);
            if (user.UserPassword != hashedAdminPassword)
            {
                throw new InvalidOperationException("管理員密碼不正確");
            }

            // 重置所有使用者密碼為預設密碼
            var defaultPassword = dto.DefaultPassword ?? "123456";
            var hashedDefaultPassword = HashPassword(defaultPassword);

            await _userRepository.ResetAllPasswordsAsync(hashedDefaultPassword, currentUser);
            _logger.LogInfo($"管理員 {currentUser} 重置所有使用者密碼成功");
        }
        catch (Exception ex)
        {
            _logger.LogError("重置所有使用者密碼失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 密碼雜湊（SHA256）
    /// </summary>
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

