using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreMember;

/// <summary>
/// 促銷活動維護控制器 (SYS3510-SYS3600 - 促銷活動維護)
/// </summary>
[Route("api/v1/promotions")]
public class PromotionController : BaseController
{
    private readonly IPromotionService _service;

    public PromotionController(
        IPromotionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢促銷活動列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PromotionDto>>>> GetPromotions(
        [FromQuery] PromotionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPromotionsAsync(query);
            return result;
        }, "查詢促銷活動列表失敗");
    }

    /// <summary>
    /// 查詢單筆促銷活動
    /// </summary>
    [HttpGet("{promotionId}")]
    public async Task<ActionResult<ApiResponse<PromotionDto>>> GetPromotion(string promotionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPromotionByIdAsync(promotionId);
            return result;
        }, $"查詢促銷活動失敗: {promotionId}");
    }

    /// <summary>
    /// 新增促銷活動
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePromotion(
        [FromBody] CreatePromotionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreatePromotionAsync(dto);
            return result;
        }, "新增促銷活動失敗");
    }

    /// <summary>
    /// 修改促銷活動
    /// </summary>
    [HttpPut("{promotionId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePromotion(
        string promotionId,
        [FromBody] UpdatePromotionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePromotionAsync(promotionId, dto);
        }, $"修改促銷活動失敗: {promotionId}");
    }

    /// <summary>
    /// 刪除促銷活動
    /// </summary>
    [HttpDelete("{promotionId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePromotion(string promotionId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePromotionAsync(promotionId);
        }, $"刪除促銷活動失敗: {promotionId}");
    }

    /// <summary>
    /// 更新促銷活動狀態
    /// </summary>
    [HttpPut("{promotionId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePromotionStatus(
        string promotionId,
        [FromBody] UpdatePromotionStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(promotionId, dto.Status);
        }, $"更新促銷活動狀態失敗: {promotionId}");
    }

    /// <summary>
    /// 查詢促銷活動使用統計
    /// </summary>
    [HttpGet("{promotionId}/statistics")]
    public async Task<ActionResult<ApiResponse<PromotionStatisticsDto>>> GetPromotionStatistics(string promotionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStatisticsAsync(promotionId);
            return result;
        }, $"查詢促銷活動統計失敗: {promotionId}");
    }
}

