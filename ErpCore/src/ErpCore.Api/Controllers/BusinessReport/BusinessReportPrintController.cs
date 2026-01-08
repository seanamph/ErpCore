using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

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
    /// 根據主鍵查詢單筆資料
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintDto>>> GetBusinessReportPrintById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintByIdAsync(tKey);
            if (result == null)
            {
                throw new Exception($"找不到業務報表列印資料: {tKey}");
            }
            return result;
        }, "查詢業務報表列印失敗");
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
            var tKey = await _service.CreateBusinessReportPrintAsync(dto);
            return tKey;
        }, "新增業務報表列印失敗");
    }

    /// <summary>
    /// 修改業務報表列印
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateBusinessReportPrint(
        long tKey,
        [FromBody] UpdateBusinessReportPrintDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UpdateBusinessReportPrintAsync(tKey, dto);
            return result;
        }, "修改業務報表列印失敗");
    }

    /// <summary>
    /// 刪除業務報表列印
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBusinessReportPrint(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.DeleteBusinessReportPrintAsync(tKey);
            return result;
        }, "刪除業務報表列印失敗");
    }

    /// <summary>
    /// 批次審核
    /// </summary>
    [HttpPost("batch-audit")]
    public async Task<ActionResult<ApiResponse<BatchAuditResultDto>>> BatchAudit(
        [FromBody] BatchAuditBusinessReportPrintDto dto)
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
    public async Task<ActionResult<ApiResponse<CopyResultDto>>> CopyNextYear(
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
}

