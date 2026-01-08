using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.DropdownList;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.DropdownList;

/// <summary>
/// 多選列表控制器 (MULTI_AREA_LIST, MULTI_SHOP_LIST, MULTI_USERS_LIST)
/// </summary>
[Route("api/v1/lists/multi-select")]
public class MultiSelectListController : BaseController
{
    private readonly IMultiSelectListService _multiSelectListService;

    public MultiSelectListController(
        IMultiSelectListService multiSelectListService,
        ILoggerService logger) : base(logger)
    {
        _multiSelectListService = multiSelectListService;
    }

    /// <summary>
    /// 查詢多選區域列表 (MULTI_AREA_LIST)
    /// </summary>
    [HttpGet("areas")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MultiAreaDto>>>> GetMultiAreas(
        [FromQuery] MultiAreaQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _multiSelectListService.GetMultiAreasAsync(query);
            return result;
        }, "查詢多選區域列表失敗");
    }

    /// <summary>
    /// 查詢區域選項（用於下拉選單）
    /// </summary>
    [HttpGet("areas/options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OptionDto>>>> GetAreaOptions(
        [FromQuery] string? status = "A")
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _multiSelectListService.GetAreaOptionsAsync(status);
            return result;
        }, "查詢區域選項失敗");
    }

    /// <summary>
    /// 查詢多選店別列表 (MULTI_SHOP_LIST)
    /// </summary>
    [HttpGet("shops")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MultiShopDto>>>> GetMultiShops(
        [FromQuery] MultiShopQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _multiSelectListService.GetMultiShopsAsync(query);
            return result;
        }, "查詢多選店別列表失敗");
    }

    /// <summary>
    /// 查詢店別選項（用於下拉選單）
    /// </summary>
    [HttpGet("shops/options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OptionDto>>>> GetShopOptions(
        [FromQuery] MultiShopQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _multiSelectListService.GetShopOptionsAsync(query);
            return result;
        }, "查詢店別選項失敗");
    }

    /// <summary>
    /// 查詢多選使用者列表 (MULTI_USERS_LIST)
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<PagedResult<MultiUserDto>>>> GetMultiUsers(
        [FromQuery] MultiUserQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _multiSelectListService.GetMultiUsersAsync(query);
            return result;
        }, "查詢多選使用者列表失敗");
    }

    /// <summary>
    /// 查詢使用者選項（用於下拉選單）
    /// </summary>
    [HttpGet("users/options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OptionDto>>>> GetUserOptions(
        [FromQuery] string? orgId = null,
        [FromQuery] string? status = "A",
        [FromQuery] string? keyword = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _multiSelectListService.GetUserOptionsAsync(orgId, status, keyword);
            return result;
        }, "查詢使用者選項失敗");
    }
}

