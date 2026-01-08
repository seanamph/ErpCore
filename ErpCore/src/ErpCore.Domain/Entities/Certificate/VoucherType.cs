namespace ErpCore.Domain.Entities.Certificate;

/// <summary>
/// 憑證類型設定 (SYSK120)
/// </summary>
public class VoucherType
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 憑證類型代碼
    /// </summary>
    public string VoucherTypeId { get; set; } = string.Empty;

    /// <summary>
    /// 憑證類型名稱
    /// </summary>
    public string VoucherTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
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

