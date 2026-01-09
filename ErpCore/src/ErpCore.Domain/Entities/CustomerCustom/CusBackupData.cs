namespace ErpCore.Domain.Entities.CustomerCustom;

/// <summary>
/// CUSBACKUP 客戶模組備份實體
/// </summary>
public class CusBackupData
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 備份編號
    /// </summary>
    public string BackupId { get; set; } = string.Empty;

    /// <summary>
    /// 客戶代碼
    /// </summary>
    public string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// 模組代碼
    /// </summary>
    public string ModuleCode { get; set; } = string.Empty;

    /// <summary>
    /// 備份版本
    /// </summary>
    public string? BackupVersion { get; set; }

    /// <summary>
    /// 備份日期
    /// </summary>
    public DateTime BackupDate { get; set; }

    /// <summary>
    /// 備份路徑
    /// </summary>
    public string? BackupPath { get; set; }

    /// <summary>
    /// 備份類型 (FULL, INCREMENTAL)
    /// </summary>
    public string? BackupType { get; set; }

    /// <summary>
    /// 檔案數量
    /// </summary>
    public int FileCount { get; set; }

    /// <summary>
    /// 檔案大小（位元組）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 備份說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

