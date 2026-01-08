using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 業務報表列印明細作業控制器 (SYSL160)
/// </summary>
[Route("api/v1/business-report-print-details")]
public class BusinessReportPrintDetailController : BaseController
{
    private readonly IBusinessReportPrintDetailService _service;

    public BusinessReportPrintDetailController(
        IBusinessReportPrintDetailService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢業務報表列印明細列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BusinessReportPrintDetailDto>>>> GetBusinessReportPrintDetails(
        [FromQuery] BusinessReportPrintDetailQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintDetailsAsync(query);
            return result;
        }, "查詢業務報表列印明細列表失敗");
    }

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<BusinessReportPrintDetailDto>>> GetBusinessReportPrintDetailById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintDetailByIdAsync(tKey);
            if (result == null)
            {
                throw new Exception($"找不到業務報表列印明細資料: {tKey}");
            }
            return result;
        }, "查詢業務報表列印明細失敗");
    }

    /// <summary>
    /// 根據 PrintId 查詢明細列表
    /// </summary>
    [HttpGet("print/{printId}")]
    public async Task<ActionResult<ApiResponse<List<BusinessReportPrintDetailDto>>>> GetBusinessReportPrintDetailsByPrintId(long printId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportPrintDetailsByPrintIdAsync(printId);
            return result;
        }, "查詢業務報表列印明細失敗");
    }

    /// <summary>
    /// 新增業務報表列印明細
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateBusinessReportPrintDetail(
        [FromBody] CreateBusinessReportPrintDetailDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var tKey = await _service.CreateBusinessReportPrintDetailAsync(dto);
            return tKey;
        }, "新增業務報表列印明細失敗");
    }

    /// <summary>
    /// 修改業務報表列印明細
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateBusinessReportPrintDetail(
        long tKey,
        [FromBody] UpdateBusinessReportPrintDetailDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UpdateBusinessReportPrintDetailAsync(tKey, dto);
            return result;
        }, "修改業務報表列印明細失敗");
    }

    /// <summary>
    /// 刪除業務報表列印明細
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBusinessReportPrintDetail(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.DeleteBusinessReportPrintDetailAsync(tKey);
            return result;
        }, "刪除業務報表列印明細失敗");
    }

    /// <summary>
    /// 批次處理業務報表列印明細
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<ApiResponse<BatchProcessBusinessReportPrintDetailResultDto>>> BatchProcess(
        [FromBody] BatchProcessBusinessReportPrintDetailDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchProcessAsync(dto);
            return result;
        }, "批次處理業務報表列印明細失敗");
    }
}

