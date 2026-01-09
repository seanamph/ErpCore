using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreFloor;

/// <summary>
/// POS資料維護控制器 (SYS6610-SYS6999)
/// </summary>
[Route("api/v1/pos-terminals")]
public class PosDataController : BaseController
{
    private readonly IPosTerminalService _service;

    public PosDataController(
        IPosTerminalService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢POS終端列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PosTerminalDto>>>> GetPosTerminals(
        [FromQuery] PosTerminalQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPosTerminalsAsync(query);
            return result;
        }, "查詢POS終端列表失敗");
    }

    /// <summary>
    /// 查詢單筆POS終端
    /// </summary>
    [HttpGet("{posTerminalId}")]
    public async Task<ActionResult<ApiResponse<PosTerminalDto>>> GetPosTerminal(string posTerminalId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPosTerminalByIdAsync(posTerminalId);
            return result;
        }, $"查詢POS終端失敗: {posTerminalId}");
    }

    /// <summary>
    /// 新增POS終端
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePosTerminal(
        [FromBody] CreatePosTerminalDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreatePosTerminalAsync(dto);
            return result;
        }, "新增POS終端失敗");
    }

    /// <summary>
    /// 修改POS終端
    /// </summary>
    [HttpPut("{posTerminalId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePosTerminal(
        string posTerminalId,
        [FromBody] UpdatePosTerminalDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePosTerminalAsync(posTerminalId, dto);
        }, $"修改POS終端失敗: {posTerminalId}");
    }

    /// <summary>
    /// 刪除POS終端
    /// </summary>
    [HttpDelete("{posTerminalId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePosTerminal(string posTerminalId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePosTerminalAsync(posTerminalId);
        }, $"刪除POS終端失敗: {posTerminalId}");
    }

    /// <summary>
    /// POS資料同步
    /// </summary>
    [HttpPost("{posTerminalId}/sync")]
    public async Task<ActionResult<ApiResponse<object>>> SyncPosTerminal(string posTerminalId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.SyncPosTerminalAsync(posTerminalId);
        }, $"同步POS終端失敗: {posTerminalId}");
    }
}

