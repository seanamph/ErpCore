using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 項目權限控制器 (SYS0360)
/// </summary>
[Route("api/v1/item-corresponds/{itemId}/permissions")]
public class ItemPermissionsController : BaseController
{
    private readonly IItemPermissionService _service;

    public ItemPermissionsController(
        IItemPermissionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢項目權限列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ItemPermissionDto>>>> GetItemPermissions(
        string itemId,
        [FromQuery] ItemPermissionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetItemPermissionsAsync(itemId, query);
            return result;
        }, $"查詢項目權限列表失敗: {itemId}");
    }

    /// <summary>
    /// 查詢項目系統列表
    /// </summary>
    [HttpGet("systems")]
    public async Task<ActionResult<ApiResponse<List<ItemSystemDto>>>> GetSystemList(string itemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemListAsync(itemId);
            return result;
        }, $"查詢項目系統列表失敗: {itemId}");
    }

    /// <summary>
    /// 查詢項目選單列表
    /// </summary>
    [HttpGet("systems/{systemId}/menus")]
    public async Task<ActionResult<ApiResponse<List<ItemMenuDto>>>> GetMenuList(
        string itemId,
        string systemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMenuListAsync(itemId, systemId);
            return result;
        }, $"查詢項目選單列表失敗: {itemId} - {systemId}");
    }

    /// <summary>
    /// 查詢項目作業列表
    /// </summary>
    [HttpGet("menus/{menuId}/programs")]
    public async Task<ActionResult<ApiResponse<List<ItemProgramDto>>>> GetProgramList(
        string itemId,
        string menuId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProgramListAsync(itemId, menuId);
            return result;
        }, $"查詢項目作業列表失敗: {itemId} - {menuId}");
    }

    /// <summary>
    /// 查詢項目按鈕列表
    /// </summary>
    [HttpGet("programs/{programId}/buttons")]
    public async Task<ActionResult<ApiResponse<List<ItemButtonDto>>>> GetButtonList(
        string itemId,
        string programId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetButtonListAsync(itemId, programId);
            return result;
        }, $"查詢項目按鈕列表失敗: {itemId} - {programId}");
    }

    /// <summary>
    /// 設定項目系統權限
    /// </summary>
    [HttpPost("systems/permissions")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> SetSystemPermissions(
        string itemId,
        [FromBody] SetItemSystemPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SetSystemPermissionsAsync(itemId, dto);
            return result;
        }, $"設定項目系統權限失敗: {itemId}");
    }

    /// <summary>
    /// 設定項目選單權限
    /// </summary>
    [HttpPost("menus/permissions")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> SetMenuPermissions(
        string itemId,
        [FromBody] SetItemMenuPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SetMenuPermissionsAsync(itemId, dto);
            return result;
        }, $"設定項目選單權限失敗: {itemId}");
    }

    /// <summary>
    /// 設定項目作業權限
    /// </summary>
    [HttpPost("programs/permissions")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> SetProgramPermissions(
        string itemId,
        [FromBody] SetItemProgramPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SetProgramPermissionsAsync(itemId, dto);
            return result;
        }, $"設定項目作業權限失敗: {itemId}");
    }

    /// <summary>
    /// 設定項目按鈕權限
    /// </summary>
    [HttpPost("buttons/permissions")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> SetButtonPermissions(
        string itemId,
        [FromBody] SetItemButtonPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SetButtonPermissionsAsync(itemId, dto);
            return result;
        }, $"設定項目按鈕權限失敗: {itemId}");
    }

    /// <summary>
    /// 刪除項目權限
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteItemPermission(
        string itemId,
        long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteItemPermissionAsync(itemId, tKey);
        }, $"刪除項目權限失敗: {itemId} - {tKey}");
    }

    /// <summary>
    /// 批量刪除項目權限
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchDeleteItemPermissions(
        string itemId,
        [FromBody] List<long> tKeys)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchDeleteItemPermissionsAsync(itemId, tKeys);
            return result;
        }, $"批量刪除項目權限失敗: {itemId}");
    }
}

