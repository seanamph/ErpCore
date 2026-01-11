using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.DropdownList;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.DropdownList;

/// <summary>
/// 使用者列表控制器 (USER_LIST)
/// </summary>
[ApiController]
[Route("api/v1/lists/users")]
public class UserListController : BaseController
{
    private readonly IUserListService _userListService;

    public UserListController(
        IUserListService userListService,
        ILoggerService logger) : base(logger)
    {
        _userListService = userListService;
    }

    /// <summary>
    /// 查詢使用者列表 (USER_LIST_USER_LIST)
    /// </summary>
    [HttpGet("user-list")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetUserList(
        [FromQuery] UserListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _userListService.GetUserListAsync(query);
            return result;
        }, "查詢使用者列表失敗");
    }

    /// <summary>
    /// 查詢部門使用者列表 (USER_LIST_DEPT_LIST)
    /// </summary>
    [HttpGet("dept-list")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetDeptUserList(
        [FromQuery] DeptUserListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _userListService.GetDeptUserListAsync(query);
            return result;
        }, "查詢部門使用者列表失敗");
    }

    /// <summary>
    /// 查詢其他使用者列表 (USER_LIST_OTHER_LIST)
    /// </summary>
    [HttpGet("other-list")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetOtherUserList(
        [FromQuery] UserListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _userListService.GetOtherUserListAsync(query);
            return result;
        }, "查詢其他使用者列表失敗");
    }
}
