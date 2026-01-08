namespace ErpCore.Domain.Entities.BasicData;

/// <summary>
/// 銀行資料實體 (SYSBC20)
/// </summary>
public class Bank
{
    /// <summary>
    /// 銀行代號
    /// </summary>
    public string BankId { get; set; } = string.Empty;

    /// <summary>
    /// 銀行名稱
    /// </summary>
    public string BankName { get; set; } = string.Empty;

    /// <summary>
    /// 帳號最小長度
    /// </summary>
    public int? AcctLen { get; set; }

    /// <summary>
    /// 帳號最大長度
    /// </summary>
    public int? AcctLenMax { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 銀行種類 (1:銀行, 2:郵局, 3:信用合作社)
    /// </summary>
    public string? BankKind { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

