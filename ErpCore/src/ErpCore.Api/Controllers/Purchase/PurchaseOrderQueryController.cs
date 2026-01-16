using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Purchase;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Purchase;

/// <summary>
/// 採購單查詢控制器 (SYSP310-SYSP330)
/// </summary>
[Route("api/v1/purchase-orders")]
[ApiController]
public class PurchaseOrderQueryController : BaseController
{
    private readonly IPurchaseOrderQueryService _queryService;

    public PurchaseOrderQueryController(
        IPurchaseOrderQueryService queryService,
        ILoggerService logger) : base(logger)
    {
        _queryService = queryService;
    }

    /// <summary>
    /// 查詢採購單列表
    /// </summary>
    [HttpGet("query")]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderQueryResultDto>>>> QueryPurchaseOrders(
        [FromQuery] PurchaseOrderQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryService.QueryPurchaseOrdersAsync(request);
            return result;
        }, "查詢採購單列表失敗");
    }

    /// <summary>
    /// 查詢採購單明細
    /// </summary>
    [HttpGet("{orderId}/details")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderDetailQueryDto>>> GetPurchaseOrderDetails(string orderId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryService.GetPurchaseOrderDetailsAsync(orderId);
            return result;
        }, $"查詢採購單明細失敗: {orderId}");
    }

    /// <summary>
    /// 查詢採購單統計資料
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderStatisticsDto>>> GetPurchaseOrderStatistics(
        [FromQuery] PurchaseOrderStatisticsRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryService.GetPurchaseOrderStatisticsAsync(request);
            return result;
        }, "查詢採購單統計失敗");
    }

    /// <summary>
    /// 匯出採購單查詢結果
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportPurchaseOrders([FromBody] PurchaseOrderExportRequestDto request)
    {
        try
        {
            var fileBytes = await _queryService.ExportPurchaseOrdersAsync(request);
            
            var contentType = request.ExportType.ToLower() switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            var fileName = $"採購單查詢結果_{DateTime.Now:yyyyMMddHHmmss}.{request.ExportType}";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出採購單查詢結果失敗", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = $"匯出失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 列印採購單
    /// </summary>
    [HttpGet("{orderId}/print")]
    public async Task<IActionResult> PrintPurchaseOrder(string orderId)
    {
        try
        {
            var fileBytes = await _queryService.PrintPurchaseOrderAsync(orderId);
            
            return File(fileBytes, "application/pdf", $"採購單_{orderId}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印採購單失敗: {orderId}", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = $"列印失敗: {ex.Message}"
            });
        }
    }
}
