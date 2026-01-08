namespace ErpCore.Domain.Entities.DropdownList;

/// <summary>
/// 區域資料實體 (ADDR_ZONE_LIST)
/// </summary>
public class Zone
{
    /// <summary>
    /// 區域代碼
    /// </summary>
    public string ZoneId { get; set; } = string.Empty;

    /// <summary>
    /// 區域名稱
    /// </summary>
    public string ZoneName { get; set; } = string.Empty;

    /// <summary>
    /// 城市代碼
    /// </summary>
    public string? CityId { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

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

