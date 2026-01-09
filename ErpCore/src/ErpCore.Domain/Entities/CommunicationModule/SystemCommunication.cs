namespace ErpCore.Domain.Entities.CommunicationModule;

/// <summary>
/// 系統通訊設定實體 (XCOM000)
/// </summary>
public class SystemCommunication
{
    /// <summary>
    /// 通訊ID
    /// </summary>
    public long CommunicationId { get; set; }

    /// <summary>
    /// 系統代碼
    /// </summary>
    public string SystemCode { get; set; } = string.Empty;

    /// <summary>
    /// 系統名稱
    /// </summary>
    public string SystemName { get; set; } = string.Empty;

    /// <summary>
    /// 通訊類型 (REST, SOAP, JSON, ETEK)
    /// </summary>
    public string CommunicationType { get; set; } = string.Empty;

    /// <summary>
    /// 端點URL
    /// </summary>
    public string? EndpointUrl { get; set; }

    /// <summary>
    /// API金鑰 (加密)
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// API密鑰 (加密)
    /// </summary>
    public string? ApiSecret { get; set; }

    /// <summary>
    /// 設定資料 (JSON格式)
    /// </summary>
    public string? ConfigData { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

