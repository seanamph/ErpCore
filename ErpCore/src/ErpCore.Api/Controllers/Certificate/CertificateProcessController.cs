using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Certificate;
using ErpCore.Application.Services.Certificate;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Certificate;

/// <summary>
/// 憑證處理作業控制器 (SYSK210-SYSK230)
/// 提供憑證檢查、導入、列印、匯出等功能
/// </summary>
[Route("api/v1/vouchers/process")]
public class CertificateProcessController : BaseController
{
    private readonly IVoucherService _voucherService;

    public CertificateProcessController(
        IVoucherService voucherService,
        ILoggerService logger) : base(logger)
    {
        _voucherService = voucherService;
    }

    /// <summary>
    /// 憑證檢查
    /// </summary>
    [HttpPost("check")]
    public async Task<ActionResult<ApiResponse<List<VoucherCheckResultDto>>>> CheckVouchers(
        [FromBody] List<string> voucherIds)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherService.CheckVouchersAsync(voucherIds);
            return result;
        }, "檢查憑證失敗");
    }

    /// <summary>
    /// 憑證導入
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<ApiResponse<VoucherProcessResultDto>>> ImportVouchers(IFormFile file)
    {
        return await ExecuteAsync(async () =>
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("檔案不能為空");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileData = memoryStream.ToArray();

            var result = await _voucherService.ImportVouchersAsync(fileData, file.FileName);
            return result;
        }, "導入憑證失敗");
    }

    /// <summary>
    /// 憑證列印
    /// </summary>
    [HttpPost("print")]
    public async Task<ActionResult<ApiResponse<PrintResultDto>>> PrintVouchers(
        [FromBody] PrintVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _voucherService.PrintVouchersAsync(dto);
            return result;
        }, "列印憑證失敗");
    }

    /// <summary>
    /// 憑證匯出
    /// </summary>
    [HttpPost("export")]
    public async Task<ActionResult> ExportVouchers([FromBody] ExportVoucherDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var fileData = await _voucherService.ExportVouchersAsync(dto);
            
            var contentType = dto.ExportFormat.ToUpper() == "PDF" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileExtension = dto.ExportFormat.ToUpper() == "PDF" ? ".pdf" : ".xlsx";
            var fileName = $"vouchers_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

            return File(fileData, contentType, fileName);
        }, "匯出憑證失敗");
    }

    /// <summary>
    /// 批量更新憑證狀態
    /// </summary>
    [HttpPut("batch-status")]
    public async Task<ActionResult<ApiResponse>> BatchUpdateStatus(
        [FromBody] BatchUpdateVoucherStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _voucherService.BatchUpdateVoucherStatusAsync(dto);
            return new { Message = "批量更新狀態成功" };
        }, "批量更新憑證狀態失敗");
    }
}

