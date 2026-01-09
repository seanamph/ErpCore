using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Energy;
using ErpCore.Application.Services.Energy;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Energy;

/// <summary>
/// 能源基礎控制器 (SYSO100-SYSO130 - 能源基礎功能)
/// </summary>
[Route("api/v1/energy-base")]
public class EnergyBaseController : BaseController
{
    private readonly IEnergyBaseService _service;

    public EnergyBaseController(
        IEnergyBaseService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢能源基礎資料列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EnergyBaseDto>>>> GetEnergyBases(
        [FromQuery] EnergyBaseQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEnergyBasesAsync(query);
            return result;
        }, "查詢能源基礎資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆能源基礎資料
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<EnergyBaseDto>>> GetEnergyBase(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEnergyBaseByIdAsync(tKey);
            return result;
        }, $"查詢能源基礎資料失敗: {tKey}");
    }

    /// <summary>
    /// 新增能源基礎資料
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateEnergyBase(
        [FromBody] CreateEnergyBaseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateEnergyBaseAsync(dto);
            return result;
        }, "新增能源基礎資料失敗");
    }

    /// <summary>
    /// 修改能源基礎資料
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateEnergyBase(
        long tKey,
        [FromBody] UpdateEnergyBaseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateEnergyBaseAsync(tKey, dto);
            return true;
        }, $"修改能源基礎資料失敗: {tKey}");
    }

    /// <summary>
    /// 刪除能源基礎資料
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEnergyBase(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteEnergyBaseAsync(tKey);
            return true;
        }, $"刪除能源基礎資料失敗: {tKey}");
    }
}

