namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 耗材主檔
/// </summary>
public class Consumable
{
    /// <summary>
    /// 耗材編號
    /// </summary>
    public string ConsumableId { get; set; } = string.Empty;

    /// <summary>
    /// 耗材名稱
    /// </summary>
    public string ConsumableName { get; set; } = string.Empty;

    /// <summary>
    /// 分類代碼
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 規格
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>
    /// 品牌
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// 型號
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 條碼
    /// </summary>
    public string? BarCode { get; set; }

    /// <summary>
    /// 狀態 (1:正常, 2:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 資產狀態
    /// </summary>
    public string? AssetStatus { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public decimal? Quantity { get; set; }

    /// <summary>
    /// 最小庫存量
    /// </summary>
    public decimal? MinQuantity { get; set; }

    /// <summary>
    /// 最大庫存量
    /// </summary>
    public decimal? MaxQuantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// 供應商代碼
    /// </summary>
    public string? SupplierId { get; set; }

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

