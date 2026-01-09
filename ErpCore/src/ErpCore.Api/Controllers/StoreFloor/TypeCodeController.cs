using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StoreFloor;

/// <summary>
/// 類型代碼維護控制器 (SYS6405-SYS6490)
/// </summary>
[Route("api/v1/type-codes")]
public class TypeCodeController : BaseController
{
    private readonly ITypeCodeService _service;

    public TypeCodeController(
        ITypeCodeService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢類型代碼列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TypeCodeDto>>>> GetTypeCodes(
        [FromQuery] TypeCodeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTypeCodesAsync(query);
            return result;
        }, "查詢類型代碼列表失敗");
    }

    /// <summary>
    /// 查詢單筆類型代碼
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<TypeCodeDto>>> GetTypeCode(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTypeCodeByIdAsync(tKey);
            return result;
        }, $"查詢類型代碼失敗: {tKey}");
    }

    /// <summary>
    /// 新增類型代碼
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateTypeCode(
        [FromBody] CreateTypeCodeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateTypeCodeAsync(dto);
            return result;
        }, "新增類型代碼失敗");
    }

    /// <summary>
    /// 修改類型代碼
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTypeCode(
        long tKey,
        [FromBody] UpdateTypeCodeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateTypeCodeAsync(tKey, dto);
        }, $"修改類型代碼失敗: {tKey}");
    }

    /// <summary>
    /// 刪除類型代碼
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTypeCode(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTypeCodeAsync(tKey);
        }, $"刪除類型代碼失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除類型代碼
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> BatchDeleteTypeCodes(
        [FromBody] List<long> tKeys)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteTypeCodesAsync(tKeys);
        }, "批次刪除類型代碼失敗");
    }

    /// <summary>
    /// 檢查類型代碼是否存在
    /// </summary>
    [HttpGet("check")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckTypeCodeExists(
        [FromQuery] string typeCode,
        [FromQuery] string? category)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExistsAsync(typeCode, category);
            return result;
        }, $"檢查類型代碼是否存在失敗: {typeCode}/{category}");
    }
}

