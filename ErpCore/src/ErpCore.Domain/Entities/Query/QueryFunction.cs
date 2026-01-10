namespace ErpCore.Domain.Entities.Query;

/// <summary>
/// 查詢功能 (SYSQ000)
/// </summary>
public class QueryFunction
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 查詢功能代碼
    /// </summary>
    public string QueryId { get; set; } = string.Empty;

    /// <summary>
    /// 查詢功能名稱
    /// </summary>
    public string QueryName { get; set; } = string.Empty;

    /// <summary>
    /// 查詢類型
    /// </summary>
    public string? QueryType { get; set; }

    /// <summary>
    /// 查詢SQL語句
    /// </summary>
    public string? QuerySql { get; set; }

    /// <summary>
    /// 查詢設定 (JSON格式)
    /// </summary>
    public string? QueryConfig { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int SeqNo { get; set; }

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

