namespace ErpCore.Domain.Entities.Extension;

/// <summary>
/// 擴展功能資料實體 (SYS9000)
/// </summary>
public class ExtensionFunction
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 擴展功能代碼
    /// </summary>
    public string ExtensionId { get; set; } = string.Empty;

    /// <summary>
    /// 擴展功能名稱
    /// </summary>
    public string ExtensionName { get; set; } = string.Empty;

    /// <summary>
    /// 擴展類型 (BASE:基礎, PROCESS:處理, REPORT:報表, QUERY:查詢)
    /// </summary>
    public string? ExtensionType { get; set; }

    /// <summary>
    /// 擴展值
    /// </summary>
    public string? ExtensionValue { get; set; }

    /// <summary>
    /// 擴展設定 (JSON格式)
    /// </summary>
    public string? ExtensionConfig { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 版本號
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

/// <summary>
/// 擴展功能查詢條件
/// </summary>
public class ExtensionFunctionQuery
{
    public string? ExtensionId { get; set; }
    public string? ExtensionName { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

