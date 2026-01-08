namespace ErpCore.Domain.Entities.Pos;

/// <summary>
/// POS同步記錄實體
/// </summary>
public class PosSyncLog
{
    /// <summary>
    /// 主鍵ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 同步類型 (Transaction/Product/Inventory)
    /// </summary>
    public string SyncType { get; set; } = string.Empty;

    /// <summary>
    /// 同步方向 (ToIMS/FromPOS)
    /// </summary>
    public string SyncDirection { get; set; } = string.Empty;

    /// <summary>
    /// 記錄筆數
    /// </summary>
    public int RecordCount { get; set; }

    /// <summary>
    /// 成功筆數
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失敗筆數
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// 狀態 (Running/Completed/Failed)
    /// </summary>
    public string Status { get; set; } = "Running";

    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime? EndTime { get; set; }

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
}

