using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.UniversalModule;
using ErpCore.Application.Services.UniversalModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.UniversalModule;

/// <summary>
/// 通用模組控制器 (UNIV000系列)
/// </summary>
[Route("api/v1/univ000")]
public class Univ000Controller : BaseController
{
    private readonly IUniv000Service _service;

    public Univ000Controller(
        IUniv000Service service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢通用模組資料列表
    /// </summary>
    [HttpGet("data")]
    public async Task<ActionResult<ApiResponse<PagedResult<Univ000Dto>>>> GetUniv000List(
        [FromQuery] Univ000QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUniv000ListAsync(query);
            return result;
        }, "查詢通用模組資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆通用模組資料
    /// </summary>
    [HttpGet("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<Univ000Dto>>> GetUniv000(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUniv000ByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }
            return result;
        }, $"查詢通用模組資料失敗: {tKey}");
    }

    /// <summary>
    /// 新增通用模組資料
    /// </summary>
    [HttpPost("data")]
    public async Task<ActionResult<ApiResponse<long>>> CreateUniv000(
        [FromBody] CreateUniv000Dto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateUniv000Async(dto);
            return result;
        }, "新增通用模組資料失敗");
    }

    /// <summary>
    /// 修改通用模組資料
    /// </summary>
    [HttpPut("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUniv000(
        long tKey,
        [FromBody] UpdateUniv000Dto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateUniv000Async(tKey, dto);
        }, $"修改通用模組資料失敗: {tKey}");
    }

    /// <summary>
    /// 刪除通用模組資料
    /// </summary>
    [HttpDelete("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUniv000(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteUniv000Async(tKey);
        }, $"刪除通用模組資料失敗: {tKey}");
    }
}

