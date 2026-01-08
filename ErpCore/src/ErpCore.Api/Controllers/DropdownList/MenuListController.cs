using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.DropdownList;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.DropdownList;

/// <summary>
/// 選單列表控制器 (MENU_LIST)
/// </summary>
[Route("api/v1/lists/menus")]
public class MenuListController : BaseController
{
    private readonly IMenuService _menuService;

    public MenuListController(
        IMenuService menuService,
        ILoggerService logger) : base(logger)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 查詢選單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<MenuDto>>>> GetMenus(
        [FromQuery] MenuQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _menuService.GetMenusAsync(query);
            return result;
        }, "查詢選單列表失敗");
    }

    /// <summary>
    /// 查詢單筆選單
    /// </summary>
    [HttpGet("{menuId}")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetMenu(string menuId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _menuService.GetMenuAsync(menuId);
            if (result == null)
            {
                throw new Exception($"選單不存在: {menuId}");
            }
            return result;
        }, $"查詢選單失敗: {menuId}");
    }

    /// <summary>
    /// 查詢選單選項（用於下拉選單）
    /// </summary>
    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuOptionDto>>>> GetMenuOptions(
        [FromQuery] string? systemId = null,
        [FromQuery] string? status = "1")
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _menuService.GetMenuOptionsAsync(systemId, status);
            return result;
        }, "查詢選單選項失敗");
    }
}

