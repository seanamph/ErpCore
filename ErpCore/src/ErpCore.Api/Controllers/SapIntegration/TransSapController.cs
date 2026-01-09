using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SapIntegration;
using ErpCore.Application.Services.SapIntegration;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SapIntegration;

/// <summary>
/// SAP整合模組控制器 (TransSAP系列)
/// </summary>
[Route("api/v1/sap")]
public class TransSapController : BaseController
{
    private readonly ITransSapService _service;

    public TransSapController(
        ITransSapService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢SAP整合資料列表
    /// </summary>
    [HttpGet("trans")]
    public async Task<ActionResult<ApiResponse<PagedResult<TransSapDto>>>> GetTransSapList(
        [FromQuery] TransSapQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransSapListAsync(query);
            return result;
        }, "查詢SAP整合資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆SAP整合資料
    /// </summary>
    [HttpGet("trans/{tKey}")]
    public async Task<ActionResult<ApiResponse<TransSapDto>>> GetTransSap(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransSapByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }
            return result;
        }, $"查詢SAP整合資料失敗: {tKey}");
    }

    /// <summary>
    /// 新增SAP整合資料
    /// </summary>
    [HttpPost("trans")]
    public async Task<ActionResult<ApiResponse<long>>> CreateTransSap(
        [FromBody] CreateTransSapDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateTransSapAsync(dto);
            return result;
        }, "新增SAP整合資料失敗");
    }

    /// <summary>
    /// 修改SAP整合資料
    /// </summary>
    [HttpPut("trans/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateTransSap(
        long tKey,
        [FromBody] UpdateTransSapDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateTransSapAsync(tKey, dto);
        }, $"修改SAP整合資料失敗: {tKey}");
    }

    /// <summary>
    /// 刪除SAP整合資料
    /// </summary>
    [HttpDelete("trans/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTransSap(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTransSapAsync(tKey);
        }, $"刪除SAP整合資料失敗: {tKey}");
    }
}

