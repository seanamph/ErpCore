using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Collections.Generic;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 業務報表列印作業控制器 (SYSL150)
/// </summary>
[Route("api/v1/business-report-print")]
public class BusinessReportPrintController : BaseController
{
    private readonly IBusinessReportPrintService _service;

    public BusinessReportPrintController(
        IBusinessReportPrintService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢業務報表列印列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BusinessReportPrintDto>>>> GetBusinessReportPrints(
        [FromQuery] BusinessReportPrintQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintsAsync(query);
            return result;
        }, "查詢業務報表列印列表失敗");
    }

    /// <summary>
    /// 查詢單筆業務報表列印
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintDto>>> GetBusinessReportPrint(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintByIdAsync(tKey);
            return result;
        }, $"查詢業務報表列印失敗: {tKey}");
    }

    /// <summary>
    /// 新增業務報表列印
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateBusinessReportPrint(
        [FromBody] CreateBusinessReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateBusinessReportPrintAsync(dto);
            return result;
        }, "新增業務報表列印失敗");
    }

    /// <summary>
    /// 修改業務報表列印
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateBusinessReportPrint(
        long tKey,
        [FromBody] UpdateBusinessReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateBusinessReportPrintAsync(tKey, dto);
        }, $"修改業務報表列印失敗: {tKey}");
    }

    /// <summary>
    /// 刪除業務報表列印
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBusinessReportPrint(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteBusinessReportPrintAsync(tKey);
        }, $"刪除業務報表列印失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除業務報表列印
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<int>>> BatchDeleteBusinessReportPrint(
        [FromBody] BatchDeleteDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchDeleteBusinessReportPrintAsync(dto.TKeys);
            return result;
        }, "批次刪除業務報表列印失敗");
    }

    /// <summary>
    /// 批次審核
    /// </summary>
    [HttpPost("batch-audit")]
    public async Task<ActionResult<ApiResponse<BatchAuditResultDto>>> BatchAudit(
        [FromBody] BatchAuditDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchAuditAsync(dto);
            return result;
        }, "批次審核失敗");
    }

    /// <summary>
    /// 複製下一年度資料
    /// </summary>
    [HttpPost("copy-next-year")]
    public async Task<ActionResult<ApiResponse<CopyNextYearResultDto>>> CopyNextYear(
        [FromBody] CopyNextYearDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CopyNextYearAsync(dto);
            return result;
        }, "複製下一年度資料失敗");
    }

    /// <summary>
    /// 計算數量
    /// </summary>
    [HttpPost("calculate-qty")]
    public async Task<ActionResult<ApiResponse<CalculateQtyResultDto>>> CalculateQty(
        [FromBody] CalculateQtyDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CalculateQtyAsync(dto);
            return result;
        }, "計算數量失敗");
    }

    /// <summary>
    /// 檢查年度是否已審核
    /// </summary>
    [HttpGet("check-year-audited")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckYearAudited(
        [FromQuery] int giveYear,
        [FromQuery] string? siteId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.IsYearAuditedAsync(giveYear, siteId);
            return result;
        }, "檢查年度審核狀態失敗");
    }
}
