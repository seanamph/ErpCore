namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 單位領用申請單明細檔 (SYSA210)
/// </summary>
public class MaterialApplyDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 主檔主鍵
    /// </summary>
    public long ApplyMasterKey { get; set; }

    /// <summary>
    /// 領用單號
    /// </summary>
    public string ApplyId { get; set; } = string.Empty;

    /// <summary>
    /// 品項主鍵
    /// </summary>
    public long GoodsTKey { get; set; }

    /// <summary>
    /// 品項編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 申請數量
    /// </summary>
    public decimal ApplyQty { get; set; }

    /// <summary>
    /// 發料數量
    /// </summary>
    public decimal? IssueQty { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 附註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 序號
    /// </summary>
    public int SeqNo { get; set; }

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
    /// 主檔導航屬性
    /// </summary>
    public MaterialApplyMaster? Master { get; set; }
}
