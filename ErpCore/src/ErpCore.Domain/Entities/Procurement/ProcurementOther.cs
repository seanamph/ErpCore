namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 採購其他功能主檔 (SYSP510-SYSP530)
/// </summary>
public class ProcurementOther
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 功能代碼
    /// </summary>
    public string FunctionId { get; set; } = string.Empty;

    /// <summary>
    /// 功能名稱
    /// </summary>
    public string FunctionName { get; set; } = string.Empty;

    /// <summary>
    /// 功能類型 (TOOL:工具, AUX:輔助, PROCESS:處理)
    /// </summary>
    public string? FunctionType { get; set; }

    /// <summary>
    /// 功能說明
    /// </summary>
    public string? FunctionDesc { get; set; }

    /// <summary>
    /// 功能配置 (JSON格式)
    /// </summary>
    public string? FunctionConfig { get; set; }

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

