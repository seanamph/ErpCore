namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// POP列印設定 (SYSW170)
/// </summary>
public class PopPrintSetting
{
    /// <summary>
    /// 設定ID
    /// </summary>
    public Guid SettingId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 店別編號
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// IP位址
    /// </summary>
    public string? Ip { get; set; }

    /// <summary>
    /// 報表類型
    /// </summary>
    public string? TypeId { get; set; }

    /// <summary>
    /// 版本標記 (AP, UA, STANDARD)
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// 除錯模式
    /// </summary>
    public bool DebugMode { get; set; }

    /// <summary>
    /// 標題高度填補
    /// </summary>
    public int HeaderHeightPadding { get; set; }

    /// <summary>
    /// 標題高度填補剩餘
    /// </summary>
    public int HeaderHeightPaddingRemain { get; set; } = 851;

    /// <summary>
    /// 頁面標題高度填補
    /// </summary>
    public int PageHeaderHeightPadding { get; set; }

    /// <summary>
    /// 頁面填補 (左,右,上,下)
    /// </summary>
    public string? PagePadding { get; set; }

    /// <summary>
    /// 頁面大小 (高,寬)
    /// </summary>
    public string? PageSize { get; set; }

    /// <summary>
    /// AP版本專屬設定 (JSON格式)
    /// </summary>
    public string? ApSpecificSettings { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

