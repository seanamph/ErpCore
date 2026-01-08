using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Pos;
using ErpCore.Application.Services.Pos;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Pos;

/// <summary>
/// POS報表查詢控制器
/// </summary>
[Route("api/v1/pos/reports")]
public class PosReportController : BaseController
{
    private readonly IPosTransactionService _service;

    public PosReportController(
        IPosTransactionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢POS交易報表
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<PosTransactionDto>>>> GetTransactionReport(
        [FromQuery] PosTransactionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPosTransactionsAsync(query);
            return result;
        }, "查詢POS交易報表失敗");
    }
}

