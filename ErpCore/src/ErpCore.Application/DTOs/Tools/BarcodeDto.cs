namespace ErpCore.Application.DTOs.Tools;

/// <summary>
/// 條碼產生請求 DTO
/// </summary>
public class BarcodeGenerateDto
{
    public string Content { get; set; } = string.Empty;
    public string Format { get; set; } = "Code128"; // Code128, EAN13, QRCode
    public int Width { get; set; } = 200;
    public int Height { get; set; } = 100;
    public bool ShowText { get; set; } = true;
}

/// <summary>
/// 條碼驗證請求 DTO
/// </summary>
public class BarcodeValidateDto
{
    public string Content { get; set; } = string.Empty;
    public string Format { get; set; } = "Code128";
}

/// <summary>
/// 條碼驗證結果 DTO
/// </summary>
public class BarcodeValidationResultDto
{
    public bool IsValid { get; set; }
    public string Format { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

