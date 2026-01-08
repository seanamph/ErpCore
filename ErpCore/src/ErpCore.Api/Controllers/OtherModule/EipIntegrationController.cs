using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Application.Services.OtherModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.OtherModule;

/// <summary>
/// EIP整合控制器
/// 提供 EIP 系統整合功能
/// </summary>
[Route("api/v1/other-module/eip")]
public class EipIntegrationController : BaseController
{
    private readonly IEipIntegrationService _service;

    public EipIntegrationController(
        IEipIntegrationService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢EIP整合設定列表
    /// </summary>
    [HttpGet("integrations")]
    public async Task<ActionResult<ApiResponse<PagedResult<EipIntegrationDto>>>> GetIntegrations([FromQuery] EipIntegrationQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetIntegrationsAsync(query);
            return result;
        }, "查詢EIP整合設定列表失敗");
    }

    /// <summary>
    /// 根據作業編號和頁面代碼取得整合設定
    /// </summary>
    [HttpGet("integrations/{progId}/{pageId}")]
    public async Task<ActionResult<ApiResponse<EipIntegrationDto>>> GetIntegration(string progId, string pageId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetIntegrationAsync(progId, pageId);
            if (result == null)
            {
                throw new InvalidOperationException($"EIP整合設定不存在: {progId}/{pageId}");
            }
            return result;
        }, "取得EIP整合設定失敗");
    }

    /// <summary>
    /// EIP單一登入
    /// </summary>
    [HttpGet("sso")]
    public async Task<ActionResult<ApiResponse<string>>> GetSsoUrl()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSsoUrlAsync();
            return result;
        }, "取得EIP單一登入URL失敗");
    }

    /// <summary>
    /// 傳送表單至EIP
    /// </summary>
    [HttpPost("send-form")]
    public async Task<ActionResult<ApiResponse<EipTransactionDto>>> SendFormToEip([FromBody] SendFormToEipRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SendFormToEipAsync(request);
            return result;
        }, "傳送表單至EIP失敗");
    }

    /// <summary>
    /// 新增整合設定
    /// </summary>
    [HttpPost("integrations")]
    public async Task<ActionResult<ApiResponse<long>>> CreateIntegration([FromBody] CreateEipIntegrationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateIntegrationAsync(dto);
            return result;
        }, "新增EIP整合設定失敗");
    }

    /// <summary>
    /// 修改整合設定
    /// </summary>
    [HttpPut("integrations/{integrationId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateIntegration(long integrationId, [FromBody] CreateEipIntegrationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateIntegrationAsync(integrationId, dto);
            return (object)null!;
        }, "修改EIP整合設定失敗");
    }

    /// <summary>
    /// 刪除整合設定
    /// </summary>
    [HttpDelete("integrations/{integrationId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteIntegration(long integrationId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteIntegrationAsync(integrationId);
            return (object)null!;
        }, "刪除EIP整合設定失敗");
    }

    /// <summary>
    /// 查詢交易記錄列表
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<EipTransactionDto>>>> GetTransactions(
        [FromQuery] string? progId,
        [FromQuery] string? pageId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransactionsAsync(progId, pageId, pageIndex, pageSize);
            return result;
        }, "查詢EIP交易記錄列表失敗");
    }
}

