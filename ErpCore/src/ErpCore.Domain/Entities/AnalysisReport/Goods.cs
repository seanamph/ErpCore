namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 商品主檔 (對應舊系統 IMS_AM.NAM_GOODS)
/// </summary>
public class Goods
{
    /// <summary>
    /// 商品代碼
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string GoodsName { get; set; } = string.Empty;

    /// <summary>
    /// 大分類代碼
    /// </summary>
    public string? BId { get; set; }

    /// <summary>
    /// 中分類代碼
    /// </summary>
    public string? MId { get; set; }

    /// <summary>
    /// 小分類代碼
    /// </summary>
    public string? SId { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 包裝單位
    /// </summary>
    public string? PackUnit { get; set; }

    /// <summary>
    /// 安全庫存量
    /// </summary>
    public decimal SafeQty { get; set; }

    /// <summary>
    /// 類型 (1:耗材, 2:固定資產)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 資產類型
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// 商品開始日期
    /// </summary>
    public DateTime? GoodsBeginDate { get; set; }

    /// <summary>
    /// 資產日期
    /// </summary>
    public DateTime? AssetDate { get; set; }

    /// <summary>
    /// 持有單位
    /// </summary>
    public string? HoldOrgId { get; set; }

    /// <summary>
    /// 持有人員
    /// </summary>
    public string? HoldEmpId { get; set; }

    /// <summary>
    /// 使用單位
    /// </summary>
    public string? UseOrgId { get; set; }

    /// <summary>
    /// 使用人員
    /// </summary>
    public string? UseEmpId { get; set; }

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
}
