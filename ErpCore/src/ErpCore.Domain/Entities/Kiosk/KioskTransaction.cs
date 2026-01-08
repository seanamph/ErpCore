namespace ErpCore.Domain.Entities.Kiosk;

/// <summary>
/// Kiosk交易主檔實體
/// </summary>
public class KioskTransaction
{
    /// <summary>
    /// 主鍵ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 交易編號
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Kiosk機號
    /// </summary>
    public string KioskId { get; set; } = string.Empty;

    /// <summary>
    /// 功能代碼 (O2, A1, C2, D4, D8等)
    /// </summary>
    public string FunctionCode { get; set; } = string.Empty;

    /// <summary>
    /// 卡片編號
    /// </summary>
    public string? CardNumber { get; set; }

    /// <summary>
    /// 會員編號
    /// </summary>
    public string? MemberId { get; set; }

    /// <summary>
    /// 交易日期時間
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// 請求資料（JSON格式）
    /// </summary>
    public string? RequestData { get; set; }

    /// <summary>
    /// 回應資料（JSON格式）
    /// </summary>
    public string? ResponseData { get; set; }

    /// <summary>
    /// 狀態 (Success/Failed)
    /// </summary>
    public string Status { get; set; } = "Success";

    /// <summary>
    /// 回應碼
    /// </summary>
    public string? ReturnCode { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

