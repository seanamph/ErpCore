using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 可管控欄位控制器 (SYS0510)
/// </summary>
[Route("api/v1/controllable-fields")]
public class ControllableFieldsController : BaseController
{
    private readonly IControllableFieldService _service;

    public ControllableFieldsController(
        IControllableFieldService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢可管控欄位列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ControllableFieldDto>>>> GetControllableFields(
        [FromQuery] ControllableFieldQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetControllableFieldsAsync(query);
            return result;
        }, "查詢可管控欄位列表失敗");
    }

    /// <summary>
    /// 查詢單筆可管控欄位
    /// </summary>
    [HttpGet("{fieldId}")]
    public async Task<ActionResult<ApiResponse<ControllableFieldDto>>> GetControllableField(string fieldId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetControllableFieldByIdAsync(fieldId);
            if (result == null)
            {
                throw new InvalidOperationException($"可管控欄位不存在: {fieldId}");
            }
            return result;
        }, $"查詢可管控欄位失敗: {fieldId}");
    }

    /// <summary>
    /// 根據資料庫和表格查詢欄位列表
    /// </summary>
    [HttpGet("db-table")]
    public async Task<ActionResult<ApiResponse<List<ControllableFieldDto>>>> GetControllableFieldsByDbTable(
        [FromQuery] string dbName,
        [FromQuery] string tableName)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetControllableFieldsByDbTableAsync(dbName, tableName);
            return result;
        }, $"查詢可管控欄位失敗: {dbName} - {tableName}");
    }

    /// <summary>
    /// 新增可管控欄位
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ControllableFieldDto>>> CreateControllableField(
        [FromBody] CreateControllableFieldDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateControllableFieldAsync(dto);
            return result;
        }, "新增可管控欄位失敗");
    }

    /// <summary>
    /// 修改可管控欄位
    /// </summary>
    [HttpPut("{fieldId}")]
    public async Task<ActionResult<ApiResponse<ControllableFieldDto>>> UpdateControllableField(
        string fieldId,
        [FromBody] UpdateControllableFieldDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UpdateControllableFieldAsync(fieldId, dto);
            return result;
        }, $"修改可管控欄位失敗: {fieldId}");
    }

    /// <summary>
    /// 刪除可管控欄位
    /// </summary>
    [HttpDelete("{fieldId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteControllableField(string fieldId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteControllableFieldAsync(fieldId);
        }, $"刪除可管控欄位失敗: {fieldId}");
    }

    /// <summary>
    /// 批量刪除可管控欄位
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<BatchOperationResult>>> BatchDeleteControllableFields(
        [FromBody] List<string> fieldIds)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchDeleteControllableFieldsAsync(fieldIds);
            return result;
        }, "批量刪除可管控欄位失敗");
    }
}

