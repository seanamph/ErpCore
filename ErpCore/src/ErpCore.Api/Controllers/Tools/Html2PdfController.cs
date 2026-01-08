using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Tools;
using ErpCore.Application.Services.Tools;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Tools;

/// <summary>
/// HTML轉PDF工具控制器 (RslHtml2Pdf)
/// </summary>
[Route("api/v1/pdf")]
public class Html2PdfController : BaseController
{
    private readonly IHtml2PdfService _service;

    public Html2PdfController(
        IHtml2PdfService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 將HTML內容轉換為PDF
    /// </summary>
    [HttpPost("convert")]
    public async Task<ActionResult<ApiResponse<Html2PdfResponseDto>>> ConvertHtmlToPdf([FromBody] Html2PdfRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ConvertHtmlToPdfAsync(request);
            return result;
        }, "轉換HTML為PDF失敗");
    }

    /// <summary>
    /// 下載PDF檔案
    /// </summary>
    [HttpGet("{logId}/download")]
    public async Task<IActionResult> DownloadPdf(Guid logId)
    {
        try
        {
            var log = await _service.GetConversionLogAsync(logId);
            if (log == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Code = 404,
                    Message = "PDF轉換記錄不存在"
                });
            }

            var fileBytes = await _service.DownloadPdfAsync(logId);
            if (fileBytes == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Code = 404,
                    Message = "PDF檔案不存在"
                });
            }

            return File(fileBytes, "application/pdf", log.FileName ?? "document.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載PDF失敗: {logId}", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = $"下載PDF失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 查詢PDF轉換記錄
    /// </summary>
    [HttpGet("{logId}")]
    public async Task<ActionResult<ApiResponse<PdfConversionLogDto>>> GetConversionLog(Guid logId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetConversionLogAsync(logId);
            if (result == null)
            {
                throw new Exception($"PDF轉換記錄不存在: {logId}");
            }
            return result;
        }, $"查詢PDF轉換記錄失敗: {logId}");
    }
}

