using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Query;

/// <summary>
/// 質量管理基礎功能控制器 (SYSQ110-SYSQ120)
/// 提供零用金參數維護與保管人及額度設定作業
/// </summary>
[Route("api/v1/quality")]
public class QualityBaseController : BaseController
{
    private readonly ICashParamsService _cashParamsService;
    private readonly IPcKeepService _pcKeepService;

    public QualityBaseController(
        ICashParamsService cashParamsService,
        IPcKeepService pcKeepService,
        ILoggerService logger) : base(logger)
    {
        _cashParamsService = cashParamsService;
        _pcKeepService = pcKeepService;
    }

    #region 零用金參數 (SYSQ110)

    /// <summary>
    /// 查詢零用金參數列表
    /// </summary>
    [HttpGet("cash-params")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CashParamsDto>>>> GetCashParams()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _cashParamsService.GetAllAsync();
            return result;
        }, "查詢零用金參數列表失敗");
    }

    /// <summary>
    /// 查詢單筆零用金參數
    /// </summary>
    [HttpGet("cash-params/{tKey}")]
    public async Task<ActionResult<ApiResponse<CashParamsDto>>> GetCashParams(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _cashParamsService.GetByIdAsync(tKey);
            return result;
        }, $"查詢零用金參數失敗: {tKey}");
    }

    /// <summary>
    /// 新增零用金參數
    /// </summary>
    [HttpPost("cash-params")]
    public async Task<ActionResult<ApiResponse<CashParamsDto>>> CreateCashParams(
        [FromBody] CreateCashParamsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _cashParamsService.CreateAsync(dto);
            return result;
        }, "新增零用金參數失敗");
    }

    /// <summary>
    /// 修改零用金參數
    /// </summary>
    [HttpPut("cash-params/{tKey}")]
    public async Task<ActionResult<ApiResponse<CashParamsDto>>> UpdateCashParams(
        long tKey,
        [FromBody] UpdateCashParamsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _cashParamsService.UpdateAsync(tKey, dto);
            return result;
        }, $"修改零用金參數失敗: {tKey}");
    }

    /// <summary>
    /// 刪除零用金參數
    /// </summary>
    [HttpDelete("cash-params/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCashParams(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _cashParamsService.DeleteAsync(tKey);
        }, $"刪除零用金參數失敗: {tKey}");
    }

    #endregion

    #region 保管人及額度設定 (SYSQ120)

    /// <summary>
    /// 查詢保管人及額度列表
    /// </summary>
    [HttpGet("pc-keep")]
    public async Task<ActionResult<ApiResponse<PagedResult<PcKeepDto>>>> GetPcKeep(
        [FromQuery] PcKeepQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcKeepService.QueryAsync(query);
            return result;
        }, "查詢保管人及額度列表失敗");
    }

    /// <summary>
    /// 查詢單筆保管人及額度
    /// </summary>
    [HttpGet("pc-keep/{tKey}")]
    public async Task<ActionResult<ApiResponse<PcKeepDto>>> GetPcKeep(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcKeepService.GetByIdAsync(tKey);
            return result;
        }, $"查詢保管人及額度失敗: {tKey}");
    }

    /// <summary>
    /// 新增保管人及額度
    /// </summary>
    [HttpPost("pc-keep")]
    public async Task<ActionResult<ApiResponse<PcKeepDto>>> CreatePcKeep(
        [FromBody] CreatePcKeepDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcKeepService.CreateAsync(dto);
            return result;
        }, "新增保管人及額度失敗");
    }

    /// <summary>
    /// 修改保管人及額度
    /// </summary>
    [HttpPut("pc-keep/{tKey}")]
    public async Task<ActionResult<ApiResponse<PcKeepDto>>> UpdatePcKeep(
        long tKey,
        [FromBody] UpdatePcKeepDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcKeepService.UpdateAsync(tKey, dto);
            return result;
        }, $"修改保管人及額度失敗: {tKey}");
    }

    /// <summary>
    /// 刪除保管人及額度
    /// </summary>
    [HttpDelete("pc-keep/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePcKeep(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _pcKeepService.DeleteAsync(tKey);
        }, $"刪除保管人及額度失敗: {tKey}");
    }

    #endregion
}

