using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Procurement;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Procurement;

/// <summary>
/// 供應商管理控制器 (SYSP210-SYSP260)
/// </summary>
[ApiController]
[Route("api/v1/suppliers")]
public class SupplierController : BaseController
{
    private readonly ISupplierService _service;

    public SupplierController(
        ISupplierService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢供應商列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplierDto>>>> GetSuppliers(
        [FromQuery] SupplierQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSuppliersAsync(query);
            return result;
        }, "查詢供應商列表失敗");
    }

    /// <summary>
    /// 查詢單筆供應商
    /// </summary>
    [HttpGet("{supplierId}")]
    public async Task<ActionResult<ApiResponse<SupplierDto>>> GetSupplier(string supplierId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSupplierByIdAsync(supplierId);
            return result;
        }, $"查詢供應商失敗: {supplierId}");
    }

    /// <summary>
    /// 新增供應商
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateSupplier(
        [FromBody] CreateSupplierDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var supplierId = await _service.CreateSupplierAsync(dto);
            return supplierId;
        }, "新增供應商失敗");
    }

    /// <summary>
    /// 修改供應商
    /// </summary>
    [HttpPut("{supplierId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSupplier(
        string supplierId,
        [FromBody] UpdateSupplierDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSupplierAsync(supplierId, dto);
        }, $"修改供應商失敗: {supplierId}");
    }

    /// <summary>
    /// 刪除供應商
    /// </summary>
    [HttpDelete("{supplierId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSupplier(string supplierId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSupplierAsync(supplierId);
        }, $"刪除供應商失敗: {supplierId}");
    }

    /// <summary>
    /// 檢查供應商是否存在
    /// </summary>
    [HttpGet("{supplierId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckSupplierExists(string supplierId)
    {
        return await ExecuteAsync(async () =>
        {
            var exists = await _service.ExistsAsync(supplierId);
            return exists;
        }, $"檢查供應商是否存在失敗: {supplierId}");
    }
}

