namespace ErpCore.Application.DTOs.Tools;

/// <summary>
/// HTML轉PDF請求 DTO
/// </summary>
public class Html2PdfRequestDto
{
    public string HtmlContent { get; set; } = string.Empty;
    public string FileName { get; set; } = "document.pdf";
    public PdfOptionsDto? Options { get; set; }
}

/// <summary>
/// PDF選項 DTO
/// </summary>
public class PdfOptionsDto
{
    public string PageSize { get; set; } = "A4"; // A4, Letter, etc.
    public string Orientation { get; set; } = "Portrait"; // Portrait, Landscape
    public PdfMarginDto? Margin { get; set; }
}

/// <summary>
/// PDF邊距 DTO
/// </summary>
public class PdfMarginDto
{
    public int Top { get; set; } = 10;
    public int Right { get; set; } = 10;
    public int Bottom { get; set; } = 10;
    public int Left { get; set; } = 10;
}

/// <summary>
/// HTML轉PDF回應 DTO
/// </summary>
public class Html2PdfResponseDto
{
    public Guid LogId { get; set; }
    public string PdfFilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

/// <summary>
/// PDF轉換記錄 DTO
/// </summary>
public class PdfConversionLogDto
{
    public Guid LogId { get; set; }
    public string? PdfFilePath { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string ConversionStatus { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

