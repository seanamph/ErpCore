using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StockAdjustment;
using ErpCore.Application.Services.StockAdjustment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StockAdjustment;

/// <summary>
/// 庫存調整作業控制器 (SYSW490)
/// </summary>
[Route("api/v1/inventory-adjustments")]
public class StockAdjustmentController : BaseController
{
    private readonly IStockAdjustmentService _service;

    public StockAdjustmentController(
        IStockAdjustmentService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢調整單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<InventoryAdjustmentDto>>>> GetInventoryAdjustments(
        [FromQuery] InventoryAdjustmentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInventoryAdjustmentsAsync(query);
            return result;
        }, "查詢調整單列表失敗");
    }

    /// <summary>
    /// 查詢單筆調整單
    /// </summary>
    [HttpGet("{adjustmentId}")]
    public async Task<ActionResult<ApiResponse<InventoryAdjustmentDto>>> GetInventoryAdjustment(string adjustmentId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInventoryAdjustmentByIdAsync(adjustmentId);
            return result;
        }, $"查詢調整單失敗: {adjustmentId}");
    }

    /// <summary>
    /// 新增調整單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateInventoryAdjustment(
        [FromBody] CreateInventoryAdjustmentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var adjustmentId = await _service.CreateInventoryAdjustmentAsync(dto);
            return adjustmentId;
        }, "新增調整單失敗");
    }

    /// <summary>
    /// 修改調整單
    /// </summary>
    [HttpPut("{adjustmentId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateInventoryAdjustment(
        string adjustmentId,
        [FromBody] UpdateInventoryAdjustmentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateInventoryAdjustmentAsync(adjustmentId, dto);
        }, $"修改調整單失敗: {adjustmentId}");
    }

    /// <summary>
    /// 刪除調整單
    /// </summary>
    [HttpDelete("{adjustmentId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteInventoryAdjustment(string adjustmentId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteInventoryAdjustmentAsync(adjustmentId);
        }, $"刪除調整單失敗: {adjustmentId}");
    }

    /// <summary>
    /// 確認調整單
    /// </summary>
    [HttpPost("{adjustmentId}/confirm")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmAdjustment(string adjustmentId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ConfirmAdjustmentAsync(adjustmentId);
        }, $"確認調整單失敗: {adjustmentId}");
    }

    /// <summary>
    /// 取消調整單
    /// </summary>
    [HttpPost("{adjustmentId}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelAdjustment(string adjustmentId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelAdjustmentAsync(adjustmentId);
        }, $"取消調整單失敗: {adjustmentId}");
    }

    /// <summary>
    /// 取得調整原因列表
    /// </summary>
    [HttpGet("reasons")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AdjustmentReasonDto>>>> GetAdjustmentReasons()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAdjustmentReasonsAsync();
            return result;
        }, "查詢調整原因失敗");
    }
}

