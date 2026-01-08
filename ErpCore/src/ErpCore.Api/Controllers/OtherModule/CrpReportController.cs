using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Application.Services.OtherModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.OtherModule;

/// <summary>
/// CRP報表控制器
/// 提供 Crystal Reports 報表生成、下載等功能
/// </summary>
[Route("api/v1/other-module/crp")]
public class CrpReportController : BaseController
{
    private readonly ICrpReportService _service;

    public CrpReportController(
        ICrpReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 取得報表設定列表
    /// </summary>
    [HttpGet("reports")]
    public async Task<ActionResult<ApiResponse<List<CrystalReportDto>>>> GetReports()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportsAsync();
            return result;
        }, "取得報表設定列表失敗");
    }

    /// <summary>
    /// 根據報表代碼取得報表設定
    /// </summary>
    [HttpGet("reports/{reportCode}")]
    public async Task<ActionResult<ApiResponse<CrystalReportDto>>> GetReportByCode(string reportCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportByCodeAsync(reportCode);
            if (result == null)
            {
                throw new InvalidOperationException($"報表設定不存在: {reportCode}");
            }
            return result;
        }, "取得報表設定失敗");
    }

    /// <summary>
    /// 生成報表
    /// </summary>
    [HttpPost("reports/generate")]
    public async Task<ActionResult<ApiResponse<GenerateReportResponseDto>>> GenerateReport([FromBody] GenerateReportRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GenerateReportAsync(request);
            return result;
        }, "生成報表失敗");
    }

    /// <summary>
    /// 下載報表
    /// </summary>
    [HttpGet("reports/{reportId}/download")]
    public async Task<ActionResult> DownloadReport(long reportId)
    {
        try
        {
            var reportBytes = await _service.DownloadReportAsync(reportId);
            if (reportBytes == null)
            {
                return NotFound(ApiResponse<object>.Fail("報表檔案不存在", "FILE_NOT_FOUND"));
            }

            var fileName = $"report_{reportId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(reportBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載報表失敗: {reportId}", ex);
            return BadRequest(ApiResponse<object>.Fail(ex.Message, "DOWNLOAD_ERROR"));
        }
    }

    /// <summary>
    /// 新增報表設定
    /// </summary>
    [HttpPost("reports")]
    public async Task<ActionResult<ApiResponse<long>>> CreateReport([FromBody] CreateCrystalReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateReportAsync(dto);
            return result;
        }, "新增報表設定失敗");
    }

    /// <summary>
    /// 修改報表設定
    /// </summary>
    [HttpPut("reports/{reportId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateReport(long reportId, [FromBody] CreateCrystalReportDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateReportAsync(reportId, dto);
            return (object)null!;
        }, "修改報表設定失敗");
    }

    /// <summary>
    /// 刪除報表設定
    /// </summary>
    [HttpDelete("reports/{reportId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteReport(long reportId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteReportAsync(reportId);
            return (object)null!;
        }, "刪除報表設定失敗");
    }

    /// <summary>
    /// 查詢操作記錄列表
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<CrystalReportLogDto>>>> GetLogs(
        [FromQuery] string? reportCode,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetLogsAsync(reportCode, pageIndex, pageSize);
            return result;
        }, "查詢操作記錄列表失敗");
    }
}

