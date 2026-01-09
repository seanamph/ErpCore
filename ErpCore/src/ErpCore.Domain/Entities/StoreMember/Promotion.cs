namespace ErpCore.Domain.Entities.StoreMember;

/// <summary>
/// 促銷活動實體 (SYS3000 - 促銷活動維護)
/// </summary>
public class Promotion
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 促銷活動編號
    /// </summary>
    public string PromotionId { get; set; } = string.Empty;

    /// <summary>
    /// 促銷活動名稱
    /// </summary>
    public string PromotionName { get; set; } = string.Empty;

    /// <summary>
    /// 促銷類型
    /// </summary>
    public string? PromotionType { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 折扣類型 (PERCENT:百分比, AMOUNT:金額)
    /// </summary>
    public string? DiscountType { get; set; }

    /// <summary>
    /// 折扣值
    /// </summary>
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// 最低消費金額
    /// </summary>
    public decimal MinPurchaseAmount { get; set; }

    /// <summary>
    /// 最高折扣金額
    /// </summary>
    public decimal? MaxDiscountAmount { get; set; }

    /// <summary>
    /// 適用商店 (JSON格式)
    /// </summary>
    public string? ApplicableShops { get; set; }

    /// <summary>
    /// 適用商品 (JSON格式)
    /// </summary>
    public string? ApplicableProducts { get; set; }

    /// <summary>
    /// 適用會員等級 (JSON格式)
    /// </summary>
    public string? ApplicableMemberLevels { get; set; }

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
}

/// <summary>
/// 商店報表實體
/// </summary>
public class StoreReport
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 報表編號
    /// </summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>
    /// 報表類型
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 商店編號
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 報表日期
    /// </summary>
    public DateTime ReportDate { get; set; }

    /// <summary>
    /// 報表資料 (JSON格式)
    /// </summary>
    public string? ReportData { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "A";

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

