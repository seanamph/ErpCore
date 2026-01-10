namespace ErpCore.Domain.Entities.Recruitment;

/// <summary>
/// 租戶位置資料實體 (SYSC999)
/// </summary>
public class TenantLocation
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 租戶主檔主鍵 (AGM_T_KEY)
    /// </summary>
    public long AgmTKey { get; set; }

    /// <summary>
    /// 位置代碼 (LOCATION_ID)
    /// </summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>
    /// 區域代碼 (AREA_ID)
    /// </summary>
    public string? AreaId { get; set; }

    /// <summary>
    /// 樓層代碼 (FLOOR_ID)
    /// </summary>
    public string? FloorId { get; set; }

    /// <summary>
    /// 狀態 (STATUS) 1:啟用, 0:停用
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 備註 (NOTES)
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者 (BUSER)
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間 (BTIME)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者 (CUSER)
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間 (CTIME)
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 建立者等級 (CPRIORITY)
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組 (CGROUP)
    /// </summary>
    public string? CreatedGroup { get; set; }
}

