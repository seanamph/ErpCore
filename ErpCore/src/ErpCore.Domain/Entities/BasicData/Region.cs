namespace ErpCore.Domain.Entities.BasicData;

/// <summary>
/// 地區資料實體 (SYSBC30)
/// </summary>
public class Region
{
    /// <summary>
    /// 地區編號
    /// </summary>
    public string RegionId { get; set; } = string.Empty;

    /// <summary>
    /// 地區名稱
    /// </summary>
    public string RegionName { get; set; } = string.Empty;

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

