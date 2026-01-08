using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Accounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Accounting;

/// <summary>
/// 財務交易控制器 (SYSN210)
/// 提供財務交易資料的新增、修改、刪除、查詢功能
/// </summary>
[Route("api/v1/accounting/financial-transactions")]
public class FinancialTransactionController : BaseController
{
    private readonly IFinancialTransactionService _service;

    public FinancialTransactionController(
        IFinancialTransactionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢財務交易列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<FinancialTransactionDto>>>> GetFinancialTransactions(
        [FromQuery] FinancialTransactionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFinancialTransactionsAsync(query);
            return result;
        }, "查詢財務交易列表失敗");
    }

    /// <summary>
    /// 根據主鍵查詢財務交易
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<FinancialTransactionDto>>> GetFinancialTransaction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFinancialTransactionByIdAsync(tKey);
            return result;
        }, "查詢財務交易失敗");
    }

    /// <summary>
    /// 根據交易單號查詢財務交易
    /// </summary>
    [HttpGet("by-txn-no/{txnNo}")]
    public async Task<ActionResult<ApiResponse<FinancialTransactionDto>>> GetFinancialTransactionByTxnNo(string txnNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFinancialTransactionByTxnNoAsync(txnNo);
            return result;
        }, "查詢財務交易失敗");
    }

    /// <summary>
    /// 新增財務交易
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateFinancialTransaction(
        [FromBody] CreateFinancialTransactionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateFinancialTransactionAsync(dto);
            return result;
        }, "新增財務交易失敗");
    }

    /// <summary>
    /// 修改財務交易
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateFinancialTransaction(
        long tKey,
        [FromBody] UpdateFinancialTransactionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateFinancialTransactionAsync(tKey, dto);
        }, "修改財務交易失敗");
    }

    /// <summary>
    /// 刪除財務交易
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFinancialTransaction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteFinancialTransactionAsync(tKey);
        }, "刪除財務交易失敗");
    }

    /// <summary>
    /// 確認財務交易
    /// </summary>
    [HttpPost("{tKey}/confirm")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmFinancialTransaction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ConfirmFinancialTransactionAsync(tKey);
        }, "確認財務交易失敗");
    }

    /// <summary>
    /// 過帳財務交易
    /// </summary>
    [HttpPost("{tKey}/post")]
    public async Task<ActionResult<ApiResponse<object>>> PostFinancialTransaction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.PostFinancialTransactionAsync(tKey);
        }, "過帳財務交易失敗");
    }

    /// <summary>
    /// 檢查借貸平衡
    /// </summary>
    [HttpPost("check-balance")]
    public async Task<ActionResult<ApiResponse<BalanceCheckDto>>> CheckBalance(
        [FromBody] List<CreateFinancialTransactionDto> transactions)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CheckBalanceAsync(transactions);
            return result;
        }, "檢查借貸平衡失敗");
    }
}

