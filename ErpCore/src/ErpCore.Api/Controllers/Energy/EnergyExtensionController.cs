using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Energy;
using ErpCore.Application.Services.Energy;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Energy;

/// <summary>
/// 能源擴展控制器 (SYSOU10-SYSOU33 - 能源擴展功能)
/// </summary>
[Route("api/v1/energy-extension")]
public class EnergyExtensionController : BaseController
{
    private readonly IEnergyExtensionService _service;

    public EnergyExtensionController(
        IEnergyExtensionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢能源擴展資料列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EnergyExtensionDto>>>> GetEnergyExtensions(
        [FromQuery] EnergyExtensionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEnergyExtensionsAsync(query);
            return result;
        }, "查詢能源擴展資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆能源擴展資料
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<EnergyExtensionDto>>> GetEnergyExtension(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEnergyExtensionByIdAsync(tKey);
            return result;
        }, $"查詢能源擴展資料失敗: {tKey}");
    }

    /// <summary>
    /// 新增能源擴展資料
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateEnergyExtension(
        [FromBody] CreateEnergyExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateEnergyExtensionAsync(dto);
            return result;
        }, "新增能源擴展資料失敗");
    }

    /// <summary>
    /// 修改能源擴展資料
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateEnergyExtension(
        long tKey,
        [FromBody] UpdateEnergyExtensionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateEnergyExtensionAsync(tKey, dto);
            return true;
        }, $"修改能源擴展資料失敗: {tKey}");
    }

    /// <summary>
    /// 刪除能源擴展資料
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEnergyExtension(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteEnergyExtensionAsync(tKey);
            return true;
        }, $"刪除能源擴展資料失敗: {tKey}");
    }
}

