using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 項目對應控制器 (SYS0360)
/// </summary>
[Route("api/v1/item-corresponds")]
public class ItemCorrespondsController : BaseController
{
    private readonly IItemCorrespondService _service;

    public ItemCorrespondsController(
        IItemCorrespondService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢項目對應列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ItemCorrespondDto>>>> GetItemCorresponds(
        [FromQuery] ItemCorrespondQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetItemCorrespondsAsync(query);
            return result;
        }, "查詢項目對應列表失敗");
    }

    /// <summary>
    /// 查詢單筆項目對應
    /// </summary>
    [HttpGet("{itemId}")]
    public async Task<ActionResult<ApiResponse<ItemCorrespondDto>>> GetItemCorrespond(string itemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetItemCorrespondByIdAsync(itemId);
            if (result == null)
            {
                throw new InvalidOperationException($"項目對應不存在: {itemId}");
            }
            return result;
        }, $"查詢項目對應失敗: {itemId}");
    }

    /// <summary>
    /// 新增項目對應
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ItemCorrespondDto>>> CreateItemCorrespond(
        [FromBody] CreateItemCorrespondDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateItemCorrespondAsync(dto);
            return result;
        }, "新增項目對應失敗");
    }

    /// <summary>
    /// 修改項目對應
    /// </summary>
    [HttpPut("{itemId}")]
    public async Task<ActionResult<ApiResponse<ItemCorrespondDto>>> UpdateItemCorrespond(
        string itemId,
        [FromBody] UpdateItemCorrespondDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UpdateItemCorrespondAsync(itemId, dto);
            return result;
        }, $"修改項目對應失敗: {itemId}");
    }

    /// <summary>
    /// 刪除項目對應
    /// </summary>
    [HttpDelete("{itemId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteItemCorrespond(string itemId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteItemCorrespondAsync(itemId);
        }, $"刪除項目對應失敗: {itemId}");
    }
}

