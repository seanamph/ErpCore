namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 採購擴展功能主檔 (SYSP610)
/// </summary>
public class PurchaseExtendedFunction
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 擴展功能代碼
    /// </summary>
    public string ExtFunctionId { get; set; } = string.Empty;

    /// <summary>
    /// 擴展功能名稱
    /// </summary>
    public string ExtFunctionName { get; set; } = string.Empty;

    /// <summary>
    /// 擴展功能類型
    /// </summary>
    public string? ExtFunctionType { get; set; }

    /// <summary>
    /// 擴展功能說明
    /// </summary>
    public string? ExtFunctionDesc { get; set; }

    /// <summary>
    /// 擴展功能配置 (JSON格式)
    /// </summary>
    public string? ExtFunctionConfig { get; set; }

    /// <summary>
    /// 參數配置 (JSON格式)
    /// </summary>
    public string? ParameterConfig { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 排序序號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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
