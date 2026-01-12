namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 商品分類表
/// </summary>
public class GoodsCategory
{
    /// <summary>
    /// 分類ID
    /// </summary>
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 分類名稱
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 分類類型 (B:大分類, M:中分類, S:小分類)
    /// </summary>
    public string CategoryType { get; set; } = string.Empty;

    /// <summary>
    /// 父分類ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 排序號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

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
