namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 費用/收入分攤比率設定實體 (SYST212)
/// </summary>
public class AllocationRatio
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 分攤年月 (YYYYMM)
    /// </summary>
    public string DisYm { get; set; } = string.Empty;

    /// <summary>
    /// 會計科目 (外鍵至AccountSubjects)
    /// </summary>
    public string StypeId { get; set; } = string.Empty;

    /// <summary>
    /// 組織代號
    /// </summary>
    public string OrgId { get; set; } = string.Empty;

    /// <summary>
    /// 分攤比率 (0-1)
    /// </summary>
    public decimal Ratio { get; set; }

    /// <summary>
    /// 傳票主檔T_KEY (外鍵至Vouchers)
    /// </summary>
    public long? VoucherTKey { get; set; }

    /// <summary>
    /// 傳票明細T_KEY (外鍵至VoucherDetails)
    /// </summary>
    public long? VoucherDTKey { get; set; }

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

