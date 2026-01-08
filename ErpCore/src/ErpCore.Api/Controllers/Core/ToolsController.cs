using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Core;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Core;

/// <summary>
/// 工具控制器
/// 提供 Excel匯出、字串編碼、ASP轉ASPX 等功能
/// </summary>
[Route("api/v1/core/tools")]
public class ToolsController : BaseController
{
    private readonly IToolsService _service;

    public ToolsController(
        IToolsService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region Export_Excel - Excel匯出功能

    /// <summary>
    /// 取得匯出設定列表
    /// </summary>
    [HttpGet("excel-export/{moduleCode}/config")]
    public async Task<ActionResult<ApiResponse<List<ExcelExportConfigDto>>>> GetExportConfigs(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetExportConfigsAsync(moduleCode);
            return result;
        }, "取得匯出設定列表失敗");
    }

    /// <summary>
    /// 匯出Excel
    /// </summary>
    [HttpPost("excel-export/{moduleCode}/export")]
    public async Task<ActionResult> ExportExcel(string moduleCode, [FromBody] ExcelExportRequestDto request)
    {
        try
        {
            request.ModuleCode = moduleCode;
            
            if (request.Async)
            {
                // 背景匯出
                var taskId = await _service.CreateExportTaskAsync(request);
                return Ok(ApiResponse<ExcelExportTaskDto>.Ok(new ExcelExportTaskDto
                {
                    TaskId = taskId,
                    Status = "PENDING",
                    CreatedAt = DateTime.Now
                }, "匯出任務已建立"));
            }
            else
            {
                // 同步匯出
                var excelBytes = await _service.ExportExcelAsync(request);
                var fileName = $"export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Excel失敗", ex);
            return BadRequest(ApiResponse<object>.Fail(ex.Message, "EXPORT_ERROR"));
        }
    }

    /// <summary>
    /// 查詢匯出任務狀態
    /// </summary>
    [HttpGet("excel-export/tasks/{taskId}")]
    public async Task<ActionResult<ApiResponse<ExcelExportTaskDto>>> GetExportTaskStatus(string taskId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetExportTaskStatusAsync(taskId);
            if (result == null)
            {
                throw new Exception("匯出任務不存在");
            }
            return result;
        }, "查詢匯出任務狀態失敗");
    }

    /// <summary>
    /// 下載匯出檔案
    /// </summary>
    [HttpGet("excel-export/download/{taskId}")]
    public async Task<ActionResult> DownloadExportFile(string taskId)
    {
        try
        {
            var fileBytes = await _service.DownloadExportFileAsync(taskId);
            if (fileBytes == null)
            {
                return NotFound(ApiResponse<object>.Fail("檔案不存在", "FILE_NOT_FOUND"));
            }

            var task = await _service.GetExportTaskStatusAsync(taskId);
            var fileName = task?.FileName ?? $"export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("下載匯出檔案失敗", ex);
            return BadRequest(ApiResponse<object>.Fail(ex.Message, "DOWNLOAD_ERROR"));
        }
    }

    /// <summary>
    /// 新增匯出設定
    /// </summary>
    [HttpPost("excel-export/config")]
    public async Task<ActionResult<ApiResponse<long>>> CreateExportConfig([FromBody] CreateExcelExportConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateExportConfigAsync(dto);
            return result;
        }, "新增匯出設定失敗");
    }

    /// <summary>
    /// 修改匯出設定
    /// </summary>
    [HttpPut("excel-export/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateExportConfig(long configId, [FromBody] CreateExcelExportConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateExportConfigAsync(configId, dto);
        }, "修改匯出設定失敗");
    }

    /// <summary>
    /// 刪除匯出設定
    /// </summary>
    [HttpDelete("excel-export/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteExportConfig(long configId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteExportConfigAsync(configId);
        }, "刪除匯出設定失敗");
    }

    #endregion

    #region Encode_String - 字串編碼工具

    /// <summary>
    /// 編碼字串
    /// </summary>
    [HttpPost("encode-string/encode")]
    public async Task<ActionResult<ApiResponse<EncodeStringResultDto>>> EncodeString([FromBody] EncodeStringRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.EncodeStringAsync(request);
            return result;
        }, "編碼字串失敗");
    }

    /// <summary>
    /// 解碼字串
    /// </summary>
    [HttpPost("encode-string/decode")]
    public async Task<ActionResult<ApiResponse<DecodeStringResultDto>>> DecodeString([FromBody] DecodeStringRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.DecodeStringAsync(request);
            return result;
        }, "解碼字串失敗");
    }

    #endregion

    #region ASPXTOASP - ASP轉ASPX工具

    /// <summary>
    /// ASPX轉ASP
    /// </summary>
    [HttpPost("aspx-to-asp")]
    public async Task<ActionResult> AspxToAsp([FromBody] PageTransitionDto dto)
    {
        try
        {
            var html = await _service.ConvertAspxToAspAsync(dto);
            return Content(html, "text/html; charset=utf-8");
        }
        catch (Exception ex)
        {
            _logger.LogError("ASPX轉ASP失敗", ex);
            return BadRequest(ApiResponse<object>.Fail(ex.Message, "CONVERSION_ERROR"));
        }
    }

    /// <summary>
    /// ASP轉ASPX
    /// </summary>
    [HttpPost("asp-to-aspx")]
    public async Task<ActionResult> AspToAspx([FromBody] PageTransitionDto dto)
    {
        try
        {
            var html = await _service.ConvertAspToAspxAsync(dto);
            return Content(html, "text/html; charset=utf-8");
        }
        catch (Exception ex)
        {
            _logger.LogError("ASP轉ASPX失敗", ex);
            return BadRequest(ApiResponse<object>.Fail(ex.Message, "CONVERSION_ERROR"));
        }
    }

    /// <summary>
    /// 查詢頁面轉換記錄
    /// </summary>
    [HttpGet("page-transitions")]
    public async Task<ActionResult<ApiResponse<PagedResult<PageTransitionLogDto>>>> GetPageTransitions([FromQuery] PageTransitionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPageTransitionsAsync(query);
            return result;
        }, "查詢頁面轉換記錄失敗");
    }

    /// <summary>
    /// 取得頁面轉換對應設定列表
    /// </summary>
    [HttpGet("page-transition-mappings")]
    public async Task<ActionResult<ApiResponse<List<PageTransitionMappingDto>>>> GetPageTransitionMappings()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPageTransitionMappingsAsync();
            return result;
        }, "取得頁面轉換對應設定列表失敗");
    }

    /// <summary>
    /// 新增頁面轉換對應設定
    /// </summary>
    [HttpPost("page-transition-mappings")]
    public async Task<ActionResult<ApiResponse<long>>> CreatePageTransitionMapping([FromBody] CreatePageTransitionMappingDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreatePageTransitionMappingAsync(dto);
            return result;
        }, "新增頁面轉換對應設定失敗");
    }

    /// <summary>
    /// 修改頁面轉換對應設定
    /// </summary>
    [HttpPut("page-transition-mappings/{mappingId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePageTransitionMapping(long mappingId, [FromBody] CreatePageTransitionMappingDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePageTransitionMappingAsync(mappingId, dto);
        }, "修改頁面轉換對應設定失敗");
    }

    /// <summary>
    /// 刪除頁面轉換對應設定
    /// </summary>
    [HttpDelete("page-transition-mappings/{mappingId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePageTransitionMapping(long mappingId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePageTransitionMappingAsync(mappingId);
        }, "刪除頁面轉換對應設定失敗");
    }

    #endregion
}

