namespace ErpCore.Application.DTOs.StoreFloor;

/// <summary>
/// 樓層 DTO (SYS6310-SYS6370 - 樓層資料維護)
/// </summary>
public class FloorDto
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
    /// 狀態 (A:啟用, I:停用)
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

    /// <summary>
    /// 商店數量（查詢時使用）
    /// </summary>
    public int? ShopCount { get; set; }
}

/// <summary>
/// 樓層查詢 DTO
/// </summary>
public class FloorQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? FloorId { get; set; }
    public string? FloorName { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 建立樓層 DTO
/// </summary>
public class CreateFloorDto
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
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改樓層 DTO
/// </summary>
public class UpdateFloorDto
{
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
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";
}

/// <summary>
/// 更新樓層狀態 DTO
/// </summary>
public class UpdateFloorStatusDto
{
    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";
}

