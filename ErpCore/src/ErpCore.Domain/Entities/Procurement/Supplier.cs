namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 供應商主檔 (SYSP210-SYSP260)
/// </summary>
public class Supplier
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 供應商編號
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 供應商名稱
    /// </summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// 供應商英文名稱
    /// </summary>
    public string? SupplierNameE { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 傳真
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 付款條件
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 評等
    /// </summary>
    public string? Rating { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

