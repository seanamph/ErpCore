namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃處理主檔 (SYS8B50-SYS8B90)
/// </summary>
public class LeaseProcess
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 處理編號
    /// </summary>
    public string ProcessId { get; set; } = string.Empty;

    /// <summary>
    /// 租賃編號
    /// </summary>
    public string LeaseId { get; set; } = string.Empty;

    /// <summary>
    /// 處理類型 (RENEWAL:續約, TERMINATION:終止, MODIFICATION:修改, PAYMENT:付款)
    /// </summary>
    public string ProcessType { get; set; } = string.Empty;

    /// <summary>
    /// 處理日期
    /// </summary>
    public DateTime ProcessDate { get; set; }

    /// <summary>
    /// 處理狀態 (P:待處理, I:處理中, C:已完成, X:已取消)
    /// </summary>
    public string ProcessStatus { get; set; } = "P";

    /// <summary>
    /// 處理結果 (SUCCESS:成功, FAILED:失敗, PENDING:待處理)
    /// </summary>
    public string? ProcessResult { get; set; }

    /// <summary>
    /// 處理人員
    /// </summary>
    public string? ProcessUserId { get; set; }

    /// <summary>
    /// 處理人員名稱
    /// </summary>
    public string? ProcessUserName { get; set; }

    /// <summary>
    /// 處理備註
    /// </summary>
    public string? ProcessMemo { get; set; }

    /// <summary>
    /// 審核人員
    /// </summary>
    public string? ApprovalUserId { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? ApprovalDate { get; set; }

    /// <summary>
    /// 審核狀態 (P:待審核, A:已審核, R:已拒絕)
    /// </summary>
    public string? ApprovalStatus { get; set; }

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
/// 租賃處理明細 (SYS8B50-SYS8B90)
/// </summary>
public class LeaseProcessDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 處理編號
    /// </summary>
    public string ProcessId { get; set; } = string.Empty;

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 欄位名稱
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// 舊值
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// 新值
    /// </summary>
    public string? NewValue { get; set; }

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
}

/// <summary>
/// 租賃處理日誌 (SYS8B50-SYS8B90)
/// </summary>
public class LeaseProcessLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 處理編號
    /// </summary>
    public string ProcessId { get; set; } = string.Empty;

    /// <summary>
    /// 日誌日期
    /// </summary>
    public DateTime LogDate { get; set; }

    /// <summary>
    /// 日誌類型 (INFO:資訊, WARNING:警告, ERROR:錯誤)
    /// </summary>
    public string? LogType { get; set; }

    /// <summary>
    /// 日誌訊息
    /// </summary>
    public string? LogMessage { get; set; }

    /// <summary>
    /// 操作人員
    /// </summary>
    public string? LogUserId { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

