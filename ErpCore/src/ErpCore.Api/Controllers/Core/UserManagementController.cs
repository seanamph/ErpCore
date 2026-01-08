using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Core;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Core;

/// <summary>
/// 使用者管理控制器
/// 提供使用者資料維護、密碼修改、密碼重置等功能
/// </summary>
[Route("api/v1/core/user-management")]
public class UserManagementController : BaseController
{
    private readonly IUserManagementService _service;

    public UserManagementController(
        IUserManagementService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢當前使用者資料 (USER_PROFILE)
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetUserProfile()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserProfileAsync();
            return result;
        }, "查詢使用者資料失敗");
    }

    /// <summary>
    /// 更新使用者資料 (USER_PROFILE)
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUserProfile(
        [FromBody] UpdateUserProfileDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateUserProfileAsync(dto);
        }, "更新使用者資料失敗");
    }

    /// <summary>
    /// 修改密碼 (ch_passwd)
    /// </summary>
    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(
        [FromBody] ChangePasswordDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ChangePasswordAsync(dto);
        }, "修改密碼失敗");
    }

    /// <summary>
    /// 重置所有使用者密碼 (ResetAllPassword)
    /// </summary>
    [HttpPost("reset-all-passwords")]
    public async Task<ActionResult<ApiResponse<object>>> ResetAllPasswords(
        [FromBody] ResetAllPasswordsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ResetAllPasswordsAsync(dto);
        }, "重置所有使用者密碼失敗");
    }
}

