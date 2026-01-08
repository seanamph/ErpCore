namespace ErpCore.Domain.Entities.Query;

/// <summary>
/// 傳票審核傳送檔 (SYSQ250)
/// </summary>
public class VoucherAudit
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 傳票編號
    /// </summary>
    public string VoucherId { get; set; } = string.Empty;

    /// <summary>
    /// 傳票種類
    /// </summary>
    public string? VoucherKind { get; set; }

    /// <summary>
    /// 傳票日期
    /// </summary>
    public DateTime VoucherDate { get; set; }

    /// <summary>
    /// 審核狀態 (PENDING:待審核, APPROVED:已審核, REJECTED:已拒絕)
    /// </summary>
    public string? AuditStatus { get; set; }

    /// <summary>
    /// 審核者
    /// </summary>
    public string? AuditUser { get; set; }

    /// <summary>
    /// 審核時間
    /// </summary>
    public DateTime? AuditTime { get; set; }

    /// <summary>
    /// 審核備註
    /// </summary>
    public string? AuditNotes { get; set; }

    /// <summary>
    /// 傳送狀態 (PENDING:待傳送, SENT:已傳送)
    /// </summary>
    public string? SendStatus { get; set; }

    /// <summary>
    /// 傳送時間
    /// </summary>
    public DateTime? SendTime { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? BUser { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime BTime { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? CUser { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? CTime { get; set; }
}

