using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.TaxAccounting;

/// <summary>
/// 稅務報表查詢控制器 (SYST411-SYST452)
/// 提供傳票列印、財務報表查詢、稅務統計報表等功能
/// </summary>
[Route("api/v1/tax-accounting/tax-reports")]
public class TaxReportController : BaseController
{
    private readonly ITaxReportService _service;

    public TaxReportController(
        ITaxReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢傳票列表
    /// </summary>
    [HttpGet("vouchers")]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceVoucherDto>>>> GetVouchers(
        [FromQuery] TaxReportVoucherQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetVouchersAsync(query);
            return result;
        }, "查詢傳票列表失敗");
    }

    /// <summary>
    /// 查詢傳票明細
    /// </summary>
    [HttpGet("vouchers/{voucherId}/details")]
    public async Task<ActionResult<ApiResponse<List<VoucherDetailDto>>>> GetVoucherDetails(string voucherId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetVoucherDetailsAsync(voucherId);
            return result;
        }, $"查詢傳票明細失敗: {voucherId}");
    }

    /// <summary>
    /// 查詢財務報表
    /// </summary>
    [HttpGet("financial-reports")]
    public async Task<ActionResult<ApiResponse<object>>> GetFinancialReports(
        [FromQuery] FinancialReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFinancialReportsAsync(query);
            return result;
        }, "查詢財務報表失敗");
    }

    /// <summary>
    /// 查詢稅務統計報表
    /// </summary>
    [HttpGet("tax-statistics")]
    public async Task<ActionResult<ApiResponse<object>>> GetTaxStatistics(
        [FromQuery] TaxStatisticsQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTaxStatisticsAsync(query);
            return result;
        }, "查詢稅務統計報表失敗");
    }

    /// <summary>
    /// 列印傳票
    /// </summary>
    [HttpPost("vouchers/print")]
    public async Task<ActionResult<ApiResponse<PrintResultDto>>> PrintVouchers(
        [FromBody] PrintVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.PrintVouchersAsync(dto);
            return result;
        }, "列印傳票失敗");
    }
}

