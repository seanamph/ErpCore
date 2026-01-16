using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 採購擴展維護控制器 (SYSPA10-SYSPB60)
/// </summary>
[ApiController]
[Route("api/v1/purchase-extended-maintenance")]
public class PurchaseExtendedMaintenanceController : BaseController
{
    private readonly IPurchaseExtendedMaintenanceService _service;

    public PurchaseExtendedMaintenanceController(
        IPurchaseExtendedMaintenanceService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購擴展維護列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseExtendedMaintenanceDto>>>> GetPurchaseExtendedMaintenances(
        [FromQuery] PurchaseExtendedMaintenanceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseExtendedMaintenancesAsync(query);
            return result;
        }, "查詢採購擴展維護列表失敗");
    }

    /// <summary>
    /// 查詢單筆採購擴展維護（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<PurchaseExtendedMaintenanceDto>>> GetPurchaseExtendedMaintenance(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseExtendedMaintenanceByTKeyAsync(tKey);
            return result;
        }, $"查詢採購擴展維護失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆採購擴展維護（根據維護代碼）
    /// </summary>
    [HttpGet("by-id/{maintenanceId}")]
    public async Task<ActionResult<ApiResponse<PurchaseExtendedMaintenanceDto>>> GetPurchaseExtendedMaintenanceByMaintenanceId(string maintenanceId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseExtendedMaintenanceByMaintenanceIdAsync(maintenanceId);
            return result;
        }, $"查詢採購擴展維護失敗: {maintenanceId}");
    }

    /// <summary>
    /// 新增採購擴展維護
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreatePurchaseExtendedMaintenance(
        [FromBody] CreatePurchaseExtendedMaintenanceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _service.CreatePurchaseExtendedMaintenanceAsync(dto);
            return tKey;
        }, "新增採購擴展維護失敗");
    }

    /// <summary>
    /// 修改採購擴展維護
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePurchaseExtendedMaintenance(
        long tKey,
        [FromBody] UpdatePurchaseExtendedMaintenanceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePurchaseExtendedMaintenanceAsync(tKey, dto);
        }, $"修改採購擴展維護失敗: {tKey}");
    }

    /// <summary>
    /// 刪除採購擴展維護
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePurchaseExtendedMaintenance(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePurchaseExtendedMaintenanceAsync(tKey);
        }, $"刪除採購擴展維護失敗: {tKey}");
    }

    /// <summary>
    /// 檢查採購擴展維護是否存在
    /// </summary>
    [HttpGet("{maintenanceId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckPurchaseExtendedMaintenanceExists(string maintenanceId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(maintenanceId);
            return exists;
        }, $"檢查採購擴展維護是否存在失敗: {maintenanceId}");
    }
}
