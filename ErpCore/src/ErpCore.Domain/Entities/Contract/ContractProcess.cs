namespace ErpCore.Domain.Entities.Contract;

/// <summary>
/// 合同處理主檔 (SYSF210-SYSF220)
/// </summary>
public class ContractProcess
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
    /// 合同編號
    /// </summary>
    public string ContractId { get; set; } = string.Empty;

    /// <summary>
    /// 版本號
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 處理類型 (PAY:付款, REC:收款, CHG:變更, TER:終止)
    /// </summary>
    public string ProcessType { get; set; } = string.Empty;

    /// <summary>
    /// 處理日期
    /// </summary>
    public DateTime ProcessDate { get; set; }

    /// <summary>
    /// 處理金額
    /// </summary>
    public decimal? ProcessAmount { get; set; } = 0;

    /// <summary>
    /// 狀態 (P:待處理, I:處理中, C:已完成, X:已取消)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 處理人員
    /// </summary>
    public string? ProcessUserId { get; set; }

    /// <summary>
    /// 處理備註
    /// </summary>
    public string? ProcessMemo { get; set; }

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

