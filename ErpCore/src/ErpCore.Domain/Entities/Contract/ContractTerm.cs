namespace ErpCore.Domain.Entities.Contract;

/// <summary>
/// 合同條款 (SYSF110-SYSF140)
/// </summary>
public class ContractTerm
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
    /// 條款類型
    /// </summary>
    public string? TermType { get; set; }

    /// <summary>
    /// 條款內容
    /// </summary>
    public string? TermContent { get; set; }

    /// <summary>
    /// 條款順序
    /// </summary>
    public int? TermOrder { get; set; }

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

