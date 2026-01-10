using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.IO;

namespace ErpCore.Api.Controllers.Common;

/// <summary>
/// 通用下載控制器
/// 提供統一的檔案下載功能
/// </summary>
[Route("api/v1/common/download")]
public class DownloadController : BaseController
{
    private readonly IWebHostEnvironment _environment;

    public DownloadController(
        IWebHostEnvironment environment,
        ILoggerService logger) : base(logger)
    {
        _environment = environment;
    }

    /// <summary>
    /// 下載檔案
    /// </summary>
    [HttpGet("{filePath}")]
    public async Task<ActionResult> DownloadFile(string filePath)
    {
        try
        {
            // 安全檢查：防止路徑遍歷攻擊
            var decodedPath = Uri.UnescapeDataString(filePath);
            var fullPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath ?? "", "uploads", decodedPath);

            // 確保檔案路徑在允許的目錄內
            var uploadsRoot = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath ?? "", "uploads");
            if (!Path.GetFullPath(fullPath).StartsWith(Path.GetFullPath(uploadsRoot)))
            {
                return BadRequest(ApiResponse<object>.Fail("無效的檔案路徑"));
            }

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(ApiResponse<object>.Fail("檔案不存在"));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var fileName = Path.GetFileName(fullPath);
            var contentType = GetContentType(fileName);

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載檔案失敗: {filePath}", ex);
            return BadRequest(ApiResponse<object>.Fail("下載檔案失敗: " + ex.Message));
        }
    }

    /// <summary>
    /// 下載報表檔案
    /// </summary>
    [HttpGet("reports/{filePath}")]
    public async Task<ActionResult> DownloadReport(string filePath)
    {
        try
        {
            var decodedPath = Uri.UnescapeDataString(filePath);
            var fullPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath ?? "", "downloads", "reports", decodedPath);

            var reportsRoot = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath ?? "", "downloads", "reports");
            if (!Path.GetFullPath(fullPath).StartsWith(Path.GetFullPath(reportsRoot)))
            {
                return BadRequest(ApiResponse<object>.Fail("無效的檔案路徑"));
            }

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(ApiResponse<object>.Fail("檔案不存在"));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var fileName = Path.GetFileName(fullPath);
            var contentType = GetContentType(fileName);

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載報表檔案失敗: {filePath}", ex);
            return BadRequest(ApiResponse<object>.Fail("下載報表檔案失敗: " + ex.Message));
        }
    }

    /// <summary>
    /// 取得檔案內容類型
    /// </summary>
    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".csv" => "text/csv",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}

