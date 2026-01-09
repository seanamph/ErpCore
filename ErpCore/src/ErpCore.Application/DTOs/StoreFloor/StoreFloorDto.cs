namespace ErpCore.Application.DTOs.StoreFloor;

/// <summary>
/// 商店樓層 DTO (SYS6000 - 商店資料維護)
/// </summary>
public class ShopFloorDto
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
/// 商店樓層查詢 DTO
/// </summary>
public class ShopFloorQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ShopId { get; set; }
    public string? ShopName { get; set; }
    public string? ShopType { get; set; }
    public string? Status { get; set; }
    public string? City { get; set; }
    public string? FloorId { get; set; }
}

/// <summary>
/// 建立商店樓層 DTO
/// </summary>
public class CreateShopFloorDto
{
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
}

/// <summary>
/// 修改商店樓層 DTO
/// </summary>
public class UpdateShopFloorDto
{
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
}

/// <summary>
/// 更新商店狀態 DTO
/// </summary>
public class UpdateShopFloorStatusDto
{
    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";
}

