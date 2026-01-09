namespace ErpCore.Domain.Entities.SapIntegration;

/// <summary>
/// SAP整合資料實體 (TransSAP系列)
/// </summary>
public class TransSap
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 交易單號
    /// </summary>
    public string TransId { get; set; } = string.Empty;

    /// <summary>
    /// 交易類型
    /// </summary>
    public string TransType { get; set; } = string.Empty;

    /// <summary>
    /// SAP系統代碼
    /// </summary>
    public string SapSystemCode { get; set; } = string.Empty;

    /// <summary>
    /// 交易日期
    /// </summary>
    public DateTime TransDate { get; set; }

    /// <summary>
    /// 交易狀態 (P:處理中, S:成功, F:失敗)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 請求資料 (JSON格式)
    /// </summary>
    public string? RequestData { get; set; }

    /// <summary>
    /// 回應資料 (JSON格式)
    /// </summary>
    public string? ResponseData { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 重試次數
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

