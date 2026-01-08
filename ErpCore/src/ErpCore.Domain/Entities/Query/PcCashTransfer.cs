namespace ErpCore.Domain.Entities.Query;

/// <summary>
/// 零用金拋轉檔 (SYSQ230)
/// </summary>
public class PcCashTransfer
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 拋轉單號
    /// </summary>
    public string TransferId { get; set; } = string.Empty;

    /// <summary>
    /// 分店代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 拋轉日期
    /// </summary>
    public DateTime TransferDate { get; set; }

    /// <summary>
    /// 傳票編號
    /// </summary>
    public string? VoucherId { get; set; }

    /// <summary>
    /// 傳票種類
    /// </summary>
    public string? VoucherKind { get; set; }

    /// <summary>
    /// 傳票日期
    /// </summary>
    public DateTime? VoucherDate { get; set; }

    /// <summary>
    /// 拋轉金額
    /// </summary>
    public decimal TransferAmount { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string? TransferStatus { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? BUser { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime BTime { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? CUser { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? CTime { get; set; }
}

