using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Tools;
using ErpCore.Application.Services.Tools;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Tools;

/// <summary>
/// 條碼處理工具控制器 (RSL_BARCODE)
/// </summary>
[Route("api/v1/utils/barcode")]
public class BarcodeController : BaseController
{
    private readonly IBarcodeService _service;

    public BarcodeController(
        IBarcodeService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 產生條碼圖片
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateBarcode([FromBody] BarcodeGenerateDto dto)
    {
        try
        {
            var imageBytes = await _service.GenerateBarcodeAsync(dto);
            return File(imageBytes, "image/png", $"barcode_{DateTime.Now:yyyyMMddHHmmss}.png");
        }
        catch (Exception ex)
        {
            _logger.LogError("產生條碼失敗", ex);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = 400,
                Message = $"產生條碼失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 驗證條碼格式
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<BarcodeValidationResultDto>>> ValidateBarcode([FromBody] BarcodeValidateDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ValidateBarcodeAsync(dto);
            return result;
        }, "驗證條碼失敗");
    }
}

