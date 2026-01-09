using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Energy;
using ErpCore.Application.Services.Energy;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Energy;

/// <summary>
/// 能源處理控制器 (SYSO310 - 能源處理功能)
/// </summary>
[Route("api/v1/energy-process")]
public class EnergyProcessController : BaseController
{
    private readonly IEnergyProcessService _service;

    public EnergyProcessController(
        IEnergyProcessService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢能源處理資料列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EnergyProcessDto>>>> GetEnergyProcesses(
        [FromQuery] EnergyProcessQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEnergyProcessesAsync(query);
            return result;
        }, "查詢能源處理資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆能源處理資料
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<EnergyProcessDto>>> GetEnergyProcess(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEnergyProcessByIdAsync(tKey);
            return result;
        }, $"查詢能源處理資料失敗: {tKey}");
    }

    /// <summary>
    /// 新增能源處理資料
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateEnergyProcess(
        [FromBody] CreateEnergyProcessDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateEnergyProcessAsync(dto);
            return result;
        }, "新增能源處理資料失敗");
    }

    /// <summary>
    /// 修改能源處理資料
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateEnergyProcess(
        long tKey,
        [FromBody] UpdateEnergyProcessDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateEnergyProcessAsync(tKey, dto);
            return true;
        }, $"修改能源處理資料失敗: {tKey}");
    }

    /// <summary>
    /// 刪除能源處理資料
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEnergyProcess(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteEnergyProcessAsync(tKey);
            return true;
        }, $"刪除能源處理資料失敗: {tKey}");
    }
}

