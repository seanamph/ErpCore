namespace ErpCore.Domain.Entities.Certificate;

/// <summary>
/// 憑證處理記錄 (SYSK210-SYSK230)
/// </summary>
public class VoucherProcessLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 憑證編號
    /// </summary>
    public string VoucherId { get; set; } = string.Empty;

    /// <summary>
    /// 處理類型 (CHECK:檢查, IMPORT:導入, PRINT:列印, EXPORT:匯出)
    /// </summary>
    public string ProcessType { get; set; } = string.Empty;

    /// <summary>
    /// 處理狀態 (SUCCESS:成功, FAILED:失敗)
    /// </summary>
    public string ProcessStatus { get; set; } = string.Empty;

    /// <summary>
    /// 處理訊息
    /// </summary>
    public string? ProcessMessage { get; set; }

    /// <summary>
    /// 處理人員
    /// </summary>
    public string? ProcessUserId { get; set; }

    /// <summary>
    /// 處理時間
    /// </summary>
    public DateTime ProcessDate { get; set; }

    /// <summary>
    /// 處理資料（JSON格式）
    /// </summary>
    public string? ProcessData { get; set; }
}

