using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Application.Services.StandardModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StandardModule;

/// <summary>
/// STD3000 標準模組控制器 (SYS3620 - 標準資料維護)
/// </summary>
[Route("api/v1/std3000")]
public class Std3000Controller : BaseController
{
    private readonly IStd3000DataService _service;

    public Std3000Controller(
        IStd3000DataService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢STD3000資料列表
    /// </summary>
    [HttpGet("data")]
    public async Task<ActionResult<ApiResponse<PagedResult<Std3000DataDto>>>> GetStd3000DataList(
        [FromQuery] Std3000DataQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStd3000DataListAsync(query);
            return result;
        }, "查詢STD3000資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆STD3000資料
    /// </summary>
    [HttpGet("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<Std3000DataDto>>> GetStd3000Data(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetStd3000DataByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }
            return result;
        }, $"查詢STD3000資料失敗: {tKey}");
    }

    /// <summary>
    /// 新增STD3000資料
    /// </summary>
    [HttpPost("data")]
    public async Task<ActionResult<ApiResponse<long>>> CreateStd3000Data(
        [FromBody] CreateStd3000DataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateStd3000DataAsync(dto);
            return result;
        }, "新增STD3000資料失敗");
    }

    /// <summary>
    /// 修改STD3000資料
    /// </summary>
    [HttpPut("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStd3000Data(
        long tKey,
        [FromBody] UpdateStd3000DataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateStd3000DataAsync(tKey, dto);
        }, $"修改STD3000資料失敗: {tKey}");
    }

    /// <summary>
    /// 刪除STD3000資料
    /// </summary>
    [HttpDelete("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStd3000Data(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteStd3000DataAsync(tKey);
        }, $"刪除STD3000資料失敗: {tKey}");
    }
}

