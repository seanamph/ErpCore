namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃主檔 (SYS8110-SYS8220)
/// </summary>
public class Lease
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 租賃編號
    /// </summary>
    public string LeaseId { get; set; } = string.Empty;

    /// <summary>
    /// 租戶代碼
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// 租戶名稱
    /// </summary>
    public string? TenantName { get; set; }

    /// <summary>
    /// 分店代碼
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 樓層代碼
    /// </summary>
    public string? FloorId { get; set; }

    /// <summary>
    /// 位置代碼
    /// </summary>
    public string? LocationId { get; set; }

    /// <summary>
    /// 租賃類型 (SYSE用)
    /// </summary>
    public string? LeaseType { get; set; }

    /// <summary>
    /// 版本號 (SYSE用)
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// 停車位代碼 (SYSM用)
    /// </summary>
    public string? ParkingSpaceId { get; set; }

    /// <summary>
    /// 停車費 (SYSM用)
    /// </summary>
    public decimal? ParkingFee { get; set; } = 0;

    /// <summary>
    /// 合同編號 (SYSM用)
    /// </summary>
    public string? ContractNo { get; set; }

    /// <summary>
    /// 合同日期 (SYSM用)
    /// </summary>
    public DateTime? ContractDate { get; set; }

    /// <summary>
    /// 租賃日期
    /// </summary>
    public DateTime LeaseDate { get; set; }

    /// <summary>
    /// 租期開始日
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 租期結束日
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 狀態 (D:草稿, S:已簽約, E:已生效, T:已終止)
    /// </summary>
    public string Status { get; set; } = "D";

    /// <summary>
    /// 月租金
    /// </summary>
    public decimal? MonthlyRent { get; set; } = 0;

    /// <summary>
    /// 總租金
    /// </summary>
    public decimal? TotalRent { get; set; } = 0;

    /// <summary>
    /// 押金
    /// </summary>
    public decimal? Deposit { get; set; } = 0;

    /// <summary>
    /// 幣別
    /// </summary>
    public string? CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 付款方式
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 付款日
    /// </summary>
    public int? PaymentDay { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

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

