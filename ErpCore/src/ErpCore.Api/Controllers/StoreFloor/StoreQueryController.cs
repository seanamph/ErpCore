using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreFloor;

/// <summary>
/// 商店查詢控制器 (SYS6210-SYS6270 - 商店查詢作業)
/// </summary>
[Route("api/v1/shop-floors")]
public class StoreQueryController : BaseController
{
    private readonly IShopFloorQueryService _service;

    public StoreQueryController(
        IShopFloorQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商店列表
    /// </summary>
    [HttpPost("query")]
    public async Task<ActionResult<ApiResponse<PagedResult<ShopFloorDto>>>> QueryShopFloors(
        [FromBody] StoreQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryShopFloorsAsync(query);
            return result;
        }, "查詢商店列表失敗");
    }

    /// <summary>
    /// 匯出商店查詢結果
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportShopFloors([FromBody] StoreExportDto dto)
    {
        try
        {
            var fileBytes = await _service.ExportShopFloorsAsync(dto);
            var contentType = dto.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase) 
                ? "application/pdf" 
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"商店查詢結果_{DateTime.Now:yyyyMMddHHmmss}.{(dto.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase) ? "pdf" : "xlsx")}";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商店查詢結果失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出失敗: " + ex.Message, "EXPORT_ERROR"));
        }
    }
}

