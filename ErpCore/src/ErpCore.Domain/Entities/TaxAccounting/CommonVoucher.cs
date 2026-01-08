namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 常用傳票主檔實體 (SYST123)
/// </summary>
public class CommonVoucher
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 傳票代號
    /// </summary>
    public string VoucherId { get; set; } = string.Empty;

    /// <summary>
    /// 傳票名稱
    /// </summary>
    public string VoucherName { get; set; } = string.Empty;

    /// <summary>
    /// 傳票型態
    /// </summary>
    public string? VoucherType { get; set; }

    /// <summary>
    /// 店代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 廠客代號
    /// </summary>
    public string? VendorId { get; set; }

    /// <summary>
    /// 廠商名稱
    /// </summary>
    public string? VendorName { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 自訂欄位1
    /// </summary>
    public string? CustomField1 { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

