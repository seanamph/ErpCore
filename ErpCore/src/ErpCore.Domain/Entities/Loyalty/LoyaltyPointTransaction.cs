namespace ErpCore.Domain.Entities.Loyalty;

/// <summary>
/// 忠誠度點數交易主檔 (LPS - 忠誠度系統維護)
/// </summary>
public class LoyaltyPointTransaction
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 交易編號
    /// </summary>
    public string RRN { get; set; } = string.Empty;

    /// <summary>
    /// 會員卡號
    /// </summary>
    public string CardNo { get; set; } = string.Empty;

    /// <summary>
    /// 追蹤編號
    /// </summary>
    public string? TraceNo { get; set; }

    /// <summary>
    /// 到期日
    /// </summary>
    public string? ExpDate { get; set; }

    /// <summary>
    /// 累積點數
    /// </summary>
    public decimal AwardPoints { get; set; }

    /// <summary>
    /// 扣減點數
    /// </summary>
    public decimal RedeemPoints { get; set; }

    /// <summary>
    /// 取消標記
    /// </summary>
    public string? ReversalFlag { get; set; }

    /// <summary>
    /// 交易金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 作廢標記
    /// </summary>
    public string? VoidFlag { get; set; }

    /// <summary>
    /// 授權碼
    /// </summary>
    public string? AuthCode { get; set; }

    /// <summary>
    /// 強制日期
    /// </summary>
    public DateTime? ForceDate { get; set; }

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string? Invoice { get; set; }

    /// <summary>
    /// 交易類型 (2, 3, 4, 11, 13, 16, 18等)
    /// </summary>
    public string? TransType { get; set; }

    /// <summary>
    /// 交易類型代碼 (2, 3, 4, 5, 7, 8, 9等)
    /// </summary>
    public string? TxnType { get; set; }

    /// <summary>
    /// 交易時間
    /// </summary>
    public DateTime TransTime { get; set; }

    /// <summary>
    /// 交易狀態
    /// </summary>
    public string Status { get; set; } = "SUCCESS";

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

