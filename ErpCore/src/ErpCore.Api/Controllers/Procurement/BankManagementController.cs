using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 銀行管理控制器
/// </summary>
[ApiController]
[Route("api/v1/bank-accounts")]
public class BankManagementController : BaseController
{
    private readonly IBankAccountService _service;

    public BankManagementController(
        IBankAccountService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢銀行帳戶列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BankAccountDto>>>> GetBankAccounts(
        [FromQuery] BankAccountQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBankAccountsAsync(query);
            return result;
        }, "查詢銀行帳戶列表失敗");
    }

    /// <summary>
    /// 查詢單筆銀行帳戶
    /// </summary>
    [HttpGet("{bankAccountId}")]
    public async Task<ActionResult<ApiResponse<BankAccountDto>>> GetBankAccount(string bankAccountId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBankAccountByIdAsync(bankAccountId);
            return result;
        }, $"查詢銀行帳戶失敗: {bankAccountId}");
    }

    /// <summary>
    /// 新增銀行帳戶
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateBankAccount(
        [FromBody] CreateBankAccountDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var bankAccountId = await _service.CreateBankAccountAsync(dto);
            return bankAccountId;
        }, "新增銀行帳戶失敗");
    }

    /// <summary>
    /// 修改銀行帳戶
    /// </summary>
    [HttpPut("{bankAccountId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateBankAccount(
        string bankAccountId,
        [FromBody] UpdateBankAccountDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateBankAccountAsync(bankAccountId, dto);
        }, $"修改銀行帳戶失敗: {bankAccountId}");
    }

    /// <summary>
    /// 刪除銀行帳戶
    /// </summary>
    [HttpDelete("{bankAccountId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBankAccount(string bankAccountId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteBankAccountAsync(bankAccountId);
        }, $"刪除銀行帳戶失敗: {bankAccountId}");
    }

    /// <summary>
    /// 更新銀行帳戶狀態
    /// </summary>
    [HttpPut("{bankAccountId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(
        string bankAccountId,
        [FromBody] UpdateStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(bankAccountId, dto.Status);
        }, $"更新銀行帳戶狀態失敗: {bankAccountId}");
    }

    /// <summary>
    /// 查詢銀行帳戶餘額
    /// </summary>
    [HttpGet("{bankAccountId}/balance")]
    public async Task<ActionResult<ApiResponse<BankAccountBalanceDto>>> GetBalance(string bankAccountId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBalanceAsync(bankAccountId);
            return result;
        }, $"查詢銀行帳戶餘額失敗: {bankAccountId}");
    }

    /// <summary>
    /// 檢查銀行帳戶是否存在
    /// </summary>
    [HttpGet("{bankAccountId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckBankAccountExists(string bankAccountId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(bankAccountId);
            return exists;
        }, $"檢查銀行帳戶是否存在失敗: {bankAccountId}");
    }
}

