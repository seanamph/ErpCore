using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CustomerInvoice;

/// <summary>
/// 總帳資料維護控制器 (SYS2000 - 總帳資料維護)
/// </summary>
[Route("api/v1/ledger-data")]
public class LedgerDataController : BaseController
{
    private readonly ILedgerDataService _service;

    public LedgerDataController(
        ILedgerDataService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢總帳列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<GeneralLedgerDto>>>> GetGeneralLedgers(
        [FromQuery] GeneralLedgerQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetGeneralLedgersAsync(query);
            return result;
        }, "查詢總帳列表失敗");
    }

    /// <summary>
    /// 查詢單筆總帳
    /// </summary>
    [HttpGet("{ledgerId}")]
    public async Task<ActionResult<ApiResponse<GeneralLedgerDto>>> GetGeneralLedger(string ledgerId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetGeneralLedgerByIdAsync(ledgerId);
            return result;
        }, $"查詢總帳失敗: {ledgerId}");
    }

    /// <summary>
    /// 新增總帳
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateGeneralLedger(
        [FromBody] CreateGeneralLedgerDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateGeneralLedgerAsync(dto);
            return result;
        }, "新增總帳失敗");
    }

    /// <summary>
    /// 修改總帳
    /// </summary>
    [HttpPut("{ledgerId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateGeneralLedger(
        string ledgerId,
        [FromBody] UpdateGeneralLedgerDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateGeneralLedgerAsync(ledgerId, dto);
        }, $"修改總帳失敗: {ledgerId}");
    }

    /// <summary>
    /// 刪除總帳
    /// </summary>
    [HttpDelete("{ledgerId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteGeneralLedger(string ledgerId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteGeneralLedgerAsync(ledgerId);
        }, $"刪除總帳失敗: {ledgerId}");
    }

    /// <summary>
    /// 過帳
    /// </summary>
    [HttpPost("{ledgerId}/post")]
    public async Task<ActionResult<ApiResponse<object>>> PostLedger(string ledgerId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.PostLedgerAsync(ledgerId);
        }, $"過帳失敗: {ledgerId}");
    }

    /// <summary>
    /// 查詢科目餘額
    /// </summary>
    [HttpGet("account-balances")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AccountBalanceDto>>>> GetAccountBalances(
        [FromQuery] AccountBalanceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAccountBalancesAsync(query);
            return result;
        }, "查詢科目餘額失敗");
    }
}

