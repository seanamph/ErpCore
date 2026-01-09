using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.MshModule;
using ErpCore.Application.Services.MshModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.MshModule;

/// <summary>
/// MSH3000 模組控制器
/// </summary>
[Route("api/v1/msh3000")]
public class Msh3000Controller : BaseController
{
    private readonly IMsh3000DataService _service;

    public Msh3000Controller(
        IMsh3000DataService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢資料列表
    /// </summary>
    [HttpGet("data")]
    public async Task<ActionResult<ApiResponse<PagedResult<Msh3000DataDto>>>> GetDataList(
        [FromQuery] Msh3000DataQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetDataListAsync(query);
            return result;
        }, "查詢MSH3000資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆資料
    /// </summary>
    [HttpGet("data/{dataId}")]
    public async Task<ActionResult<ApiResponse<Msh3000DataDto>>> GetData(string dataId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetDataByIdAsync(dataId);
            return result;
        }, $"查詢MSH3000資料失敗: {dataId}");
    }

    /// <summary>
    /// 新增資料
    /// </summary>
    [HttpPost("data")]
    public async Task<ActionResult<ApiResponse<string>>> CreateData(
        [FromBody] CreateMsh3000DataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateDataAsync(dto);
            return result;
        }, "新增MSH3000資料失敗");
    }

    /// <summary>
    /// 修改資料
    /// </summary>
    [HttpPut("data/{dataId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateData(
        string dataId,
        [FromBody] UpdateMsh3000DataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateDataAsync(dataId, dto);
            return new object();
        }, $"修改MSH3000資料失敗: {dataId}");
    }

    /// <summary>
    /// 刪除資料
    /// </summary>
    [HttpDelete("data/{dataId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteData(string dataId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteDataAsync(dataId);
            return new object();
        }, $"刪除MSH3000資料失敗: {dataId}");
    }
}

