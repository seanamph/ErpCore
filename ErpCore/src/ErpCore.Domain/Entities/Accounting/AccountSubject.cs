namespace ErpCore.Domain.Entities.Accounting;

/// <summary>
/// 會計科目實體 (SYSN110)
/// </summary>
public class AccountSubject
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 科目代號
    /// </summary>
    public string StypeId { get; set; } = string.Empty;

    /// <summary>
    /// 科目名稱
    /// </summary>
    public string StypeName { get; set; } = string.Empty;

    /// <summary>
    /// 科目英文名稱
    /// </summary>
    public string? StypeNameE { get; set; }

    /// <summary>
    /// 借/貸 (D:借方, C:貸方)
    /// </summary>
    public string? Dc { get; set; }

    /// <summary>
    /// 統制/明細 (L:統制, M:明細)
    /// </summary>
    public string? LedgerMd { get; set; }

    /// <summary>
    /// 三碼代號
    /// </summary>
    public string? MtypeId { get; set; }

    /// <summary>
    /// 是否為沖帳代號 (Y/N)
    /// </summary>
    public string? AbatYn { get; set; }

    /// <summary>
    /// 傳票格式
    /// </summary>
    public string? VoucherType { get; set; }

    /// <summary>
    /// 是否為預算科目 (Y/N)
    /// </summary>
    public string? BudgetYn { get; set; }

    /// <summary>
    /// 是否設定部門代號 (Y/N)
    /// </summary>
    public string? OrgYn { get; set; }

    /// <summary>
    /// 折舊攤提年限
    /// </summary>
    public decimal? ExpYear { get; set; }

    /// <summary>
    /// 殘值年限
    /// </summary>
    public decimal? ResiValue { get; set; }

    /// <summary>
    /// 折舊會計科目
    /// </summary>
    public string? DepreLid { get; set; }

    /// <summary>
    /// 累計折舊會計科目
    /// </summary>
    public string? AccudepreLid { get; set; }

    /// <summary>
    /// 是否可輸 (Y/N)
    /// </summary>
    public string? StypeYn { get; set; }

    /// <summary>
    /// IFRS會計科目
    /// </summary>
    public string? IfrsStypeId { get; set; }

    /// <summary>
    /// 集團會計科目
    /// </summary>
    public string? RocStypeId { get; set; }

    /// <summary>
    /// SAP會計科目
    /// </summary>
    public string? SapStypeId { get; set; }

    /// <summary>
    /// 科目別
    /// </summary>
    public string? StypeClass { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? StypeOrder { get; set; }

    /// <summary>
    /// 期初餘額
    /// </summary>
    public decimal? Amt0 { get; set; }

    /// <summary>
    /// 期末餘額
    /// </summary>
    public decimal? Amt1 { get; set; }

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

