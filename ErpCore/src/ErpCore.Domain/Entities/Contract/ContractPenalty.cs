namespace ErpCore.Domain.Entities.Contract;

/// <summary>
/// 合同罰則 (SYSF110-SYSF140)
/// </summary>
public class ContractPenalty
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
    /// 罰則類型
    /// </summary>
    public string? PenaltyType { get; set; }

    /// <summary>
    /// 罰則金額
    /// </summary>
    public decimal? PenaltyAmount { get; set; }

    /// <summary>
    /// 罰則比率
    /// </summary>
    public decimal? PenaltyRate { get; set; }

    /// <summary>
    /// 罰則說明
    /// </summary>
    public string? PenaltyDesc { get; set; }

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

