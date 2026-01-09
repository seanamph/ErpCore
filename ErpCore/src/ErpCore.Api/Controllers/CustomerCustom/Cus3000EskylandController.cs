using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Application.Services.CustomerCustom;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CustomerCustom;

/// <summary>
/// CUS3000.ESKYLAND 客戶定制模組控制器
/// ESKYLAND客戶定制版本，功能類似CUS3000但針對ESKYLAND客戶場景優化
/// </summary>
[Route("api/v1/cus3000-eskyland")]
public class Cus3000EskylandController : BaseController
{
    private readonly ICus3000EskylandMemberService _memberService;

    public Cus3000EskylandController(
        ICus3000EskylandMemberService memberService,
        ILoggerService logger) : base(logger)
    {
        _memberService = memberService;
    }

    #region 會員管理

    /// <summary>
    /// 查詢會員列表
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<ApiResponse<PagedResult<Cus3000EskylandMemberDto>>>> GetCus3000EskylandMemberList(
        [FromQuery] Cus3000EskylandMemberQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetCus3000EskylandMemberListAsync(query);
            return result;
        }, "查詢ESKYLAND會員列表失敗");
    }

    /// <summary>
    /// 查詢單筆會員
    /// </summary>
    [HttpGet("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<Cus3000EskylandMemberDto>>> GetCus3000EskylandMemberById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetCus3000EskylandMemberByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"會員不存在: {tKey}");
            }
            return result;
        }, "查詢ESKYLAND會員失敗");
    }

    /// <summary>
    /// 查詢會員（依會員編號）
    /// </summary>
    [HttpGet("members/memberid/{memberId}")]
    public async Task<ActionResult<ApiResponse<Cus3000EskylandMemberDto>>> GetCus3000EskylandMemberByMemberId(string memberId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetCus3000EskylandMemberByMemberIdAsync(memberId);
            if (result == null)
            {
                throw new InvalidOperationException($"會員不存在: {memberId}");
            }
            return result;
        }, "查詢ESKYLAND會員失敗");
    }

    /// <summary>
    /// 查詢會員（依會員卡號）
    /// </summary>
    [HttpGet("members/cardno/{cardNo}")]
    public async Task<ActionResult<ApiResponse<Cus3000EskylandMemberDto>>> GetCus3000EskylandMemberByCardNo(string cardNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetCus3000EskylandMemberByCardNoAsync(cardNo);
            if (result == null)
            {
                throw new InvalidOperationException($"會員不存在: {cardNo}");
            }
            return result;
        }, "查詢ESKYLAND會員失敗");
    }

    /// <summary>
    /// 新增會員
    /// </summary>
    [HttpPost("members")]
    public async Task<ActionResult<ApiResponse<long>>> CreateCus3000EskylandMember([FromBody] CreateCus3000EskylandMemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _memberService.CreateCus3000EskylandMemberAsync(dto);
            return tKey;
        }, "新增ESKYLAND會員失敗");
    }

    /// <summary>
    /// 修改會員
    /// </summary>
    [HttpPut("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCus3000EskylandMember(long tKey, [FromBody] UpdateCus3000EskylandMemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _memberService.UpdateCus3000EskylandMemberAsync(tKey, dto);
            return true;
        }, "修改ESKYLAND會員失敗");
    }

    /// <summary>
    /// 刪除會員
    /// </summary>
    [HttpDelete("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCus3000EskylandMember(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _memberService.DeleteCus3000EskylandMemberAsync(tKey);
            return true;
        }, "刪除ESKYLAND會員失敗");
    }

    #endregion
}

