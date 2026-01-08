namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 暫存傳票明細檔實體 (SYSTA00-SYSTA70)
/// </summary>
public class TmpVoucherD
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 傳票主檔TKey
    /// </summary>
    public long VoucherTKey { get; set; }

    /// <summary>
    /// 序號
    /// </summary>
    public string Sn { get; set; } = string.Empty;

    /// <summary>
    /// 借貸方 (0:借方, 1:貸方)
    /// </summary>
    public string? Dc { get; set; }

    /// <summary>
    /// 科目代號
    /// </summary>
    public string? SubN { get; set; }

    /// <summary>
    /// 部門代號
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 專案代號
    /// </summary>
    public string? ActId { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 借方金額
    /// </summary>
    public decimal Val0 { get; set; }

    /// <summary>
    /// 貸方金額
    /// </summary>
    public decimal Val1 { get; set; }

    /// <summary>
    /// 立沖對象
    /// </summary>
    public string? SupN { get; set; }

    /// <summary>
    /// 立沖憑證
    /// </summary>
    public string? InN { get; set; }

    /// <summary>
    /// 廠商代號
    /// </summary>
    public string? VendorId { get; set; }

    /// <summary>
    /// 立沖代號
    /// </summary>
    public string? AbatId { get; set; }

    /// <summary>
    /// 關係人代號
    /// </summary>
    public string? ObjectId { get; set; }

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

