namespace ErpCore.Domain.Entities.StandardModule;

/// <summary>
/// STD5000 會員積分明細實體 (SYS5210-SYS52A0 - 會員積分管理)
/// </summary>
public class Std5000MemberPoint
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 會員編號
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 交易日期
    /// </summary>
    public DateTime TransDate { get; set; }

    /// <summary>
    /// 交易類型 (EARN:獲得, USE:使用, EXPIRE:過期)
    /// </summary>
    public string TransType { get; set; } = string.Empty;

    /// <summary>
    /// 積分數量
    /// </summary>
    public decimal Points { get; set; }

    /// <summary>
    /// 交易單號
    /// </summary>
    public string? TransId { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

