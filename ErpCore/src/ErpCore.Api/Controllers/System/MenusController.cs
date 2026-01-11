using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 子系統項目資料維護作業控制器 (SYS0420)
/// </summary>
[Route("api/v1/menus")]
public class MenusController : BaseController
{
    private readonly IMenuService _service;

    public MenusController(
        IMenuService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢子系統列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<MenuDto>>>> GetMenus(
        [FromQuery] MenuQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMenusAsync(query);
            return result;
        }, "查詢子系統列表失敗");
    }

    /// <summary>
    /// 查詢單筆子系統
    /// </summary>
    [HttpGet("{menuId}")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetMenu(string menuId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMenuAsync(menuId);
            return result;
        }, $"查詢子系統失敗: {menuId}");
    }

    /// <summary>
    /// 新增子系統
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateMenu(
        [FromBody] CreateMenuDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateMenuAsync(dto);
            return result;
        }, "新增子系統失敗");
    }

    /// <summary>
    /// 修改子系統
    /// </summary>
    [HttpPut("{menuId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMenu(
        string menuId,
        [FromBody] UpdateMenuDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateMenuAsync(menuId, dto);
        }, $"修改子系統失敗: {menuId}");
    }

    /// <summary>
    /// 刪除子系統
    /// </summary>
    [HttpDelete("{menuId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMenu(string menuId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteMenuAsync(menuId);
        }, $"刪除子系統失敗: {menuId}");
    }

    /// <summary>
    /// 批次刪除子系統
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMenusBatch(
        [FromBody] BatchDeleteMenusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteMenusBatchAsync(dto);
        }, "批次刪除子系統失敗");
    }
}
