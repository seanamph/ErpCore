using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 組別資料維護控制器 (SYSWB70)
/// </summary>
[Route("api/v1/groups")]
public class GroupsController : BaseController
{
    private readonly IGroupService _service;

    public GroupsController(
        IGroupService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢組別列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<GroupDto>>>> GetGroups(
        [FromQuery] GroupQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetGroupsAsync(query);
            return result;
        }, "查詢組別列表失敗");
    }

    /// <summary>
    /// 查詢單筆組別
    /// </summary>
    [HttpGet("{groupId}")]
    public async Task<ActionResult<ApiResponse<GroupDto>>> GetGroup(string groupId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetGroupByIdAsync(groupId);
            return result;
        }, $"查詢組別失敗: {groupId}");
    }

    /// <summary>
    /// 新增組別
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateGroup(
        [FromBody] CreateGroupDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateGroupAsync(dto);
            return result;
        }, "新增組別失敗");
    }

    /// <summary>
    /// 修改組別
    /// </summary>
    [HttpPut("{groupId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateGroup(
        string groupId,
        [FromBody] UpdateGroupDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateGroupAsync(groupId, dto);
        }, $"修改組別失敗: {groupId}");
    }

    /// <summary>
    /// 刪除組別
    /// </summary>
    [HttpDelete("{groupId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteGroup(string groupId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteGroupAsync(groupId);
        }, $"刪除組別失敗: {groupId}");
    }

    /// <summary>
    /// 批次刪除組別
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteGroupsBatch(
        [FromBody] BatchDeleteGroupDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteGroupsBatchAsync(dto);
        }, "批次刪除組別失敗");
    }

    /// <summary>
    /// 更新組別狀態
    /// </summary>
    [HttpPut("{groupId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateGroupStatus(
        string groupId,
        [FromBody] UpdateGroupStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(groupId, dto);
        }, $"更新組別狀態失敗: {groupId}");
    }
}

