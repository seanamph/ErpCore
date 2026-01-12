using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.AnalysisReport;

/// <summary>
/// 分析報表控制器 (SYSA1000)
/// </summary>
[Route("api/v1/analysis-reports")]
public class AnalysisReportsController : BaseController
{
    private readonly IAnalysisReportService _service;

    public AnalysisReportsController(
        IAnalysisReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢商品分析報表 (SYSA1011)
    /// </summary>
    [HttpGet("sysa1011")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1011ReportDto>>>> GetSYSA1011Report(
        [FromQuery] SYSA1011QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1011ReportAsync(query);
            return result;
        }, "查詢商品分析報表失敗");
    }

    /// <summary>
    /// 匯出商品分析報表 (SYSA1011)
    /// </summary>
    [HttpPost("sysa1011/export")]
    public async Task<IActionResult> ExportSYSA1011Report(
        [FromBody] SYSA1011QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1011ReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 列印商品分析報表 (SYSA1011)
    /// </summary>
    [HttpPost("sysa1011/print")]
    public async Task<IActionResult> PrintSYSA1011Report([FromBody] SYSA1011QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1011ReportAsync(query);
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 查詢商品分類列表 (SYSA1011)
    /// </summary>
    [HttpGet("sysa1011/categories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<GoodsCategoryDto>>>> GetGoodsCategories(
        [FromQuery] string categoryType,
        [FromQuery] string? parentId = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetGoodsCategoriesAsync(categoryType, parentId);
            return result;
        }, "查詢商品分類列表失敗");
    }

    /// <summary>
    /// 查詢進銷存月報表 (SYSA1012)
    /// </summary>
    [HttpGet("sysa1012")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1012ReportDto>>>> GetSYSA1012Report(
        [FromQuery] SYSA1012QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1012ReportAsync(query);
            return result;
        }, "查詢進銷存月報表失敗");
    }

    /// <summary>
    /// 匯出進銷存月報表 (SYSA1012)
    /// </summary>
    [HttpPost("sysa1012/export")]
    public async Task<IActionResult> ExportSYSA1012Report(
        [FromBody] SYSA1012QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1012ReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"進銷存月報表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出進銷存月報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出進銷存月報表失敗" });
        }
    }

    /// <summary>
    /// 列印進銷存月報表 (SYSA1012)
    /// </summary>
    [HttpPost("sysa1012/print")]
    public async Task<IActionResult> PrintSYSA1012Report([FromBody] SYSA1012QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1012ReportAsync(query);
            var fileName = $"進銷存月報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印進銷存月報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印進銷存月報表失敗" });
        }
    }

    /// <summary>
    /// 查詢耗材出庫明細表 (SYSA1013)
    /// </summary>
    [HttpGet("sysa1013")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1013ReportDto>>>> GetSYSA1013Report(
        [FromQuery] SYSA1013QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1013ReportAsync(query);
            return result;
        }, "查詢耗材出庫明細表失敗");
    }

    /// <summary>
    /// 匯出耗材出庫明細表 (SYSA1013)
    /// </summary>
    [HttpPost("sysa1013/export")]
    public async Task<IActionResult> ExportSYSA1013Report(
        [FromBody] SYSA1013QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1013ReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"耗材出庫明細表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出耗材出庫明細表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出耗材出庫明細表失敗" });
        }
    }

    /// <summary>
    /// 列印耗材出庫明細表 (SYSA1013)
    /// </summary>
    [HttpPost("sysa1013/print")]
    public async Task<IActionResult> PrintSYSA1013Report([FromBody] SYSA1013QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1013ReportAsync(query);
            var fileName = $"耗材出庫明細表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印耗材出庫明細表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印耗材出庫明細表失敗" });
        }
    }

    /// <summary>
    /// 查詢商品分析報表 (SYSA1014)
    /// </summary>
    [HttpGet("sysa1014")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1014ReportDto>>>> GetSYSA1014Report(
        [FromQuery] SYSA1014QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1014ReportAsync(query);
            return result;
        }, "查詢商品分析報表失敗");
    }

    /// <summary>
    /// 匯出商品分析報表 (SYSA1014)
    /// </summary>
    [HttpPost("sysa1014/export")]
    public async Task<IActionResult> ExportSYSA1014Report(
        [FromBody] SYSA1014QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1014ReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 列印商品分析報表 (SYSA1014)
    /// </summary>
    [HttpPost("sysa1014/print")]
    public async Task<IActionResult> PrintSYSA1014Report([FromBody] SYSA1014QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1014ReportAsync(query);
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 查詢商品分析報表 (SYSA1015)
    /// </summary>
    [HttpGet("sysa1015")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1015ReportDto>>>> GetSYSA1015Report(
        [FromQuery] SYSA1015QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1015ReportAsync(query);
            return result;
        }, "查詢商品分析報表失敗");
    }

    /// <summary>
    /// 匯出商品分析報表 (SYSA1015)
    /// </summary>
    [HttpPost("sysa1015/export")]
    public async Task<IActionResult> ExportSYSA1015Report(
        [FromBody] SYSA1015QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1015ReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 列印商品分析報表 (SYSA1015)
    /// </summary>
    [HttpPost("sysa1015/print")]
    public async Task<IActionResult> PrintSYSA1015Report([FromBody] SYSA1015QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1015ReportAsync(query);
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 查詢商品分析報表 (SYSA1016)
    /// </summary>
    [HttpGet("sysa1016")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1016ReportDto>>>> GetSYSA1016Report(
        [FromQuery] SYSA1016QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1016ReportAsync(query);
            return result;
        }, "查詢商品分析報表失敗");
    }

    /// <summary>
    /// 匯出商品分析報表 (SYSA1016)
    /// </summary>
    [HttpPost("sysa1016/export")]
    public async Task<IActionResult> ExportSYSA1016Report(
        [FromBody] SYSA1016QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1016ReportAsync(query, format);
            var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 列印商品分析報表 (SYSA1016)
    /// </summary>
    [HttpPost("sysa1016/print")]
    public async Task<IActionResult> PrintSYSA1016Report([FromBody] SYSA1016QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1016ReportAsync(query);
            var fileName = $"商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 查詢商品分析報表 (SYSA1017)
    /// </summary>
    [HttpGet("sysa1017")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1017ReportDto>>>> GetSYSA1017Report(
        [FromQuery] SYSA1017QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1017ReportAsync(query);
            return result;
        }, "查詢商品分析報表失敗");
    }

    /// <summary>
    /// 匯出商品分析報表 (SYSA1017)
    /// </summary>
    [HttpPost("sysa1017/export")]
    public async Task<IActionResult> ExportSYSA1017Report(
        [FromBody] SYSA1017QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1017ReportAsync(query, format);
            var fileName = $"SYSA1017_商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(format == "excel" ? "xlsx" : "pdf")}";
            return File(fileBytes, format == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 列印商品分析報表 (SYSA1017)
    /// </summary>
    [HttpPost("sysa1017/print")]
    public async Task<IActionResult> PrintSYSA1017Report([FromBody] SYSA1017QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1017ReportAsync(query);
            var fileName = $"SYSA1017_商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 查詢工務維修件數統計報表 (SYSA1018)
    /// </summary>
    [HttpGet("sysa1018")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1018ReportDto>>>> GetSYSA1018Report(
        [FromQuery] SYSA1018QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1018ReportAsync(query);
            return result;
        }, "查詢工務維修件數統計報表失敗");
    }

    /// <summary>
    /// 匯出工務維修件數統計報表 (SYSA1018)
    /// </summary>
    [HttpPost("sysa1018/export")]
    public async Task<IActionResult> ExportSYSA1018Report(
        [FromBody] SYSA1018QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1018ReportAsync(query, format);
            var fileName = $"SYSA1018_工務維修件數統計表_{DateTime.Now:yyyyMMddHHmmss}.{(format == "excel" ? "xlsx" : "pdf")}";
            return File(fileBytes, format == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出工務維修件數統計報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出工務維修件數統計報表失敗" });
        }
    }

    /// <summary>
    /// 列印工務維修件數統計報表 (SYSA1018)
    /// </summary>
    [HttpPost("sysa1018/print")]
    public async Task<IActionResult> PrintSYSA1018Report([FromBody] SYSA1018QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1018ReportAsync(query);
            var fileName = $"SYSA1018_工務維修件數統計表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印工務維修件數統計報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印工務維修件數統計報表失敗" });
        }
    }

    /// <summary>
    /// 查詢商品分析報表 (SYSA1019)
    /// </summary>
    [HttpGet("sysa1019")]
    public async Task<ActionResult<ApiResponse<PagedResult<SYSA1019ReportDto>>>> GetSYSA1019Report(
        [FromQuery] SYSA1019QueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSYSA1019ReportAsync(query);
            return result;
        }, "查詢商品分析報表失敗");
    }

    /// <summary>
    /// 匯出商品分析報表 (SYSA1019)
    /// </summary>
    [HttpPost("sysa1019/export")]
    public async Task<IActionResult> ExportSYSA1019Report(
        [FromBody] SYSA1019QueryDto query,
        [FromQuery] string format = "excel")
    {
        try
        {
            var fileBytes = await _service.ExportSYSA1019ReportAsync(query, format);
            var fileName = $"SYSA1019_商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(format == "excel" ? "xlsx" : "pdf")}";
            return File(fileBytes, format == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "匯出商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 列印商品分析報表 (SYSA1019)
    /// </summary>
    [HttpPost("sysa1019/print")]
    public async Task<IActionResult> PrintSYSA1019Report([FromBody] SYSA1019QueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintSYSA1019ReportAsync(query);
            var fileName = $"SYSA1019_商品分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            return BadRequest(new ApiResponse { Success = false, Message = "列印商品分析報表失敗" });
        }
    }

    /// <summary>
    /// 查詢進銷存分析報表 (SYSA1000) - 通用查詢方法
    /// </summary>
    [HttpGet("{reportId}")]
    public async Task<ActionResult<ApiResponse<PagedResult<Dictionary<string, object>>>>> GetAnalysisReport(
        string reportId,
        [FromQuery] AnalysisReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAnalysisReportAsync(reportId, query);
            return result;
        }, $"查詢進銷存分析報表失敗: {reportId}");
    }

    /// <summary>
    /// 匯出進銷存分析報表 (SYSA1000)
    /// </summary>
    [HttpPost("{reportId}/export")]
    public async Task<IActionResult> ExportAnalysisReport(
        string reportId,
        [FromBody] ExportReportDto dto,
        [FromQuery] string? format = null)
    {
        try
        {
            var exportFormat = format ?? dto.Format;
            var fileBytes = await _service.ExportAnalysisReportAsync(reportId, dto.QueryParams, exportFormat);
            var contentType = exportFormat.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"{reportId}_進銷存分析報表_{DateTime.Now:yyyyMMddHHmmss}.{(exportFormat.ToLower() == "pdf" ? "pdf" : "xlsx")}";
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出進銷存分析報表失敗: {reportId}", ex);
            return BadRequest(new ApiResponse { Success = false, Message = $"匯出進銷存分析報表失敗: {reportId}" });
        }
    }

    /// <summary>
    /// 列印進銷存分析報表 (SYSA1000)
    /// </summary>
    [HttpPost("{reportId}/print")]
    public async Task<IActionResult> PrintAnalysisReport(
        string reportId,
        [FromBody] AnalysisReportQueryDto query)
    {
        try
        {
            var fileBytes = await _service.PrintAnalysisReportAsync(reportId, query);
            var fileName = $"{reportId}_進銷存分析報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印進銷存分析報表失敗: {reportId}", ex);
            return BadRequest(new ApiResponse { Success = false, Message = $"列印進銷存分析報表失敗: {reportId}" });
        }
    }
}
