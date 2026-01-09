namespace ErpCore.Domain.Entities.SystemExtensionH;

/// <summary>
/// 人事匯入記錄 (SYSH3D0_FMI - 人事批量新增)
/// </summary>
public class PersonnelImportLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 匯入批次編號
    /// </summary>
    public string ImportId { get; set; } = string.Empty;

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 總筆數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 成功筆數
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失敗筆數
    /// </summary>
    public int FailCount { get; set; }

    /// <summary>
    /// 匯入狀態 (PENDING, PROCESSING, SUCCESS, FAILED)
    /// </summary>
    public string ImportStatus { get; set; } = "PENDING";

    /// <summary>
    /// 匯入日期
    /// </summary>
    public DateTime ImportDate { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

