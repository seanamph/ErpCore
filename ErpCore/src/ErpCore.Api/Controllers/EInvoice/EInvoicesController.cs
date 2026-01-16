using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.EInvoice;
using ErpCore.Application.Services.EInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.EInvoice;

/// <summary>
/// 電子發票處理作業控制器 (ECA3010)
/// </summary>
[Route("api/v1/einvoices")]
public class EInvoicesController : BaseController
{
    private readonly IEInvoiceService _service;

    public EInvoicesController(
        IEInvoiceService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 上傳電子發票檔案
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<EInvoiceUploadDto>>> UploadFile(
        IFormFile file,
        [FromForm] string? storeId = null,
        [FromForm] string? retailerId = null,
        [FromForm] string? uploadType = null)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.UploadFileAsync(file, storeId, retailerId, uploadType);
            return result;
        }, "上傳電子發票檔案失敗");
    }

    /// <summary>
    /// 查詢上傳記錄列表
    /// </summary>
    [HttpGet("uploads")]
    public async Task<ActionResult<ApiResponse<PagedResult<EInvoiceUploadDto>>>> GetUploads(
        [FromQuery] EInvoiceUploadQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUploadsAsync(query);
            return result;
        }, "查詢上傳記錄列表失敗");
    }

    /// <summary>
    /// 查詢單筆上傳記錄
    /// </summary>
    [HttpGet("uploads/{uploadId}")]
    public async Task<ActionResult<ApiResponse<EInvoiceUploadDto>>> GetUpload(long uploadId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUploadAsync(uploadId);
            return result;
        }, $"查詢上傳記錄失敗: {uploadId}");
    }

    /// <summary>
    /// 查詢處理狀態
    /// </summary>
    [HttpGet("uploads/{uploadId}/status")]
    public async Task<ActionResult<ApiResponse<EInvoiceProcessStatusDto>>> GetProcessStatus(long uploadId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetProcessStatusAsync(uploadId);
            return result;
        }, $"查詢處理狀態失敗: {uploadId}");
    }

    /// <summary>
    /// 開始處理上傳檔案
    /// </summary>
    [HttpPost("uploads/{uploadId}/process")]
    public async Task<ActionResult<ApiResponse<object>>> StartProcess(long uploadId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.StartProcessAsync(uploadId);
        }, $"開始處理上傳檔案失敗: {uploadId}");
    }

    /// <summary>
    /// 查詢電子發票列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EInvoiceDto>>>> GetEInvoices(
        [FromQuery] EInvoiceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEInvoicesAsync(query);
            return result;
        }, "查詢電子發票列表失敗");
    }

    /// <summary>
    /// 查詢單筆電子發票
    /// </summary>
    [HttpGet("{invoiceId}")]
    public async Task<ActionResult<ApiResponse<EInvoiceDto>>> GetEInvoice(long invoiceId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEInvoiceAsync(invoiceId);
            return result;
        }, $"查詢電子發票失敗: {invoiceId}");
    }

    /// <summary>
    /// 匯出電子發票查詢結果到 Excel (ECA3020)
    /// </summary>
    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportEInvoicesToExcel(
        [FromBody] EInvoiceQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportEInvoicesToExcelAsync(query);
            var fileName = $"電子發票查詢_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出電子發票查詢結果到 Excel 失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出電子發票查詢結果到 Excel 失敗"));
        }
    }

    /// <summary>
    /// 匯出電子發票查詢結果到 PDF (ECA3020)
    /// </summary>
    [HttpPost("export/pdf")]
    public async Task<IActionResult> ExportEInvoicesToPdf(
        [FromBody] EInvoiceQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportEInvoicesToPdfAsync(query);
            var fileName = $"電子發票查詢_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出電子發票查詢結果到 PDF 失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出電子發票查詢結果到 PDF 失敗"));
        }
    }

    /// <summary>
    /// 刪除上傳記錄
    /// </summary>
    [HttpDelete("uploads/{uploadId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUpload(long uploadId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteUploadAsync(uploadId);
        }, $"刪除上傳記錄失敗: {uploadId}");
    }

    /// <summary>
    /// 下載上傳檔案或處理結果 (ECA2050, ECA3010)
    /// </summary>
    [HttpGet("uploads/{uploadId}/download")]
    public async Task<IActionResult> DownloadUpload(long uploadId, [FromQuery] string? type = null)
    {
        try
        {
            // 如果有 type 參數，則下載處理結果（成功/失敗清單）
            if (!string.IsNullOrEmpty(type))
            {
                var fileBytes = await _service.DownloadResultAsync(uploadId, type);
                var typeName = type.ToLower() switch
                {
                    "success" => "成功清單",
                    "failed" => "失敗清單",
                    "all" => "全部清單",
                    _ => "處理結果"
                };
                var fileName = $"電子發票處理結果_{typeName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                // 否則下載上傳的原始檔案
                var (fileBytes, fileName, contentType) = await _service.DownloadUploadFileAsync(uploadId);
                return File(fileBytes, contentType, fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載失敗: {uploadId}, Type: {type}", ex);
            return BadRequest(ApiResponse<object>.Fail($"下載失敗: {ex.Message}"));
        }
    }

    /// <summary>
    /// 查詢電子發票報表 (ECA3040, ECA4010-ECA4060)
    /// </summary>
    [HttpPost("reports")]
    public async Task<ActionResult<ApiResponse<PagedResult<EInvoiceReportDto>>>> GetEInvoiceReports(
        [FromBody] EInvoiceReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetEInvoiceReportsAsync(query);
            return result;
        }, "查詢電子發票報表失敗");
    }

    /// <summary>
    /// 匯出電子發票報表到 Excel (ECA3040, ECA4010-ECA4060)
    /// </summary>
    [HttpPost("reports/export/excel")]
    public async Task<IActionResult> ExportEInvoiceReportsToExcel(
        [FromBody] EInvoiceReportQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportEInvoiceReportsToExcelAsync(query);
            var reportTypeName = GetReportTypeName(query.ReportType);
            var fileName = $"電子發票報表_{reportTypeName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出電子發票報表到 Excel 失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出電子發票報表到 Excel 失敗"));
        }
    }

    /// <summary>
    /// 匯出電子發票報表到 PDF (ECA3040, ECA4010-ECA4060)
    /// </summary>
    [HttpPost("reports/export/pdf")]
    public async Task<IActionResult> ExportEInvoiceReportsToPdf(
        [FromBody] EInvoiceReportQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportEInvoiceReportsToPdfAsync(query);
            var reportTypeName = GetReportTypeName(query.ReportType);
            var fileName = $"電子發票報表_{reportTypeName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出電子發票報表到 PDF 失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出電子發票報表到 PDF 失敗"));
        }
    }

    /// <summary>
    /// 列印電子發票報表 (ECA3040)
    /// </summary>
    [HttpPost("reports/print")]
    public async Task<IActionResult> PrintEInvoiceReports(
        [FromBody] EInvoiceReportQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportEInvoiceReportsToPdfAsync(query);
            var reportTypeName = GetReportTypeName(query.ReportType);
            var fileName = $"電子發票報表_{reportTypeName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            // 打印功能返回PDF，前端会在新窗口打开
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印電子發票報表失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("列印電子發票報表失敗"));
        }
    }

    /// <summary>
    /// 取得報表類型名稱
    /// </summary>
    private string GetReportTypeName(string? reportType)
    {
        return reportType switch
        {
            "ECA3040" => "電子發票報表查詢",
            "ECA4010" => "訂單明細",
            "ECA4020" => "商品銷售統計",
            "ECA4030" => "零售商銷售統計",
            "ECA4040" => "店別銷售統計",
            "ECA4050" => "出貨日期統計",
            "ECA4060" => "訂單日期統計",
            _ => "電子發票報表"
        };
    }
}

