using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.AnalysisReport;

/// <summary>
/// 耗材出售單控制器 (SYSA297)
/// </summary>
[Route("api/v1/consumable-sales")]
public class ConsumableSalesController : BaseController
{
    private readonly IConsumableSalesService _service;

    public ConsumableSalesController(
        IConsumableSalesService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢耗材出售單列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ConsumableSalesDto>>>> GetSales(
        [FromQuery] ConsumableSalesQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSalesAsync(query);
            return result;
        }, "查詢耗材出售單列表失敗");
    }

    /// <summary>
    /// 查詢單筆耗材出售單
    /// </summary>
    [HttpGet("{txnNo}")]
    public async Task<ActionResult<ApiResponse<ConsumableSalesDetailDto>>> GetSalesDetail(string txnNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSalesDetailAsync(txnNo);
            return result;
        }, "查詢耗材出售單詳細資料失敗");
    }

    /// <summary>
    /// 新增耗材出售單
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateSales(
        [FromBody] CreateConsumableSalesDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var txnNo = await _service.CreateSalesAsync(dto, userId);
            return txnNo;
        }, "新增耗材出售單失敗");
    }

    /// <summary>
    /// 修改耗材出售單
    /// </summary>
    [HttpPut("{txnNo}")]
    public async Task<ActionResult<ApiResponse>> UpdateSales(
        string txnNo,
        [FromBody] UpdateConsumableSalesDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            await _service.UpdateSalesAsync(txnNo, dto, userId);
            return new { success = true };
        }, "修改耗材出售單失敗");
    }

    /// <summary>
    /// 刪除耗材出售單
    /// </summary>
    [HttpDelete("{txnNo}")]
    public async Task<ActionResult<ApiResponse>> DeleteSales(string txnNo)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            await _service.DeleteSalesAsync(txnNo, userId);
            return new { success = true };
        }, "刪除耗材出售單失敗");
    }

    /// <summary>
    /// 審核耗材出售單
    /// </summary>
    [HttpPost("{txnNo}/approve")]
    public async Task<ActionResult<ApiResponse>> ApproveSales(
        string txnNo,
        [FromBody] ApproveSalesDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            await _service.ApproveSalesAsync(txnNo, dto, userId);
            return new { success = true, txnNo, status = "2" };
        }, "審核耗材出售單失敗");
    }

    /// <summary>
    /// 取消耗材出售單
    /// </summary>
    [HttpPost("{txnNo}/cancel")]
    public async Task<ActionResult<ApiResponse>> CancelSales(string txnNo)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            await _service.CancelSalesAsync(txnNo, userId);
            return new { success = true, txnNo, status = "3" };
        }, "取消耗材出售單失敗");
    }
}
