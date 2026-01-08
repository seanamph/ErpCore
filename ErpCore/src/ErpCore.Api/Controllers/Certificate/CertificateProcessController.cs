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

    // TODO: 實作其他處理功能（導入、列印、匯出等）
}

