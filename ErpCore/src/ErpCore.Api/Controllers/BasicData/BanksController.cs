using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 銀行基本資料維護控制器 (SYSBC20)
/// </summary>
[Route("api/v1/banks")]
public class BanksController : BaseController
{
    private readonly IBankService _service;

    public BanksController(
        IBankService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢銀行列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BankDto>>>> GetBanks(
        [FromQuery] BankQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBanksAsync(query);
            return result;
        }, "查詢銀行列表失敗");
    }

    /// <summary>
    /// 查詢單筆銀行
    /// </summary>
    [HttpGet("{bankId}")]
    public async Task<ActionResult<ApiResponse<BankDto>>> GetBank(string bankId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBankAsync(bankId);
            return result;
        }, $"查詢銀行失敗: {bankId}");
    }

    /// <summary>
    /// 新增銀行
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateBank(
        [FromBody] CreateBankDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateBankAsync(dto);
            return result;
        }, "新增銀行失敗");
    }

    /// <summary>
    /// 修改銀行
    /// </summary>
    [HttpPut("{bankId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateBank(
        string bankId,
        [FromBody] UpdateBankDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateBankAsync(bankId, dto);
        }, $"修改銀行失敗: {bankId}");
    }

    /// <summary>
    /// 刪除銀行
    /// </summary>
    [HttpDelete("{bankId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBank(string bankId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteBankAsync(bankId);
        }, $"刪除銀行失敗: {bankId}");
    }

    /// <summary>
    /// 批次刪除銀行
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBanksBatch(
        [FromBody] BatchDeleteBankDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteBanksBatchAsync(dto);
        }, "批次刪除銀行失敗");
    }
}
