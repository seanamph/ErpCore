using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Pos;
using ErpCore.Application.Services.Pos;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Pos;

/// <summary>
/// POS資料同步作業控制器
/// </summary>
[Route("api/v1/pos/sync")]
public class PosSyncController : BaseController
{
    private readonly IPosSyncService _service;

    public PosSyncController(
        IPosSyncService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 同步POS交易資料
    /// </summary>
    [HttpPost("transactions")]
    public async Task<ActionResult<ApiResponse<PosSyncResultDto>>> SyncTransactions(
        [FromBody] PosSyncRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SyncTransactionsAsync(request);
            return result;
        }, "同步POS交易資料失敗");
    }

    /// <summary>
    /// 查詢POS同步記錄列表
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<PosSyncLogDto>>>> GetSyncLogs(
        [FromQuery] PosSyncLogQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSyncLogsAsync(query);
            return result;
        }, "查詢POS同步記錄列表失敗");
    }

    /// <summary>
    /// 根據ID查詢POS同步記錄
    /// </summary>
    [HttpGet("logs/{id}")]
    public async Task<ActionResult<ApiResponse<PosSyncLogDto>>> GetSyncLog(long id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSyncLogByIdAsync(id);
            if (result == null)
            {
                throw new Exception($"找不到POS同步記錄: {id}");
            }
            return result;
        }, $"查詢POS同步記錄失敗: {id}");
    }
}

