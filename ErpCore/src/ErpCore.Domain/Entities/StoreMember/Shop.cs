namespace ErpCore.Domain.Entities.StoreMember;

/// <summary>
/// 商店實體 (SYS3000 - 商店資料維護)
/// </summary>
public class Shop
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 商店編號
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 商店名稱
    /// </summary>
    public string ShopName { get; set; } = string.Empty;

    /// <summary>
    /// 商店英文名稱
    /// </summary>
    public string? ShopNameEn { get; set; }

    /// <summary>
    /// 商店類型
    /// </summary>
    public string? ShopType { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? Zone { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 傳真
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 店長姓名
    /// </summary>
    public string? ManagerName { get; set; }

    /// <summary>
    /// 店長電話
    /// </summary>
    public string? ManagerPhone { get; set; }

    /// <summary>
    /// 開店日期
    /// </summary>
    public DateTime? OpenDate { get; set; }

    /// <summary>
    /// 關店日期
    /// </summary>
    public DateTime? CloseDate { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 樓層代碼
    /// </summary>
    public string? FloorId { get; set; }

    /// <summary>
    /// 區域代碼
    /// </summary>
    public string? AreaId { get; set; }

    /// <summary>
    /// POS啟用
    /// </summary>
    public bool? PosEnabled { get; set; }

    /// <summary>
    /// POS系統代碼
    /// </summary>
    public string? PosSystemId { get; set; }

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
/// 商店類型實體
/// </summary>
public class ShopType
{
    /// <summary>
    /// 商店類型編號
    /// </summary>
    public string ShopTypeId { get; set; } = string.Empty;

    /// <summary>
    /// 商店類型名稱
    /// </summary>
    public string ShopTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 商店類型英文名稱
    /// </summary>
    public string? ShopTypeNameEn { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

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

/// <summary>
/// 商店POS設定實體
/// </summary>
public class ShopPosSetting
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 商店編號
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// POS系統代碼
    /// </summary>
    public string PosSystemId { get; set; } = string.Empty;

    /// <summary>
    /// POS終端機代碼
    /// </summary>
    public string? PosTerminalId { get; set; }

    /// <summary>
    /// POS設定 (JSON格式)
    /// </summary>
    public string? PosConfig { get; set; }

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

