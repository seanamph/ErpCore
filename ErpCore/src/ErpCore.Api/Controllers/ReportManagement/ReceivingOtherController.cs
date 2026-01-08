using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Application.Services.ReportManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ReportManagement;

/// <summary>
/// 收款其他功能控制器 (SYSR510-SYSR570)
/// 提供保證金維護、存款功能、收款查詢與報表等其他收款相關功能
/// </summary>
[Route("api/v1/receipt")]
public class ReceivingOtherController : BaseController
{
    private readonly IReceivingOtherService _service;

    public ReceivingOtherController(
        IReceivingOtherService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region 保證金維護 (SYSR510)

    /// <summary>
    /// 查詢保證金列表
    /// </summary>
    [HttpGet("deposits")]
    public async Task<ActionResult<ApiResponse<PagedResult<DepositsDto>>>> GetDeposits(
        [FromQuery] DepositsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.QueryDepositsAsync(query);
        }, "查詢保證金列表失敗");
    }

    /// <summary>
    /// 查詢單筆保證金
    /// </summary>
    [HttpGet("deposits/{tKey}")]
    public async Task<ActionResult<ApiResponse<DepositsDto>>> GetDeposit(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetDepositByIdAsync(tKey);
        }, $"查詢保證金失敗: {tKey}");
    }

    /// <summary>
    /// 新增保證金
    /// </summary>
    [HttpPost("deposits")]
    public async Task<ActionResult<ApiResponse<DepositsDto>>> CreateDeposit(
        [FromBody] CreateDepositsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.CreateDepositAsync(dto);
        }, "新增保證金失敗");
    }

    /// <summary>
    /// 修改保證金
    /// </summary>
    [HttpPut("deposits/{tKey}")]
    public async Task<ActionResult<ApiResponse<DepositsDto>>> UpdateDeposit(
        long tKey,
        [FromBody] UpdateDepositsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.UpdateDepositAsync(tKey, dto);
        }, $"修改保證金失敗: {tKey}");
    }

    /// <summary>
    /// 刪除保證金
    /// </summary>
    [HttpDelete("deposits/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDeposit(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteDepositAsync(tKey);
            return new { message = "刪除成功" };
        }, $"刪除保證金失敗: {tKey}");
    }

    #endregion

    #region 保證金退還與存款 (SYSR510)

    /// <summary>
    /// 保證金退還
    /// </summary>
    [HttpPost("deposits/{tKey}/return")]
    public async Task<ActionResult<ApiResponse<DepositsDto>>> ReturnDeposit(
        long tKey,
        [FromBody] ReturnDepositsDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.ReturnDepositAsync(tKey, dto);
        }, $"退還保證金失敗: {tKey}");
    }

    /// <summary>
    /// 保證金存款
    /// </summary>
    [HttpPost("deposits/{tKey}/deposit")]
    public async Task<ActionResult<ApiResponse<DepositsDto>>> DepositAmount(
        long tKey,
        [FromBody] DepositAmountDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.DepositAmountAsync(tKey, dto);
        }, $"存款保證金失敗: {tKey}");
    }

    #endregion
}

