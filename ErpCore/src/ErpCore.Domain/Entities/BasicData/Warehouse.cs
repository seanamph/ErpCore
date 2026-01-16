namespace ErpCore.Domain.Entities.BasicData;

/// <summary>
/// 庫別資料實體 (SYSWB60)
/// </summary>
public class Warehouse
{
    /// <summary>
    /// 庫別代碼
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// 庫別名稱
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// 庫別類型
    /// </summary>
    public string? WarehouseType { get; set; }

    /// <summary>
    /// 庫別位置
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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
