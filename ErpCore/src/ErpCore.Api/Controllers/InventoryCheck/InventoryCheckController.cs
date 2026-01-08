using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.InventoryCheck;
using ErpCore.Application.Services.InventoryCheck;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.InventoryCheck;

/// <summary>
/// 盤點維護作業控制器 (SYSW53M)
/// </summary>
[Route("api/v1/stocktaking-plans")]
public class InventoryCheckController : BaseController
{
    private readonly IStocktakingPlanService _service;

    public InventoryCheckController(
        IStocktakingPlanService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢盤點計劃列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<StocktakingPlanDto>>>> GetStocktakingPlans(
        [FromQuery] StocktakingPlanQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStocktakingPlansAsync(query);
            return result;
        }, "查詢盤點計劃列表失敗");
    }

    /// <summary>
    /// 查詢單筆盤點計劃
    /// </summary>
    [HttpGet("{planId}")]
    public async Task<ActionResult<ApiResponse<StocktakingPlanDto>>> GetStocktakingPlan(string planId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStocktakingPlanByIdAsync(planId);
            return result;
        }, $"查詢盤點計劃失敗: {planId}");
    }

    /// <summary>
    /// 新增盤點計劃
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateStocktakingPlan(
        [FromBody] CreateStocktakingPlanDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var planId = await _service.CreateStocktakingPlanAsync(dto);
            return planId;
        }, "新增盤點計劃失敗");
    }

    /// <summary>
    /// 修改盤點計劃
    /// </summary>
    [HttpPut("{planId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStocktakingPlan(
        string planId,
        [FromBody] UpdateStocktakingPlanDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStocktakingPlanAsync(planId, dto);
            return new object();
        }, $"修改盤點計劃失敗: {planId}");
    }

    /// <summary>
    /// 刪除盤點計劃
    /// </summary>
    [HttpDelete("{planId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStocktakingPlan(string planId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteStocktakingPlanAsync(planId);
            return new object();
        }, $"刪除盤點計劃失敗: {planId}");
    }

    /// <summary>
    /// 審核盤點計劃
    /// </summary>
    [HttpPost("{planId}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveStocktakingPlan(string planId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApproveStocktakingPlanAsync(planId);
            return new object();
        }, $"審核盤點計劃失敗: {planId}");
    }

    /// <summary>
    /// 上傳盤點資料
    /// </summary>
    [HttpPost("{planId}/upload")]
    public async Task<ActionResult<ApiResponse<object>>> UploadStocktakingData(
        string planId,
        [FromForm] IFormFile? file,
        [FromForm] List<StocktakingTempDto>? data)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UploadStocktakingDataAsync(planId, file, data);
            return new object();
        }, $"上傳盤點資料失敗: {planId}");
    }

    /// <summary>
    /// 計算盤點差異
    /// </summary>
    [HttpPost("{planId}/calculate")]
    public async Task<ActionResult<ApiResponse<object>>> CalculateStocktakingDiff(string planId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CalculateStocktakingDiffAsync(planId);
            return new object();
        }, $"計算盤點差異失敗: {planId}");
    }

    /// <summary>
    /// 確認盤點結果
    /// </summary>
    [HttpPost("{planId}/confirm")]
    public async Task<ActionResult<ApiResponse<string>>> ConfirmStocktakingResult(string planId)
    {
        return await ExecuteAsync(async () =>
        {
            var adjustmentId = await _service.ConfirmStocktakingResultAsync(planId);
            return adjustmentId;
        }, $"確認盤點結果失敗: {planId}");
    }

    /// <summary>
    /// 查詢盤點報表
    /// </summary>
    [HttpGet("{planId}/report")]
    public async Task<ActionResult<ApiResponse<StocktakingReportDto>>> GetStocktakingReport(
        string planId,
        [FromQuery] StocktakingReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStocktakingReportAsync(planId, query);
            return result;
        }, $"查詢盤點報表失敗: {planId}");
    }
}

