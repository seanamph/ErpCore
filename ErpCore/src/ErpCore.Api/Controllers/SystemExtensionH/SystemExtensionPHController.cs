using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SystemExtensionH;
using ErpCore.Application.Services.SystemExtensionH;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemExtensionH;

/// <summary>
/// 系統擴展PH控制器 (SYSPH00 - 感應卡維護作業)
/// </summary>
[Route("api/v1/emp-cards")]
public class SystemExtensionPHController : BaseController
{
    private readonly ISystemExtensionPHService _service;

    public SystemExtensionPHController(
        ISystemExtensionPHService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢感應卡列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EmpCardDto>>>> GetEmpCards(
        [FromQuery] EmpCardQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEmpCardsAsync(query);
            return result;
        }, "查詢感應卡列表失敗");
    }

    /// <summary>
    /// 查詢單筆感應卡
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<EmpCardDto>>> GetEmpCard(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEmpCardByIdAsync(tKey);
            return result;
        }, $"查詢感應卡失敗: {tKey}");
    }

    /// <summary>
    /// 新增感應卡
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateEmpCard(
        [FromBody] CreateEmpCardDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateEmpCardAsync(dto);
            return result;
        }, "新增感應卡失敗");
    }

    /// <summary>
    /// 批量新增感應卡
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<ApiResponse<int>>> CreateBatchEmpCards(
        [FromBody] CreateBatchEmpCardDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateBatchEmpCardsAsync(dto);
            return result;
        }, "批量新增感應卡失敗");
    }

    /// <summary>
    /// 修改感應卡
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateEmpCard(
        long tKey,
        [FromBody] UpdateEmpCardDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateEmpCardAsync(tKey, dto);
            return (object)null!;
        }, $"修改感應卡失敗: {tKey}");
    }

    /// <summary>
    /// 刪除感應卡
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEmpCard(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteEmpCardAsync(tKey);
            return (object)null!;
        }, $"刪除感應卡失敗: {tKey}");
    }
}

