using ErpCore.Application.DTOs.Tools;

namespace ErpCore.Application.Services.Tools;

/// <summary>
/// HTML轉PDF服務介面
/// </summary>
public interface IHtml2PdfService
{
    /// <summary>
    /// 將HTML內容轉換為PDF
    /// </summary>
    Task<Html2PdfResponseDto> ConvertHtmlToPdfAsync(Html2PdfRequestDto request);

    /// <summary>
    /// 下載PDF檔案
    /// </summary>
    Task<byte[]?> DownloadPdfAsync(Guid logId);

    /// <summary>
    /// 查詢PDF轉換記錄
    /// </summary>
    Task<PdfConversionLogDto?> GetConversionLogAsync(Guid logId);
}

