namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃合同資料 (SYSM111-SYSM138)
/// </summary>
public class LeaseContract
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 合同編號
    /// </summary>
    public string ContractNo { get; set; } = string.Empty;

    /// <summary>
    /// 租賃編號
    /// </summary>
    public string LeaseId { get; set; } = string.Empty;

    /// <summary>
    /// 合同日期
    /// </summary>
    public DateTime ContractDate { get; set; }

    /// <summary>
    /// 合同類型
    /// </summary>
    public string? ContractType { get; set; }

    /// <summary>
    /// 合同內容
    /// </summary>
    public string? ContractContent { get; set; }

    /// <summary>
    /// 狀態 (A:有效, I:無效)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 簽約人
    /// </summary>
    public string? SignedBy { get; set; }

    /// <summary>
    /// 簽約日期
    /// </summary>
    public DateTime? SignedDate { get; set; }

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

