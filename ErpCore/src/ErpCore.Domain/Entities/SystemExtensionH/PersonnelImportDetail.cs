namespace ErpCore.Domain.Entities.SystemExtensionH;

/// <summary>
/// 人事匯入明細 (SYSH3D0_FMI - 人事批量新增)
/// </summary>
public class PersonnelImportDetail
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
    /// 行號
    /// </summary>
    public int RowNum { get; set; }

    /// <summary>
    /// 人事編號
    /// </summary>
    public string? PersonnelId { get; set; }

    /// <summary>
    /// 人事姓名
    /// </summary>
    public string? PersonnelName { get; set; }

    /// <summary>
    /// 匯入狀態 (SUCCESS, FAILED)
    /// </summary>
    public string ImportStatus { get; set; } = "FAILED";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

