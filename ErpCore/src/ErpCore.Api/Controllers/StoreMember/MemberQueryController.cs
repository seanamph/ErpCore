using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreMember;

/// <summary>
/// 會員查詢作業控制器 (SYS3330-SYS33B0 - 會員查詢作業)
/// </summary>
[Route("api/v1/members/query")]
public class MemberQueryController : BaseController
{
    private readonly IMemberQueryService _service;

    public MemberQueryController(
        IMemberQueryService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢會員列表（進階查詢）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PagedResult<MemberQueryResultDto>>>> QueryMembers(
        [FromBody] MemberQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryMembersAsync(request);
            return result;
        }, "查詢會員列表失敗");
    }

    /// <summary>
    /// 查詢會員交易記錄
    /// </summary>
    [HttpGet("{memberId}/transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<MemberTransactionDto>>>> GetMemberTransactions(
        string memberId,
        [FromQuery] MemberPointQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMemberTransactionsAsync(memberId, query);
            return result;
        }, $"查詢會員交易記錄失敗: {memberId}");
    }

    /// <summary>
    /// 查詢會員回門禮卡號補登記錄
    /// </summary>
    [HttpGet("exchange-logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<MemberExchangeLogDto>>>> GetExchangeLogs(
        [FromQuery] MemberExchangeLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetExchangeLogsAsync(query);
            return result;
        }, "查詢會員回門禮卡號補登記錄失敗");
    }

    /// <summary>
    /// 匯出會員查詢結果
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportMembers([FromBody] MemberExportRequestDto request)
    {
        try
        {
            var fileBytes = await _service.ExportMembersAsync(request);
            var fileName = $"會員查詢結果_{DateTime.Now:yyyyMMddHHmmss}.{request.ExportType.ToLower()}";
            var contentType = request.ExportType == "Excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出會員查詢結果失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出失敗: " + ex.Message));
        }
    }

    /// <summary>
    /// 列印會員報表
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult> PrintMemberReport([FromBody] MemberPrintRequestDto request)
    {
        try
        {
            var fileBytes = await _service.PrintMemberReportAsync(request);
            return File(fileBytes, "application/pdf", $"會員報表_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印會員報表失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("列印失敗: " + ex.Message));
        }
    }
}

