using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.DropdownList;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.DropdownList;

/// <summary>
/// 系統列表控制器 (SYSID_LIST, USER_LIST)
/// </summary>
[Route("api/v1/lists/system")]
public class SystemListController : BaseController
{
    private readonly ISystemListService _systemListService;

    public SystemListController(
        ISystemListService systemListService,
        ILoggerService logger) : base(logger)
    {
        _systemListService = systemListService;
    }

    /// <summary>
    /// 查詢系統列表 (SYSID_LIST)
    /// </summary>
    [HttpGet("systems")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SystemListDto>>>> GetSystemList(
        [FromQuery] SystemListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _systemListService.GetSystemListAsync(query);
            return result;
        }, "查詢系統列表失敗");
    }

    /// <summary>
    /// 查詢系統選項（用於下拉選單）
    /// </summary>
    [HttpGet("systems/options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OptionDto>>>> GetSystemOptions(
        [FromQuery] string? status = null,
        [FromQuery] string? excludeSystems = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _systemListService.GetSystemOptionsAsync(status, excludeSystems);
            return result;
        }, "查詢系統選項失敗");
    }

    /// <summary>
    /// 查詢使用者列表 (USER_LIST)
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetUserList(
        [FromQuery] UserListQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _systemListService.GetUserListAsync(query);
            return result;
        }, "查詢使用者列表失敗");
    }

    /// <summary>
    /// 查詢使用者列表選項（用於下拉選單）
    /// </summary>
    [HttpGet("users/options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OptionDto>>>> GetUserListOptions(
        [FromQuery] string? orgId = null,
        [FromQuery] string? status = "A",
        [FromQuery] string? keyword = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _systemListService.GetUserListOptionsAsync(orgId, status, keyword);
            return result;
        }, "查詢使用者列表選項失敗");
    }
}

