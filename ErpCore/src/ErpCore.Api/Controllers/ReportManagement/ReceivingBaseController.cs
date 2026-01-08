using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Application.Services.ReportManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ReportManagement;

/// <summary>
/// 收款基礎功能控制器 (SYSR110-SYSR120)
/// 提供收款項目對照會計科目維護作業
/// </summary>
[Route("api/v1/receipt")]
public class ReceivingBaseController : BaseController
{
    private readonly IArItemsService _arItemsService;

    public ReceivingBaseController(
        IArItemsService arItemsService,
        ILoggerService logger) : base(logger)
    {
        _arItemsService = arItemsService;
    }

    #region 收款項目維護 (SYSR110-SYSR120)

    /// <summary>
    /// 查詢收款項目列表
    /// </summary>
    [HttpGet("aritems")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ArItemsDto>>>> GetArItems(
        [FromQuery] string? siteId = null,
        [FromQuery] string? aritemId = null)
    {
        return await ExecuteAsync(async () =>
        {
            IEnumerable<ArItemsDto> result;
            if (!string.IsNullOrEmpty(siteId))
            {
                result = await _arItemsService.GetBySiteIdAsync(siteId);
            }
            else
            {
                result = await _arItemsService.GetAllAsync();
            }

            // 如果指定了 aritemId，進行過濾
            if (!string.IsNullOrEmpty(aritemId) && !string.IsNullOrEmpty(siteId))
            {
                var item = await _arItemsService.GetBySiteIdAndAritemIdAsync(siteId, aritemId);
                result = item != null ? new[] { item } : Enumerable.Empty<ArItemsDto>();
            }

            return result;
        }, "查詢收款項目列表失敗");
    }

    /// <summary>
    /// 查詢單筆收款項目
    /// </summary>
    [HttpGet("aritems/{tKey}")]
    public async Task<ActionResult<ApiResponse<ArItemsDto>>> GetArItem(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _arItemsService.GetByIdAsync(tKey);
            return result;
        }, $"查詢收款項目失敗: {tKey}");
    }

    /// <summary>
    /// 查詢收款項目（依分店和項目代號）
    /// </summary>
    [HttpGet("aritems/{siteId}/{aritemId}")]
    public async Task<ActionResult<ApiResponse<ArItemsDto>>> GetArItemBySiteIdAndAritemId(
        string siteId,
        string aritemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _arItemsService.GetBySiteIdAndAritemIdAsync(siteId, aritemId);
            return result;
        }, $"查詢收款項目失敗: SiteId={siteId}, AritemId={aritemId}");
    }

    /// <summary>
    /// 新增收款項目
    /// </summary>
    [HttpPost("aritems")]
    public async Task<ActionResult<ApiResponse<ArItemsDto>>> CreateArItem(
        [FromBody] CreateArItemsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _arItemsService.CreateAsync(dto);
            return result;
        }, "新增收款項目失敗");
    }

    /// <summary>
    /// 修改收款項目
    /// </summary>
    [HttpPut("aritems/{tKey}")]
    public async Task<ActionResult<ApiResponse<ArItemsDto>>> UpdateArItem(
        long tKey,
        [FromBody] UpdateArItemsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _arItemsService.UpdateAsync(tKey, dto);
            return result;
        }, $"修改收款項目失敗: {tKey}");
    }

    /// <summary>
    /// 刪除收款項目
    /// </summary>
    [HttpDelete("aritems/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteArItem(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _arItemsService.DeleteAsync(tKey);
        }, $"刪除收款項目失敗: {tKey}");
    }

    /// <summary>
    /// 檢查收款項目是否存在
    /// </summary>
    [HttpGet("aritems/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckArItemExists(
        [FromQuery] string siteId,
        [FromQuery] string aritemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _arItemsService.ExistsAsync(siteId, aritemId);
            return result;
        }, "檢查收款項目是否存在失敗");
    }

    #endregion
}

