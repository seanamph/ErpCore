using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 使用者基本資料維護作業控制器 (SYS0110)
/// </summary>
[Route("api/v1/users")]
public class UsersController : BaseController
{
    private readonly IUserService _service;

    public UsersController(
        IUserService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢使用者列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetUsers(
        [FromQuery] UserQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUsersAsync(query);
            return result;
        }, "查詢使用者列表失敗");
    }

    /// <summary>
    /// 查詢單筆使用者
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(string userId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserAsync(userId);
            return result;
        }, $"查詢使用者失敗: {userId}");
    }

    /// <summary>
    /// 新增使用者
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateUser(
        [FromBody] CreateUserDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateUserAsync(dto);
            return result;
        }, "新增使用者失敗");
    }

    /// <summary>
    /// 修改使用者
    /// </summary>
    [HttpPut("{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUser(
        string userId,
        [FromBody] UpdateUserDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateUserAsync(userId, dto);
        }, $"修改使用者失敗: {userId}");
    }

    /// <summary>
    /// 刪除使用者
    /// </summary>
    [HttpDelete("{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(string userId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteUserAsync(userId);
        }, $"刪除使用者失敗: {userId}");
    }

    /// <summary>
    /// 批次刪除使用者
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUsersBatch(
        [FromBody] BatchDeleteUserDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteUsersBatchAsync(dto);
        }, "批次刪除使用者失敗");
    }

    /// <summary>
    /// 修改密碼
    /// </summary>
    [HttpPut("{userId}/password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(
        string userId,
        [FromBody] ChangePasswordDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ChangePasswordAsync(userId, dto);
        }, $"修改密碼失敗: {userId}");
    }

    /// <summary>
    /// 使用者資料瀏覽（只讀模式）(SYS0120)
    /// </summary>
    [HttpGet("browse")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> BrowseUsers(
        [FromQuery] UserQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUsersAsync(query);
            return result;
        }, "瀏覽使用者列表失敗");
    }

    /// <summary>
    /// 使用者資料瀏覽（單筆，只讀模式）(SYS0120)
    /// </summary>
    [HttpGet("{userId}/browse")]
    public async Task<ActionResult<ApiResponse<UserDto>>> BrowseUser(string userId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserAsync(userId);
            return result;
        }, $"瀏覽使用者失敗: {userId}");
    }

    /// <summary>
    /// 驗證使用者編號和密碼 (SYS0130)
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<UserValidationResultDto>>> ValidateUser(
        [FromBody] UserValidationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ValidateUserAsync(dto.UserId, dto.Password);
            if (!result.IsValid)
            {
                return new ApiResponse<UserValidationResultDto>
                {
                    Success = false,
                    Code = 400,
                    Message = result.ErrorCode == "USER_INVALID" ? "使用者編號錯誤" : "使用者密碼錯誤",
                    Data = result
                };
            }
            return new ApiResponse<UserValidationResultDto>
            {
                Success = true,
                Code = 200,
                Message = "驗證成功",
                Data = result
            };
        }, "驗證使用者失敗");
    }

    /// <summary>
    /// 修改使用者帳戶原則 (SYS0130)
    /// </summary>
    [HttpPut("{userId}/account-policy")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAccountPolicy(
        string userId,
        [FromBody] UpdateAccountPolicyDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證使用者編號和密碼
            var validation = await _service.ValidateUserAsync(userId, dto.Password);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(
                    validation.ErrorCode == "USER_INVALID" ? "使用者編號錯誤" : "使用者密碼錯誤");
            }

            // 修改帳戶原則
            await _service.UpdateAccountPolicyAsync(userId, dto.NewPassword, dto.EndDate);
        }, $"修改使用者帳戶原則失敗: {userId}");
    }

    /// <summary>
    /// 僅修改帳號終止日 (SYS0130)
    /// </summary>
    [HttpPut("{userId}/end-date")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateEndDate(
        string userId,
        [FromBody] UpdateEndDateDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            // 驗證使用者編號和密碼
            var validation = await _service.ValidateUserAsync(userId, dto.Password);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(
                    validation.ErrorCode == "USER_INVALID" ? "使用者編號錯誤" : "使用者密碼錯誤");
            }

            // 修改帳號終止日
            await _service.UpdateEndDateAsync(userId, dto.EndDate);
        }, $"修改帳號終止日失敗: {userId}");
    }

    /// <summary>
    /// 使用者查詢功能 (SYS0910) - POST 方法
    /// </summary>
    [HttpPost("query")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> QueryUsers(
        [FromBody] UserQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUsersAsync(query);
            return result;
        }, "查詢使用者列表失敗");
    }

    /// <summary>
    /// 使用者查詢功能 (SYS0910) - GET 方法
    /// </summary>
    [HttpGet("query")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> QueryUsersGet(
        [FromQuery] UserQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUsersAsync(query);
            return result;
        }, "查詢使用者列表失敗");
    }

    /// <summary>
    /// 查詢單筆使用者 (SYS0910)
    /// </summary>
    [HttpGet("{userId}/query")]
    public async Task<ActionResult<ApiResponse<UserDto>>> QueryUserById(string userId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserAsync(userId);
            return result;
        }, $"查詢使用者失敗: {userId}");
    }

    /// <summary>
    /// 匯出使用者查詢結果 (SYS0910)
    /// </summary>
    [HttpPost("query/export")]
    public async Task<IActionResult> ExportUserQuery(
        [FromBody] UserQueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportUserQueryAsync(query, format);

            var contentType = format.ToLower() == "pdf"
                ? "application/pdf"
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fileName = $"使用者查詢結果_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出使用者查詢結果失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出使用者查詢結果 (SYS0140)
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportUsers(
        [FromBody] UserQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportUserQueryAsync(query, "excel");

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"使用者查詢結果_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出使用者查詢結果失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得當前登入使用者資訊
    /// </summary>
    [HttpGet("current")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetCurrentUserAsync();
            return result;
        }, "取得當前登入使用者資訊失敗");
    }

    /// <summary>
    /// 重設密碼 (SYS0110)
    /// </summary>
    [HttpPost("{userId}/reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword(
        string userId,
        [FromBody] ResetPasswordDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ResetPasswordAsync(userId, dto);
        }, $"重設密碼失敗: {userId}");
    }

    /// <summary>
    /// 啟用/停用使用者 (SYS0110)
    /// </summary>
    [HttpPut("{userId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(
        string userId,
        [FromBody] UpdateStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(userId, dto);
        }, $"更新使用者狀態失敗: {userId}");
    }

    /// <summary>
    /// 查詢單筆使用者（含業種儲位資訊）(SYS0111)
    /// </summary>
    [HttpGet("{userId}/detail")]
    public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetUserDetail(string userId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserDetailAsync(userId);
            return result;
        }, $"查詢使用者詳細資料失敗: {userId}");
    }

    /// <summary>
    /// 查詢業種大分類列表 (SYS0111)
    /// </summary>
    [HttpGet("business-types/major")]
    public async Task<ActionResult<ApiResponse<List<BusinessTypeMajorDto>>>> GetBusinessTypeMajors()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessTypeMajorsAsync();
            return result;
        }, "查詢業種大分類列表失敗");
    }

    /// <summary>
    /// 查詢業種中分類列表 (SYS0111)
    /// </summary>
    [HttpGet("business-types/middle")]
    public async Task<ActionResult<ApiResponse<List<BusinessTypeMiddleDto>>>> GetBusinessTypeMiddles(
        [FromQuery] string btypeMId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessTypeMiddlesAsync(btypeMId);
            return result;
        }, $"查詢業種中分類列表失敗: {btypeMId}");
    }

    /// <summary>
    /// 查詢業種小分類列表 (SYS0111)
    /// </summary>
    [HttpGet("business-types/minor")]
    public async Task<ActionResult<ApiResponse<List<BusinessTypeMinorDto>>>> GetBusinessTypeMinors(
        [FromQuery] string btypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessTypeMinorsAsync(btypeId);
            return result;
        }, $"查詢業種小分類列表失敗: {btypeId}");
    }

    /// <summary>
    /// 查詢儲位列表 (SYS0111)
    /// </summary>
    [HttpGet("warehouse-areas")]
    public async Task<ActionResult<ApiResponse<List<WarehouseAreaDto>>>> GetWarehouseAreas(
        [FromQuery] int? level,
        [FromQuery] string? parentId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetWarehouseAreasAsync(level, parentId);
            return result;
        }, "查詢儲位列表失敗");
    }

    /// <summary>
    /// 查詢7X承租分店列表 (SYS0111)
    /// </summary>
    [HttpGet("stores")]
    public async Task<ActionResult<ApiResponse<List<StoreDto>>>> GetStores()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStoresAsync();
            return result;
        }, "查詢7X承租分店列表失敗");
    }

    /// <summary>
    /// 新增使用者（含業種儲位設定）(SYS0111)
    /// </summary>
    [HttpPost("with-business-types")]
    public async Task<ActionResult<ApiResponse<string>>> CreateUserWithBusinessTypes(
        [FromBody] CreateUserWithBusinessTypesDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateUserWithBusinessTypesAsync(dto);
            return result;
        }, "新增使用者（含業種儲位設定）失敗");
    }

    /// <summary>
    /// 修改使用者（含業種儲位設定）(SYS0111)
    /// </summary>
    [HttpPut("{userId}/with-business-types")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUserWithBusinessTypes(
        string userId,
        [FromBody] UpdateUserWithBusinessTypesDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateUserWithBusinessTypesAsync(userId, dto);
        }, $"修改使用者（含業種儲位設定）失敗: {userId}");
    }
}

