using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreMember;

/// <summary>
/// 商店查詢作業控制器 (SYS3210-SYS3299 - 商店查詢作業)
/// </summary>
[Route("api/v1/shops/query")]
public class StoreQueryController : BaseController
{
    private readonly IStoreService _storeService;
    private readonly IStoreQueryService _queryService;

    public StoreQueryController(
        IStoreService storeService,
        IStoreQueryService queryService,
        ILoggerService logger) : base(logger)
    {
        _storeService = storeService;
        _queryService = queryService;
    }

    /// <summary>
    /// 查詢商店列表（進階查詢）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PagedResult<ShopQueryDto>>>> QueryShops(
        [FromBody] ShopQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryService.QueryShopsAsync(request);
            return result;
        }, "查詢商店列表失敗");
    }

    /// <summary>
    /// 匯出商店查詢結果
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportShops([FromBody] ShopExportRequestDto request)
    {
        try
        {
            var fileBytes = await _queryService.ExportShopsAsync(request);
            var fileName = $"商店查詢結果_{DateTime.Now:yyyyMMddHHmmss}.{request.ExportType.ToLower()}";
            var contentType = request.ExportType == "Excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商店查詢結果失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出失敗: " + ex.Message));
        }
    }
}

