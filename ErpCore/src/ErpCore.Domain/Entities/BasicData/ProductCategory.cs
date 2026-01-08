namespace ErpCore.Domain.Entities.BasicData;

/// <summary>
/// 商品分類資料實體 (SYSB110)
/// </summary>
public class ProductCategory
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 分類代碼
    /// </summary>
    public string ClassId { get; set; } = string.Empty;

    /// <summary>
    /// 分類名稱
    /// </summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// 分類型式 (1:資料, 2:耗材)
    /// </summary>
    public string? ClassType { get; set; }

    /// <summary>
    /// 分類區分 (1:大分類, 2:中分類, 3:小分類)
    /// </summary>
    public string ClassMode { get; set; } = string.Empty;

    /// <summary>
    /// 大分類代碼
    /// </summary>
    public string? BClassId { get; set; }

    /// <summary>
    /// 中分類代碼
    /// </summary>
    public string? MClassId { get; set; }

    /// <summary>
    /// 父分類主鍵
    /// </summary>
    public long? ParentTKey { get; set; }

    /// <summary>
    /// 所屬會計科目(借)
    /// </summary>
    public string? StypeId { get; set; }

    /// <summary>
    /// 所屬會計科目(貸)
    /// </summary>
    public string? StypeId2 { get; set; }

    /// <summary>
    /// 折舊科目(借)
    /// </summary>
    public string? DepreStypeId { get; set; }

    /// <summary>
    /// 累計折舊科目(貸)
    /// </summary>
    public string? DepreStypeId2 { get; set; }

    /// <summary>
    /// 進項稅額科目(借)
    /// </summary>
    public string? StypeTax { get; set; }

    /// <summary>
    /// 所屬項目個數
    /// </summary>
    public int? ItemCount { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string? Status { get; set; }

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

