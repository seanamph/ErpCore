using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.MirModule;
using ErpCore.Application.Services.MirModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.MirModule;

/// <summary>
/// MIRV000 憑證管理模組控制器
/// </summary>
[Route("api/v1/mirv000")]
public class MirV000Controller : BaseController
{
    private readonly IMirV000DataService _dataService;

    public MirV000Controller(
        IMirV000DataService dataService,
        ILoggerService logger) : base(logger)
    {
        _dataService = dataService;
    }

    /// <summary>
    /// 查詢資料列表
    /// </summary>
    [HttpGet("data")]
    public async Task<ActionResult<ApiResponse<PagedResult<MirV000DataDto>>>> GetDataList(
        [FromQuery] MirV000DataQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dataService.GetDataListAsync(query);
            return result;
        }, "查詢MIRV000資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆資料
    /// </summary>
    [HttpGet("data/{dataId}")]
    public async Task<ActionResult<ApiResponse<MirV000DataDto>>> GetData(string dataId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dataService.GetDataByIdAsync(dataId);
            return result;
        }, $"查詢MIRV000資料失敗: {dataId}");
    }

    /// <summary>
    /// 新增資料
    /// </summary>
    [HttpPost("data")]
    public async Task<ActionResult<ApiResponse<string>>> CreateData(
        [FromBody] CreateMirV000DataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dataService.CreateDataAsync(dto);
            return result;
        }, "新增MIRV000資料失敗");
    }

    /// <summary>
    /// 修改資料
    /// </summary>
    [HttpPut("data/{dataId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateData(
        string dataId,
        [FromBody] UpdateMirV000DataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _dataService.UpdateDataAsync(dataId, dto);
            return new object();
        }, $"修改MIRV000資料失敗: {dataId}");
    }

    /// <summary>
    /// 刪除資料
    /// </summary>
    [HttpDelete("data/{dataId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteData(string dataId)
    {
        return await ExecuteAsync(async () =>
        {
            await _dataService.DeleteDataAsync(dataId);
            return new object();
        }, $"刪除MIRV000資料失敗: {dataId}");
    }
}

