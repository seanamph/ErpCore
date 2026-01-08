namespace ErpCore.Domain.Entities.DropdownList;

/// <summary>
/// 城市資料實體 (ADDR_CITY_LIST)
/// </summary>
public class City
{
    /// <summary>
    /// 城市代碼
    /// </summary>
    public string CityId { get; set; } = string.Empty;

    /// <summary>
    /// 城市名稱
    /// </summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    /// 國家代碼
    /// </summary>
    public string? CountryCode { get; set; }

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

