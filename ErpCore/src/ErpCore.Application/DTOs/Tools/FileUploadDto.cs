namespace ErpCore.Application.DTOs.Tools;

/// <summary>
/// 檔案上傳 DTO
/// </summary>
public class FileUploadDto
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public string? FileExtension { get; set; }
    public string? UploadPath { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
    public string Status { get; set; } = "1";
    public string? RelatedTable { get; set; }
    public string? RelatedId { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// 檔案上傳請求 DTO
/// </summary>
public class FileUploadRequestDto
{
    public string? UploadPath { get; set; }
    public string? RelatedTable { get; set; }
    public string? RelatedId { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// 檔案上傳回應 DTO
/// </summary>
public class FileUploadResponseDto
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public string? FileExtension { get; set; }
    public string? UploadPath { get; set; }
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// 檔案上傳查詢 DTO
/// </summary>
public class FileUploadQueryDto
{
    public string? UploadedBy { get; set; }
    public string? RelatedTable { get; set; }
    public string? RelatedId { get; set; }
    public string? Status { get; set; } = "1";
}

