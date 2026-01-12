namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者總公司/分店權限實體 (SYS0113)
/// </summary>
public class UserShop
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 總公司代號
    /// </summary>
    public string? PShopId { get; set; }

    /// <summary>
    /// 分店代號
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 據點代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
