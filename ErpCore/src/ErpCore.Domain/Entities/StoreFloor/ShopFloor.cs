namespace ErpCore.Domain.Entities.StoreFloor;

/// <summary>
/// 商店樓層管理實體 (SYS6000 - 商店資料維護)
/// </summary>
public class ShopFloor
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
    /// 樓層代碼
    /// </summary>
    public string? FloorId { get; set; }

    /// <summary>
    /// 樓層名稱
    /// </summary>
    public string? FloorName { get; set; }

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
    /// POS啟用
    /// </summary>
    public bool? PosEnabled { get; set; }

    /// <summary>
    /// POS系統代碼
    /// </summary>
    public string? PosSystemId { get; set; }

    /// <summary>
    /// POS終端代碼
    /// </summary>
    public string? PosTerminalId { get; set; }

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
/// 樓層實體
/// </summary>
public class Floor
{
    /// <summary>
    /// 樓層代碼
    /// </summary>
    public string FloorId { get; set; } = string.Empty;

    /// <summary>
    /// 樓層名稱
    /// </summary>
    public string FloorName { get; set; } = string.Empty;

    /// <summary>
    /// 樓層英文名稱
    /// </summary>
    public string? FloorNameEn { get; set; }

    /// <summary>
    /// 樓層號碼
    /// </summary>
    public int? FloorNumber { get; set; }

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
/// 類型代碼實體
/// </summary>
public class TypeCode
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 類型代碼
    /// </summary>
    public string TypeCodeValue { get; set; } = string.Empty;

    /// <summary>
    /// 類型名稱
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 類型英文名稱
    /// </summary>
    public string? TypeNameEn { get; set; }

    /// <summary>
    /// 分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int? SortOrder { get; set; }

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

