namespace ErpCore.Domain.Entities.CommunicationModule;

/// <summary>
/// XCOM系統參數實體 (XCOM2A0)
/// </summary>
public class XComSystemParam
{
    /// <summary>
    /// 參數代碼 (主鍵)
    /// </summary>
    public string ParamCode { get; set; } = string.Empty;

    /// <summary>
    /// 參數名稱
    /// </summary>
    public string ParamName { get; set; } = string.Empty;

    /// <summary>
    /// 參數值
    /// </summary>
    public string? ParamValue { get; set; }

    /// <summary>
    /// 參數類型 (STRING, NUMBER, BOOLEAN, DATE等)
    /// </summary>
    public string? ParamType { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 系統ID
    /// </summary>
    public string? SystemId { get; set; }

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

