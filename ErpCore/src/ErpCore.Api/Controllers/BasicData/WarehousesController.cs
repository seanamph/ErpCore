using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 庫別資料維護控制器 (SYSWB60)
/// </summary>
[Route("api/v1/warehouses")]
public class WarehousesController : BaseController
{
    private readonly IWarehouseService _service;

    public WarehousesController(
        IWarehouseService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢庫別列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<WarehouseDto>>>> GetWarehouses(
        [FromQuery] WarehouseQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetWarehousesAsync(query);
            return result;
        }, "查詢庫別列表失敗");
    }

    /// <summary>
    /// 查詢單筆庫別
    /// </summary>
    [HttpGet("{warehouseId}")]
    public async Task<ActionResult<ApiResponse<WarehouseDto>>> GetWarehouse(string warehouseId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetWarehouseByIdAsync(warehouseId);
            return result;
        }, $"查詢庫別失敗: {warehouseId}");
    }

    /// <summary>
    /// 新增庫別
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateWarehouse(
        [FromBody] CreateWarehouseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateWarehouseAsync(dto);
            return result;
        }, "新增庫別失敗");
    }

    /// <summary>
    /// 修改庫別
    /// </summary>
    [HttpPut("{warehouseId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateWarehouse(
        string warehouseId,
        [FromBody] UpdateWarehouseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateWarehouseAsync(warehouseId, dto);
        }, $"修改庫別失敗: {warehouseId}");
    }

    /// <summary>
    /// 刪除庫別
    /// </summary>
    [HttpDelete("{warehouseId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteWarehouse(string warehouseId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteWarehouseAsync(warehouseId);
        }, $"刪除庫別失敗: {warehouseId}");
    }

    /// <summary>
    /// 批次刪除庫別
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteWarehousesBatch(
        [FromBody] BatchDeleteWarehouseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteWarehousesBatchAsync(dto);
        }, "批次刪除庫別失敗");
    }

    /// <summary>
    /// 更新庫別狀態
    /// </summary>
    [HttpPut("{warehouseId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateWarehouseStatus(
        string warehouseId,
        [FromBody] UpdateWarehouseStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStatusAsync(warehouseId, dto);
        }, $"更新庫別狀態失敗: {warehouseId}");
    }
}
