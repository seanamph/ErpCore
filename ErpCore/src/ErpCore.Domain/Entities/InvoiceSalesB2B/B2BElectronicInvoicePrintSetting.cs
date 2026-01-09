namespace ErpCore.Domain.Entities.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票列印設定實體 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public class B2BElectronicInvoicePrintSetting
{
    /// <summary>
    /// 設定編號
    /// </summary>
    public string SettingId { get; set; } = string.Empty;

    /// <summary>
    /// 列印格式 (A4, A5, THERMAL)
    /// </summary>
    public string PrintFormat { get; set; } = string.Empty;

    /// <summary>
    /// 條碼類型
    /// </summary>
    public string? BarcodeType { get; set; }

    /// <summary>
    /// 條碼大小
    /// </summary>
    public int? BarcodeSize { get; set; } = 40;

    /// <summary>
    /// 條碼邊距
    /// </summary>
    public int? BarcodeMargin { get; set; } = 5;

    /// <summary>
    /// 欄數
    /// </summary>
    public int? ColCount { get; set; } = 2;

    /// <summary>
    /// 頁數
    /// </summary>
    public int? PageCount { get; set; } = 14;

    /// <summary>
    /// B2B標記
    /// </summary>
    public string B2BFlag { get; set; } = "Y";

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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
}

