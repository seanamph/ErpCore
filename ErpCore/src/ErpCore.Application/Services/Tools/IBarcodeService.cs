using ErpCore.Application.DTOs.Tools;

namespace ErpCore.Application.Services.Tools;

/// <summary>
/// 條碼服務介面
/// </summary>
public interface IBarcodeService
{
    /// <summary>
    /// 產生條碼圖片
    /// </summary>
    Task<byte[]> GenerateBarcodeAsync(BarcodeGenerateDto dto);

    /// <summary>
    /// 驗證條碼格式
    /// </summary>
    Task<BarcodeValidationResultDto> ValidateBarcodeAsync(BarcodeValidateDto dto);
}

