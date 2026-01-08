using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Pos;
using ErpCore.Application.Services.Pos;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Pos;

/// <summary>
/// POS交易查詢控制器
/// </summary>
[Route("api/v1/pos/transactions")]
public class PosTransactionController : BaseController
{
    private readonly IPosTransactionService _service;

    public PosTransactionController(
        IPosTransactionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢POS交易列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PosTransactionDto>>>> GetPosTransactions(
        [FromQuery] PosTransactionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPosTransactionsAsync(query);
            return result;
        }, "查詢POS交易列表失敗");
    }

    /// <summary>
    /// 根據交易編號查詢POS交易
    /// </summary>
    [HttpGet("{transactionId}")]
    public async Task<ActionResult<ApiResponse<PosTransactionDto>>> GetPosTransaction(string transactionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPosTransactionByIdAsync(transactionId);
            if (result == null)
            {
                throw new Exception($"找不到POS交易: {transactionId}");
            }
            return result;
        }, $"查詢POS交易失敗: {transactionId}");
    }
}

