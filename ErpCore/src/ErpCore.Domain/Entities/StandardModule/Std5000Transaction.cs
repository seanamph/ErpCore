namespace ErpCore.Domain.Entities.StandardModule;

/// <summary>
/// STD5000 交易主檔實體 (SYS5310-SYS53C6 - 交易管理)
/// </summary>
public class Std5000Transaction
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 交易單號
    /// </summary>
    public string TransId { get; set; } = string.Empty;

    /// <summary>
    /// 交易日期
    /// </summary>
    public DateTime TransDate { get; set; }

    /// <summary>
    /// 交易類型 (SALE:銷售, RETURN:退貨, ADJUST:調整)
    /// </summary>
    public string TransType { get; set; } = string.Empty;

    /// <summary>
    /// 會員編號
    /// </summary>
    public string? MemberId { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 積分
    /// </summary>
    public decimal Points { get; set; }

    /// <summary>
    /// 狀態 (A:正常, C:取消)
    /// </summary>
    public string Status { get; set; } = "A";

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

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

