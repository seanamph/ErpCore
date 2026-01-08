namespace ErpCore.Domain.Entities.Contract;

/// <summary>
/// CMS合同主檔 (CMS2310-CMS2320)
/// </summary>
public class CmsContract
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
    /// 合同類型
    /// </summary>
    public string ContractType { get; set; } = string.Empty;

    /// <summary>
    /// 版本號
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// 廠商代碼
    /// </summary>
    public string VendorId { get; set; } = string.Empty;

    /// <summary>
    /// 廠商名稱
    /// </summary>
    public string? VendorName { get; set; }

    /// <summary>
    /// 簽約日期
    /// </summary>
    public DateTime? SignDate { get; set; }

    /// <summary>
    /// 生效日期
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// 到期日期
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// 狀態 (D:草稿, P:審核中, A:已生效, E:已到期, T:已終止)
    /// </summary>
    public string Status { get; set; } = "D";

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; } = 0;

    /// <summary>
    /// 幣別
    /// </summary>
    public string CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 匯率
    /// </summary>
    public decimal ExchangeRate { get; set; } = 1;

    /// <summary>
    /// 位置編號
    /// </summary>
    public string? LocationId { get; set; }

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

