using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreMember;

/// <summary>
/// 會員資料維護控制器 (SYS3000 - 會員資料維護)
/// </summary>
[Route("api/v1/members")]
public class MemberController : BaseController
{
    private readonly IMemberService _service;

    public MemberController(
        IMemberService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢會員列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<MemberDto>>>> GetMembers(
        [FromQuery] MemberQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMembersAsync(query);
            return result;
        }, "查詢會員列表失敗");
    }

    /// <summary>
    /// 查詢單筆會員
    /// </summary>
    [HttpGet("{memberId}")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> GetMember(string memberId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMemberByIdAsync(memberId);
            return result;
        }, $"查詢會員失敗: {memberId}");
    }

    /// <summary>
    /// 新增會員
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateMember(
        [FromBody] CreateMemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateMemberAsync(dto);
            return result;
        }, "新增會員失敗");
    }

    /// <summary>
    /// 修改會員
    /// </summary>
    [HttpPut("{memberId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMember(
        string memberId,
        [FromBody] UpdateMemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateMemberAsync(memberId, dto);
        }, $"修改會員失敗: {memberId}");
    }

    /// <summary>
    /// 刪除會員
    /// </summary>
    [HttpDelete("{memberId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMember(string memberId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteMemberAsync(memberId);
        }, $"刪除會員失敗: {memberId}");
    }

    /// <summary>
    /// 檢查會員編號是否存在
    /// </summary>
    [HttpGet("check/{memberId}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckMemberExists(string memberId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExistsAsync(memberId);
            return result;
        }, $"檢查會員編號是否存在失敗: {memberId}");
    }

    /// <summary>
    /// 更新會員狀態
    /// </summary>
    [HttpPut("{memberId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMemberStatus(
        string memberId,
        [FromBody] UpdateMemberStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(memberId, dto.Status);
        }, $"更新會員狀態失敗: {memberId}");
    }

    /// <summary>
    /// 查詢會員積分記錄
    /// </summary>
    [HttpGet("{memberId}/points")]
    public async Task<ActionResult<ApiResponse<PagedResult<MemberPointDto>>>> GetMemberPoints(
        string memberId,
        [FromQuery] MemberPointQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMemberPointsAsync(memberId, query);
            return result;
        }, $"查詢會員積分記錄失敗: {memberId}");
    }
}

