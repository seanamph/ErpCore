namespace ErpCore.Domain.Entities.Contract;

/// <summary>
/// 合同擴展主檔 (SYSF350-SYSF540)
/// </summary>
public class ContractExtension
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
    public int Version { get; set; } = 1;

    /// <summary>
    /// 擴展類型
    /// </summary>
    public string? ExtensionType { get; set; }

    /// <summary>
    /// 供應商代碼
    /// </summary>
    public string? VendorId { get; set; }

    /// <summary>
    /// 供應商名稱
    /// </summary>
    public string? VendorName { get; set; }

    /// <summary>
    /// 擴展日期
    /// </summary>
    public DateTime? ExtensionDate { get; set; }

    /// <summary>
    /// 擴展金額
    /// </summary>
    public decimal ExtensionAmount { get; set; } = 0;

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

