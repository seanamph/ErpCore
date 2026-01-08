namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 傳票轉入記錄檔實體 (SYST002-SYST003)
/// </summary>
public class VoucherImportLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 轉入類型 (AHM:住金, HTV:日立)
    /// </summary>
    public string ImportType { get; set; } = string.Empty;

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 檔案路徑
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// 轉入日期
    /// </summary>
    public DateTime ImportDate { get; set; }

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
    /// 跳過筆數 (已存在)
    /// </summary>
    public int SkipCount { get; set; }

    /// <summary>
    /// 狀態 (P:處理中, S:成功, F:失敗)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

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

