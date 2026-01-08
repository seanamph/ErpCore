namespace ErpCore.Domain.Entities.OtherManagement;

/// <summary>
/// J系統功能資料實體 (SYSJ000)
/// </summary>
public class SYSJFunction
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
    /// 功能類型
    /// </summary>
    public string? FunctionType { get; set; }

    /// <summary>
    /// 功能值
    /// </summary>
    public string? FunctionValue { get; set; }

    /// <summary>
    /// 功能設定 (JSON格式)
    /// </summary>
    public string? FunctionConfig { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

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
/// J系統功能查詢條件
/// </summary>
public class SYSJFunctionQuery
{
    public string? FunctionId { get; set; }
    public string? FunctionName { get; set; }
    public string? FunctionType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

