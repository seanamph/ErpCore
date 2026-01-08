namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 傳票轉入明細記錄檔實體 (SYST002-SYST003)
/// </summary>
public class VoucherImportDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 轉入記錄TKey
    /// </summary>
    public long ImportLogTKey { get; set; }

    /// <summary>
    /// 行號
    /// </summary>
    public int? RowNumber { get; set; }

    /// <summary>
    /// 傳票主檔TKey (轉入成功時填入)
    /// </summary>
    public long? VoucherTKey { get; set; }

    /// <summary>
    /// 狀態 (P:處理中, S:成功, F:失敗, K:跳過)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 原始資料 (JSON格式)
    /// </summary>
    public string? SourceData { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

