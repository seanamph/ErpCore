namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 停車位資料 (SYSM111-SYSM138)
/// </summary>
public class ParkingSpace
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 停車位代碼
    /// </summary>
    public string ParkingSpaceId { get; set; } = string.Empty;

    /// <summary>
    /// 停車位編號
    /// </summary>
    public string? ParkingSpaceNo { get; set; }

    /// <summary>
    /// 分店代碼
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 樓層代碼
    /// </summary>
    public string? FloorId { get; set; }

    /// <summary>
    /// 面積
    /// </summary>
    public decimal? Area { get; set; }

    /// <summary>
    /// 狀態 (A:可用, U:使用中, M:維護中)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 租賃編號
    /// </summary>
    public string? LeaseId { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

