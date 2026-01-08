using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 使用者權限代理控制器 (SYS0117)
/// </summary>
[Route("api/v1/user-agents")]
public class UserAgentsController : BaseController
{
    private readonly IUserAgentService _service;

    public UserAgentsController(
        IUserAgentService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢代理列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserAgentDto>>>> GetUserAgents(
        [FromQuery] UserAgentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserAgentsAsync(query);
            return result;
        }, "查詢代理列表失敗");
    }

    /// <summary>
    /// 查詢單筆代理記錄
    /// </summary>
    [HttpGet("{agentId}")]
    public async Task<ActionResult<ApiResponse<UserAgentDto>>> GetUserAgent(Guid agentId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserAgentAsync(agentId);
            return result;
        }, $"查詢代理記錄失敗: {agentId}");
    }

    /// <summary>
    /// 查詢委託人的代理記錄
    /// </summary>
    [HttpGet("users/{userId}/agent-as-principal")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserAgentDto>>>> GetUserAgentsByPrincipal(
        string userId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserAgentsByPrincipalAsync(userId, pageIndex, pageSize);
            return result;
        }, $"查詢委託人代理記錄失敗: {userId}");
    }

    /// <summary>
    /// 查詢代理人的代理記錄
    /// </summary>
    [HttpGet("users/{userId}/agent-as-agent")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserAgentDto>>>> GetUserAgentsByAgent(
        string userId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserAgentsByAgentAsync(userId, pageIndex, pageSize);
            return result;
        }, $"查詢代理人代理記錄失敗: {userId}");
    }

    /// <summary>
    /// 查詢有效代理記錄
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserAgentDto>>>> GetActiveUserAgents(
        [FromQuery] string? principalUserId = null,
        [FromQuery] string? agentUserId = null,
        [FromQuery] DateTime? currentTime = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetActiveUserAgentsAsync(principalUserId, agentUserId, currentTime);
            return result;
        }, "查詢有效代理記錄失敗");
    }

    /// <summary>
    /// 新增代理記錄
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateUserAgent(
        [FromBody] CreateUserAgentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateUserAgentAsync(dto);
            return result;
        }, "新增代理記錄失敗");
    }

    /// <summary>
    /// 修改代理記錄
    /// </summary>
    [HttpPut("{agentId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUserAgent(
        Guid agentId,
        [FromBody] UpdateUserAgentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateUserAgentAsync(agentId, dto);
        }, $"修改代理記錄失敗: {agentId}");
    }

    /// <summary>
    /// 刪除代理記錄
    /// </summary>
    [HttpDelete("{agentId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUserAgent(Guid agentId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteUserAgentAsync(agentId);
        }, $"刪除代理記錄失敗: {agentId}");
    }

    /// <summary>
    /// 批次刪除代理記錄
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUserAgentsBatch(
        [FromBody] BatchDeleteUserAgentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteUserAgentsBatchAsync(dto);
        }, "批次刪除代理記錄失敗");
    }

    /// <summary>
    /// 更新代理記錄狀態
    /// </summary>
    [HttpPut("{agentId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUserAgentStatus(
        Guid agentId,
        [FromBody] UpdateUserAgentStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateUserAgentStatusAsync(agentId, dto);
        }, $"更新代理記錄狀態失敗: {agentId}");
    }

    /// <summary>
    /// 檢查代理權限
    /// </summary>
    [HttpPost("check-permission")]
    public async Task<ActionResult<ApiResponse<CheckAgentPermissionResultDto>>> CheckAgentPermission(
        [FromBody] CheckAgentPermissionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CheckAgentPermissionAsync(dto);
            return result;
        }, "檢查代理權限失敗");
    }
}

