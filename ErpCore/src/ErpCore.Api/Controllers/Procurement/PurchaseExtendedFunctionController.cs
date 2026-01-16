using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 採購擴展功能控制器 (SYSP610)
/// </summary>
[ApiController]
[Route("api/v1/purchase-extended")]
public class PurchaseExtendedFunctionController : BaseController
{
    private readonly IPurchaseExtendedFunctionService _service;

    public PurchaseExtendedFunctionController(
        IPurchaseExtendedFunctionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢採購擴展功能列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseExtendedFunctionDto>>>> GetPurchaseExtendedFunctions(
        [FromQuery] PurchaseExtendedFunctionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseExtendedFunctionsAsync(query);
            return result;
        }, "查詢採購擴展功能列表失敗");
    }

    /// <summary>
    /// 查詢單筆採購擴展功能（根據主鍵）
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<PurchaseExtendedFunctionDto>>> GetPurchaseExtendedFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseExtendedFunctionByTKeyAsync(tKey);
            return result;
        }, $"查詢採購擴展功能失敗: {tKey}");
    }

    /// <summary>
    /// 查詢單筆採購擴展功能（根據功能代碼）
    /// </summary>
    [HttpGet("by-id/{extFunctionId}")]
    public async Task<ActionResult<ApiResponse<PurchaseExtendedFunctionDto>>> GetPurchaseExtendedFunctionByExtFunctionId(string extFunctionId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPurchaseExtendedFunctionByExtFunctionIdAsync(extFunctionId);
            return result;
        }, $"查詢採購擴展功能失敗: {extFunctionId}");
    }

    /// <summary>
    /// 新增採購擴展功能
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreatePurchaseExtendedFunction(
        [FromBody] CreatePurchaseExtendedFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _service.CreatePurchaseExtendedFunctionAsync(dto);
            return tKey;
        }, "新增採購擴展功能失敗");
    }

    /// <summary>
    /// 修改採購擴展功能
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePurchaseExtendedFunction(
        long tKey,
        [FromBody] UpdatePurchaseExtendedFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePurchaseExtendedFunctionAsync(tKey, dto);
        }, $"修改採購擴展功能失敗: {tKey}");
    }

    /// <summary>
    /// 刪除採購擴展功能
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePurchaseExtendedFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePurchaseExtendedFunctionAsync(tKey);
        }, $"刪除採購擴展功能失敗: {tKey}");
    }

    /// <summary>
    /// 檢查採購擴展功能是否存在
    /// </summary>
    [HttpGet("{extFunctionId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckPurchaseExtendedFunctionExists(string extFunctionId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(extFunctionId);
            return exists;
        }, $"檢查採購擴展功能是否存在失敗: {extFunctionId}");
    }
}
