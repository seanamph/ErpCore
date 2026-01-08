namespace ErpCore.Domain.Entities.Contract;

/// <summary>
/// CMS合同條款 (CMS2310-CMS2320)
/// </summary>
public class CmsContractTerm
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// CMS合同編號
    /// </summary>
    public string CmsContractId { get; set; } = string.Empty;

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

