namespace ErpCore.Domain.Entities.ReportManagement;

/// <summary>
/// 收款項目主檔 (SYSR110-SYSR120)
/// </summary>
public class ArItems
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 分店代號 (SITE_ID)
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 收款項目代號 (ARITEM_ID)
    /// </summary>
    public string AritemId { get; set; } = string.Empty;

    /// <summary>
    /// 收款項目名稱 (ARITEM_NAME)
    /// </summary>
    public string AritemName { get; set; } = string.Empty;

    /// <summary>
    /// 會計科目代號 (STYPE_ID)
    /// </summary>
    public string? StypeId { get; set; }

    /// <summary>
    /// 備註 (NOTES)
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立人員 (BUSER)
    /// </summary>
    public string? Buser { get; set; }

    /// <summary>
    /// 建立時間 (BTIME)
    /// </summary>
    public DateTime? Btime { get; set; }

    /// <summary>
    /// 建立優先權 (CPRIORITY)
    /// </summary>
    public int? Cpriority { get; set; }

    /// <summary>
    /// 建立群組 (CGROUP)
    /// </summary>
    public string? Cgroup { get; set; }

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
}

