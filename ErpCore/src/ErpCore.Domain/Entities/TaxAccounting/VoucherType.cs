namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 傳票型態實體 (SYST121-SYST122)
/// </summary>
public class VoucherType
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 型態代號
    /// </summary>
    public string VoucherId { get; set; } = string.Empty;

    /// <summary>
    /// 型態名稱
    /// </summary>
    public string VoucherName { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string? Status { get; set; } = "1";

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

