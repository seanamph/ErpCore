using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Application.Services.CustomerCustom;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CustomerCustom;

/// <summary>
/// CUS3000 客戶定制模組控制器 (SYS3130-SYS3160 - 會員管理, SYS3310-SYS3399 - 促銷活動管理, SYS3510-SYS3580 - 活動管理)
/// </summary>
[Route("api/v1/cus3000")]
public class Cus3000Controller : BaseController
{
    private readonly ICus3000MemberService _memberService;
    private readonly ICus3000PromotionService _promotionService;
    private readonly ICus3000ActivityService _activityService;

    public Cus3000Controller(
        ICus3000MemberService memberService,
        ICus3000PromotionService promotionService,
        ICus3000ActivityService activityService,
        ILoggerService logger) : base(logger)
    {
        _memberService = memberService;
        _promotionService = promotionService;
        _activityService = activityService;
    }

    #region 會員管理 (SYS3130-SYS3160)

    /// <summary>
    /// 查詢會員列表
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<ApiResponse<PagedResult<Cus3000MemberDto>>>> GetCus3000MemberList(
        [FromQuery] Cus3000MemberQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetCus3000MemberListAsync(query);
            return result;
        }, "查詢會員列表失敗");
    }

    /// <summary>
    /// 查詢單筆會員
    /// </summary>
    [HttpGet("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<Cus3000MemberDto>>> GetCus3000MemberById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetCus3000MemberByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"會員不存在: {tKey}");
            }
            return result;
        }, "查詢會員失敗");
    }

    /// <summary>
    /// 查詢會員（依會員編號）
    /// </summary>
    [HttpGet("members/memberid/{memberId}")]
    public async Task<ActionResult<ApiResponse<Cus3000MemberDto>>> GetCus3000MemberByMemberId(string memberId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetCus3000MemberByMemberIdAsync(memberId);
            if (result == null)
            {
                throw new InvalidOperationException($"會員不存在: {memberId}");
            }
            return result;
        }, "查詢會員失敗");
    }

    /// <summary>
    /// 查詢會員（依會員卡號）
    /// </summary>
    [HttpGet("members/cardno/{cardNo}")]
    public async Task<ActionResult<ApiResponse<Cus3000MemberDto>>> GetCus3000MemberByCardNo(string cardNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetCus3000MemberByCardNoAsync(cardNo);
            if (result == null)
            {
                throw new InvalidOperationException($"會員不存在: {cardNo}");
            }
            return result;
        }, "查詢會員失敗");
    }

    /// <summary>
    /// 新增會員
    /// </summary>
    [HttpPost("members")]
    public async Task<ActionResult<ApiResponse<long>>> CreateCus3000Member([FromBody] CreateCus3000MemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _memberService.CreateCus3000MemberAsync(dto);
            return tKey;
        }, "新增會員失敗");
    }

    /// <summary>
    /// 修改會員
    /// </summary>
    [HttpPut("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCus3000Member(long tKey, [FromBody] UpdateCus3000MemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _memberService.UpdateCus3000MemberAsync(tKey, dto);
            return true;
        }, "修改會員失敗");
    }

    /// <summary>
    /// 刪除會員
    /// </summary>
    [HttpDelete("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCus3000Member(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _memberService.DeleteCus3000MemberAsync(tKey);
            return true;
        }, "刪除會員失敗");
    }

    #endregion

    #region 促銷活動管理 (SYS3310-SYS3399)

    /// <summary>
    /// 查詢促銷活動列表
    /// </summary>
    [HttpGet("promotions")]
    public async Task<ActionResult<ApiResponse<PagedResult<Cus3000PromotionDto>>>> GetCus3000PromotionList(
        [FromQuery] Cus3000PromotionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _promotionService.GetCus3000PromotionListAsync(query);
            return result;
        }, "查詢促銷活動列表失敗");
    }

    /// <summary>
    /// 查詢單筆促銷活動
    /// </summary>
    [HttpGet("promotions/{tKey}")]
    public async Task<ActionResult<ApiResponse<Cus3000PromotionDto>>> GetCus3000PromotionById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _promotionService.GetCus3000PromotionByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"促銷活動不存在: {tKey}");
            }
            return result;
        }, "查詢促銷活動失敗");
    }

    /// <summary>
    /// 查詢促銷活動（依促銷活動編號）
    /// </summary>
    [HttpGet("promotions/promotionid/{promotionId}")]
    public async Task<ActionResult<ApiResponse<Cus3000PromotionDto>>> GetCus3000PromotionByPromotionId(string promotionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _promotionService.GetCus3000PromotionByPromotionIdAsync(promotionId);
            if (result == null)
            {
                throw new InvalidOperationException($"促銷活動不存在: {promotionId}");
            }
            return result;
        }, "查詢促銷活動失敗");
    }

    /// <summary>
    /// 新增促銷活動
    /// </summary>
    [HttpPost("promotions")]
    public async Task<ActionResult<ApiResponse<long>>> CreateCus3000Promotion([FromBody] CreateCus3000PromotionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _promotionService.CreateCus3000PromotionAsync(dto);
            return tKey;
        }, "新增促銷活動失敗");
    }

    /// <summary>
    /// 修改促銷活動
    /// </summary>
    [HttpPut("promotions/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCus3000Promotion(long tKey, [FromBody] UpdateCus3000PromotionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _promotionService.UpdateCus3000PromotionAsync(tKey, dto);
            return true;
        }, "修改促銷活動失敗");
    }

    /// <summary>
    /// 刪除促銷活動
    /// </summary>
    [HttpDelete("promotions/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCus3000Promotion(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _promotionService.DeleteCus3000PromotionAsync(tKey);
            return true;
        }, "刪除促銷活動失敗");
    }

    #endregion

    #region 活動管理 (SYS3510-SYS3580)

    /// <summary>
    /// 查詢活動列表
    /// </summary>
    [HttpGet("activities")]
    public async Task<ActionResult<ApiResponse<PagedResult<Cus3000ActivityDto>>>> GetCus3000ActivityList(
        [FromQuery] Cus3000ActivityQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _activityService.GetCus3000ActivityListAsync(query);
            return result;
        }, "查詢活動列表失敗");
    }

    /// <summary>
    /// 查詢單筆活動
    /// </summary>
    [HttpGet("activities/{tKey}")]
    public async Task<ActionResult<ApiResponse<Cus3000ActivityDto>>> GetCus3000ActivityById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _activityService.GetCus3000ActivityByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"活動不存在: {tKey}");
            }
            return result;
        }, "查詢活動失敗");
    }

    /// <summary>
    /// 查詢活動（依活動編號）
    /// </summary>
    [HttpGet("activities/activityid/{activityId}")]
    public async Task<ActionResult<ApiResponse<Cus3000ActivityDto>>> GetCus3000ActivityByActivityId(string activityId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _activityService.GetCus3000ActivityByActivityIdAsync(activityId);
            if (result == null)
            {
                throw new InvalidOperationException($"活動不存在: {activityId}");
            }
            return result;
        }, "查詢活動失敗");
    }

    /// <summary>
    /// 新增活動
    /// </summary>
    [HttpPost("activities")]
    public async Task<ActionResult<ApiResponse<long>>> CreateCus3000Activity([FromBody] CreateCus3000ActivityDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _activityService.CreateCus3000ActivityAsync(dto);
            return tKey;
        }, "新增活動失敗");
    }

    /// <summary>
    /// 修改活動
    /// </summary>
    [HttpPut("activities/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCus3000Activity(long tKey, [FromBody] UpdateCus3000ActivityDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _activityService.UpdateCus3000ActivityAsync(tKey, dto);
            return true;
        }, "修改活動失敗");
    }

    /// <summary>
    /// 刪除活動
    /// </summary>
    [HttpDelete("activities/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCus3000Activity(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _activityService.DeleteCus3000ActivityAsync(tKey);
            return true;
        }, "刪除活動失敗");
    }

    #endregion
}

