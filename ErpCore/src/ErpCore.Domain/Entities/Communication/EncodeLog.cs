namespace ErpCore.Domain.Entities.Communication;

/// <summary>
/// 編碼記錄實體
/// </summary>
public class EncodeLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 編碼類型 (Base64, String, Date, Data)
    /// </summary>
    public string EncodeType { get; set; } = string.Empty;

    /// <summary>
    /// 原始資料
    /// </summary>
    public string? OriginalData { get; set; }

    /// <summary>
    /// 編碼後資料
    /// </summary>
    public string? EncodedData { get; set; }

    /// <summary>
    /// 金鑰類型（用於字串加密）
    /// </summary>
    public string? KeyKind { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 用途說明
    /// </summary>
    public string? Purpose { get; set; }
}

