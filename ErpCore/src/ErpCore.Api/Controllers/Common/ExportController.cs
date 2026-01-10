using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Text;

namespace ErpCore.Api.Controllers.Common;

/// <summary>
/// 通用匯出控制器
/// 提供統一的資料匯出功能（Excel、PDF、CSV等）
/// </summary>
[Route("api/v1/common/export")]
public class ExportController : BaseController
{
    private readonly ExportHelper _exportHelper;

    public ExportController(
        ILoggerService logger) : base(logger)
    {
        _exportHelper = new ExportHelper(logger);
    }

    /// <summary>
    /// 匯出資料到 Excel
    /// </summary>
    [HttpPost("excel")]
    public async Task<ActionResult> ExportToExcel(
        [FromBody] ExportRequestDto request)
    {
        try
        {
            var excelBytes = _exportHelper.ExportToExcel(
                request.Data,
                request.Columns,
                request.SheetName ?? "Sheet1",
                request.Title);

            var fileName = $"{request.FileName ?? "Export"}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Excel失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出Excel失敗: " + ex.Message));
        }
    }

    /// <summary>
    /// 匯出資料到 PDF
    /// </summary>
    [HttpPost("pdf")]
    public async Task<ActionResult> ExportToPdf(
        [FromBody] ExportRequestDto request)
    {
        try
        {
            var pdfBytes = _exportHelper.ExportToPdf(
                request.Data,
                request.Columns,
                request.Title);

            var fileName = $"{request.FileName ?? "Export"}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出PDF失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出PDF失敗: " + ex.Message));
        }
    }

    /// <summary>
    /// 匯出資料到 CSV
    /// </summary>
    [HttpPost("csv")]
    public async Task<ActionResult> ExportToCsv(
        [FromBody] ExportRequestDto request)
    {
        try
        {
            var csvContent = _exportHelper.ExportToCsv(
                request.Data,
                request.Columns);

            var csvBytes = Encoding.UTF8.GetBytes(csvContent);
            var fileName = $"{request.FileName ?? "Export"}_{DateTime.Now:yyyyMMddHHmmss}.csv";
            return File(csvBytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出CSV失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出CSV失敗: " + ex.Message));
        }
    }
}

/// <summary>
/// 匯出請求DTO
/// </summary>
public class ExportRequestDto
{
    /// <summary>
    /// 資料列表
    /// </summary>
    public IEnumerable<Dictionary<string, object>> Data { get; set; } = new List<Dictionary<string, object>>();

    /// <summary>
    /// 欄位定義
    /// </summary>
    public List<ExportColumn> Columns { get; set; } = new();

    /// <summary>
    /// 工作表名稱（Excel用）
    /// </summary>
    public string? SheetName { get; set; }

    /// <summary>
    /// 報表標題
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 檔案名稱（不含副檔名）
    /// </summary>
    public string? FileName { get; set; }
}

