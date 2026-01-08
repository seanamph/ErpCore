using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BasicData;

/// <summary>
/// 廠/客基本資料維護作業控制器 (SYSB206)
/// </summary>
[Route("api/v1/vendors")]
public class VendorsController : BaseController
{
    private readonly IVendorService _service;

    public VendorsController(
        IVendorService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢廠商列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<VendorDto>>>> GetVendors(
        [FromQuery] VendorQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetVendorsAsync(query);
            return result;
        }, "查詢廠商列表失敗");
    }

    /// <summary>
    /// 查詢單筆廠商
    /// </summary>
    [HttpGet("{vendorId}")]
    public async Task<ActionResult<ApiResponse<VendorDto>>> GetVendor(string vendorId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetVendorByIdAsync(vendorId);
            return result;
        }, $"查詢廠商失敗: {vendorId}");
    }

    /// <summary>
    /// 檢查統一編號是否存在
    /// </summary>
    [HttpGet("check-gui-id/{guiId}")]
    public async Task<ActionResult<ApiResponse<CheckGuiIdResultDto>>> CheckGuiId(string guiId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CheckGuiIdAsync(guiId);
            return result;
        }, $"檢查統一編號失敗: {guiId}");
    }

    /// <summary>
    /// 新增廠商
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateVendor(
        [FromBody] CreateVendorDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateVendorAsync(dto);
            return result;
        }, "新增廠商失敗");
    }

    /// <summary>
    /// 修改廠商
    /// </summary>
    [HttpPut("{vendorId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateVendor(
        string vendorId,
        [FromBody] UpdateVendorDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateVendorAsync(vendorId, dto);
        }, $"修改廠商失敗: {vendorId}");
    }

    /// <summary>
    /// 刪除廠商
    /// </summary>
    [HttpDelete("{vendorId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVendor(string vendorId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteVendorAsync(vendorId);
        }, $"刪除廠商失敗: {vendorId}");
    }

    /// <summary>
    /// 批次刪除廠商
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVendorsBatch(
        [FromBody] BatchDeleteVendorDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteVendorsBatchAsync(dto);
        }, "批次刪除廠商失敗");
    }
}

