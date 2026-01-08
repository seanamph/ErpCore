namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃擴展主檔 (SYS8A10-SYS8A45)
/// </summary>
public class LeaseExtension
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 擴展編號
    /// </summary>
    public string ExtensionId { get; set; } = string.Empty;

    /// <summary>
    /// 租賃編號
    /// </summary>
    public string LeaseId { get; set; } = string.Empty;

    /// <summary>
    /// 擴展類型 (CONDITION:特殊條件, TERM:附加條款, SETTING:擴展設定)
    /// </summary>
    public string ExtensionType { get; set; } = string.Empty;

    /// <summary>
    /// 擴展名稱
    /// </summary>
    public string? ExtensionName { get; set; }

    /// <summary>
    /// 擴展值
    /// </summary>
    public string? ExtensionValue { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; } = 0;

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

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

/// <summary>
/// 租賃擴展明細 (SYS8A10-SYS8A45)
/// </summary>
public class LeaseExtensionDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 擴展編號
    /// </summary>
    public string ExtensionId { get; set; } = string.Empty;

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 欄位名稱
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// 欄位值
    /// </summary>
    public string? FieldValue { get; set; }

    /// <summary>
    /// 欄位類型 (TEXT:文字, NUMBER:數字, DATE:日期, BOOLEAN:布林)
    /// </summary>
    public string? FieldType { get; set; }

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

