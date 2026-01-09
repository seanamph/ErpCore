using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.SalesReport;
using ErpCore.Application.Services.SalesReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SalesReport;

/// <summary>
/// 銷售報表模組控制器 (SYS1000 - 銷售報表模組系列)
/// </summary>
[Route("api/v1/sales-reports")]
public class SalesReportModuleController : BaseController
{
    private readonly ISalesReportService _service;

    public SalesReportModuleController(
        ISalesReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢銷售報表列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesReportDto>>>> GetSalesReports(
        [FromQuery] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSalesReportsAsync(query);
            return result;
        }, "查詢銷售報表列表失敗");
    }

    /// <summary>
    /// 查詢單筆銷售報表
    /// </summary>
    [HttpGet("{reportId}")]
    public async Task<ActionResult<ApiResponse<SalesReportDto>>> GetSalesReport(string reportId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSalesReportByIdAsync(reportId);
            if (result == null)
            {
                throw new Exception($"銷售報表不存在: {reportId}");
            }
            return result;
        }, $"查詢銷售報表失敗: {reportId}");
    }

    /// <summary>
    /// 新增銷售報表
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateSalesReport(
        [FromBody] CreateSalesReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateSalesReportAsync(dto);
            return result;
        }, "新增銷售報表失敗");
    }

    /// <summary>
    /// 修改銷售報表
    /// </summary>
    [HttpPut("{reportId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSalesReport(
        string reportId,
        [FromBody] UpdateSalesReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSalesReportAsync(reportId, dto);
            return new { Message = "修改成功" };
        }, $"修改銷售報表失敗: {reportId}");
    }

    /// <summary>
    /// 刪除銷售報表
    /// </summary>
    [HttpDelete("{reportId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSalesReport(string reportId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSalesReportAsync(reportId);
            return new { Message = "刪除成功" };
        }, $"刪除銷售報表失敗: {reportId}");
    }

    /// <summary>
    /// 生成報表
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<GenerateReportResponseDto>>> GenerateReport(
        [FromBody] GenerateReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GenerateReportAsync(dto);
            return result;
        }, "生成報表失敗");
    }

    /// <summary>
    /// 下載報表
    /// </summary>
    [HttpGet("{reportId}/download")]
    public async Task<IActionResult> DownloadReport(string reportId, [FromQuery] string format = "excel")
    {
        try
        {
            var data = await _service.DownloadReportAsync(reportId, format);
            var contentType = format.ToLower() switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pdf" => "application/pdf",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };
            var fileName = $"report_{reportId}.{format}";
            return File(data, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載報表失敗: {reportId}", ex);
            return BadRequest(ApiResponse<object>.Fail($"下載報表失敗: {ex.Message}"));
        }
    }
}

