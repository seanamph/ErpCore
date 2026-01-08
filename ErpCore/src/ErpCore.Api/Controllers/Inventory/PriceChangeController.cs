using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Inventory;

/// <summary>
/// 商品永久變價作業控制器 (SYSW150)
/// </summary>
[Route("api/v1/price-changes")]
public class PriceChangeController : BaseController
{
    private readonly IPriceChangeService _service;

    public PriceChangeController(
        IPriceChangeService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢變價單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PriceChangeDto>>>> GetPriceChanges(
        [FromQuery] PriceChangeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPriceChangesAsync(query);
            return result;
        }, "查詢變價單列表失敗");
    }

    /// <summary>
    /// 查詢單筆變價單（含明細）
    /// </summary>
    [HttpGet("{priceChangeId}/{priceChangeType}")]
    public async Task<ActionResult<ApiResponse<PriceChangeDetailDto>>> GetPriceChange(
        string priceChangeId, 
        string priceChangeType)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPriceChangeByIdAsync(priceChangeId, priceChangeType);
            return result;
        }, $"查詢變價單失敗: {priceChangeId}/{priceChangeType}");
    }

    /// <summary>
    /// 新增變價單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreatePriceChange(
        [FromBody] CreatePriceChangeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var priceChangeId = await _service.CreatePriceChangeAsync(dto);
            return new { PriceChangeId = priceChangeId };
        }, "新增變價單失敗");
    }

    /// <summary>
    /// 修改變價單
    /// </summary>
    [HttpPut("{priceChangeId}/{priceChangeType}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePriceChange(
        string priceChangeId,
        string priceChangeType,
        [FromBody] UpdatePriceChangeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePriceChangeAsync(priceChangeId, priceChangeType, dto);
        }, $"修改變價單失敗: {priceChangeId}/{priceChangeType}");
    }

    /// <summary>
    /// 刪除變價單
    /// </summary>
    [HttpDelete("{priceChangeId}/{priceChangeType}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePriceChange(
        string priceChangeId,
        string priceChangeType)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePriceChangeAsync(priceChangeId, priceChangeType);
        }, $"刪除變價單失敗: {priceChangeId}/{priceChangeType}");
    }

    /// <summary>
    /// 審核變價單
    /// </summary>
    [HttpPut("{priceChangeId}/{priceChangeType}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApprovePriceChange(
        string priceChangeId,
        string priceChangeType,
        [FromBody] ApprovePriceChangeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApprovePriceChangeAsync(priceChangeId, priceChangeType, dto);
        }, $"審核變價單失敗: {priceChangeId}/{priceChangeType}");
    }

    /// <summary>
    /// 確認變價單
    /// </summary>
    [HttpPut("{priceChangeId}/{priceChangeType}/confirm")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmPriceChange(
        string priceChangeId,
        string priceChangeType,
        [FromBody] ConfirmPriceChangeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ConfirmPriceChangeAsync(priceChangeId, priceChangeType, dto);
        }, $"確認變價單失敗: {priceChangeId}/{priceChangeType}");
    }

    /// <summary>
    /// 作廢變價單
    /// </summary>
    [HttpPut("{priceChangeId}/{priceChangeType}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelPriceChange(
        string priceChangeId,
        string priceChangeType)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.CancelPriceChangeAsync(priceChangeId, priceChangeType);
        }, $"作廢變價單失敗: {priceChangeId}/{priceChangeType}");
    }
}

