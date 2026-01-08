namespace ErpCore.Domain.Entities.Tools;

/// <summary>
/// 檔案上傳記錄實體
/// </summary>
public class FileUpload
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 檔案名稱（儲存後的檔案名稱）
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 原始檔案名稱
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// 檔案路徑
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 檔案大小（位元組）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 檔案類型（MIME類型）
    /// </summary>
    public string? FileType { get; set; }

    /// <summary>
    /// 副檔名
    /// </summary>
    public string? FileExtension { get; set; }

    /// <summary>
    /// 上傳路徑
    /// </summary>
    public string? UploadPath { get; set; }

    /// <summary>
    /// 上傳者
    /// </summary>
    public string? UploadedBy { get; set; }

    /// <summary>
    /// 上傳時間
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// 狀態（1:正常, 0:已刪除）
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 關聯資料表
    /// </summary>
    public string? RelatedTable { get; set; }

    /// <summary>
    /// 關聯ID
    /// </summary>
    public string? RelatedId { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
}

