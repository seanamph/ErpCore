namespace ErpCore.Domain.Entities.Contract;

/// <summary>
/// 合同會計分類 (SYSF110-SYSF140)
/// </summary>
public class ContractAccounting
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 合同編號
    /// </summary>
    public string ContractId { get; set; } = string.Empty;

    /// <summary>
    /// 版本號
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 會計科目代碼
    /// </summary>
    public string? AccountCode { get; set; }

    /// <summary>
    /// 會計科目名稱
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

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

