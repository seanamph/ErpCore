using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CommunicationModule;
using ErpCore.Application.Services.CommunicationModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CommunicationModule;

/// <summary>
/// XCOM000系列通訊模組控制器
/// </summary>
[Route("api/v1/communication")]
public class XcomController : BaseController
{
    private readonly ISystemCommunicationService _systemCommunicationService;
    private readonly IXComSystemParamService _systemParamService;

    public XcomController(
        ISystemCommunicationService systemCommunicationService,
        IXComSystemParamService systemParamService,
        ILoggerService logger) : base(logger)
    {
        _systemCommunicationService = systemCommunicationService;
        _systemParamService = systemParamService;
    }

    /// <summary>
    /// 查詢系統通訊設定列表
    /// </summary>
    [HttpGet("systems")]
    public async Task<ActionResult<ApiResponse<PagedResult<SystemCommunicationDto>>>> GetSystemCommunications(
        [FromQuery] SystemCommunicationQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _systemCommunicationService.GetSystemCommunicationsAsync(query);
            return result;
        }, "查詢系統通訊設定列表失敗");
    }

    /// <summary>
    /// 查詢單筆系統通訊設定
    /// </summary>
    [HttpGet("systems/{communicationId}")]
    public async Task<ActionResult<ApiResponse<SystemCommunicationDto>>> GetSystemCommunication(long communicationId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _systemCommunicationService.GetSystemCommunicationByIdAsync(communicationId);
            return result;
        }, $"查詢系統通訊設定失敗: {communicationId}");
    }

    /// <summary>
    /// 新增系統通訊設定
    /// </summary>
    [HttpPost("systems")]
    public async Task<ActionResult<ApiResponse<long>>> CreateSystemCommunication(
        [FromBody] CreateSystemCommunicationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var id = await _systemCommunicationService.CreateSystemCommunicationAsync(dto);
            return id;
        }, "新增系統通訊設定失敗");
    }

    /// <summary>
    /// 修改系統通訊設定
    /// </summary>
    [HttpPut("systems/{communicationId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSystemCommunication(
        long communicationId,
        [FromBody] UpdateSystemCommunicationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _systemCommunicationService.UpdateSystemCommunicationAsync(communicationId, dto);
            return new object();
        }, $"修改系統通訊設定失敗: {communicationId}");
    }

    /// <summary>
    /// 刪除系統通訊設定
    /// </summary>
    [HttpDelete("systems/{communicationId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSystemCommunication(long communicationId)
    {
        return await ExecuteAsync(async () =>
        {
            await _systemCommunicationService.DeleteSystemCommunicationAsync(communicationId);
            return new object();
        }, $"刪除系統通訊設定失敗: {communicationId}");
    }

    /// <summary>
    /// 查詢XCOM系統參數列表 (XCOM2A0)
    /// </summary>
    [HttpGet("system-params")]
    public async Task<ActionResult<ApiResponse<PagedResult<XComSystemParamDto>>>> GetSystemParams(
        [FromQuery] XComSystemParamQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _systemParamService.GetSystemParamsAsync(query);
            return result;
        }, "查詢XCOM系統參數列表失敗");
    }

    /// <summary>
    /// 查詢單筆XCOM系統參數 (XCOM2A0)
    /// </summary>
    [HttpGet("system-params/{paramCode}")]
    public async Task<ActionResult<ApiResponse<XComSystemParamDto>>> GetSystemParam(string paramCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _systemParamService.GetSystemParamByIdAsync(paramCode);
            return result;
        }, $"查詢XCOM系統參數失敗: {paramCode}");
    }

    /// <summary>
    /// 新增XCOM系統參數 (XCOM2A0)
    /// </summary>
    [HttpPost("system-params")]
    public async Task<ActionResult<ApiResponse<object>>> CreateSystemParam(
        [FromBody] CreateXComSystemParamDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _systemParamService.CreateSystemParamAsync(dto);
            return new object();
        }, "新增XCOM系統參數失敗");
    }

    /// <summary>
    /// 修改XCOM系統參數 (XCOM2A0)
    /// </summary>
    [HttpPut("system-params/{paramCode}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSystemParam(
        string paramCode,
        [FromBody] UpdateXComSystemParamDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _systemParamService.UpdateSystemParamAsync(paramCode, dto);
            return new object();
        }, $"修改XCOM系統參數失敗: {paramCode}");
    }

    /// <summary>
    /// 刪除XCOM系統參數 (XCOM2A0)
    /// </summary>
    [HttpDelete("system-params/{paramCode}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSystemParam(string paramCode)
    {
        return await ExecuteAsync(async () =>
        {
            await _systemParamService.DeleteSystemParamAsync(paramCode);
            return new object();
        }, $"刪除XCOM系統參數失敗: {paramCode}");
    }

    /// <summary>
    /// 更新XCOM系統參數狀態 (XCOM2A0)
    /// </summary>
    [HttpPut("system-params/{paramCode}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSystemParamStatus(
        string paramCode,
        [FromBody] UpdateStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _systemParamService.UpdateStatusAsync(paramCode, dto.Status);
            return new object();
        }, $"更新XCOM系統參數狀態失敗: {paramCode}");
    }
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}

