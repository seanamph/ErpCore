namespace ErpCore.Domain.Entities.Accounting;

/// <summary>
/// 資產實體 (SYSN310)
/// </summary>
public class Asset
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 資產編號
    /// </summary>
    public string AssetId { get; set; } = string.Empty;

    /// <summary>
    /// 資產名稱
    /// </summary>
    public string AssetName { get; set; } = string.Empty;

    /// <summary>
    /// 資產類別
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// 取得日期
    /// </summary>
    public DateTime? AcquisitionDate { get; set; }

    /// <summary>
    /// 取得成本
    /// </summary>
    public decimal? AcquisitionCost { get; set; }

    /// <summary>
    /// 折舊方法
    /// </summary>
    public string? DepreciationMethod { get; set; }

    /// <summary>
    /// 使用年限
    /// </summary>
    public int? UsefulLife { get; set; }

    /// <summary>
    /// 殘值
    /// </summary>
    public decimal? ResidualValue { get; set; }

    /// <summary>
    /// 狀態 (A:使用中, D:已處分)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 存放地點
    /// </summary>
    public string? Location { get; set; }

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

