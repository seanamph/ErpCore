namespace ErpCore.Application.DTOs.Core;

/// <summary>
/// 選單節點 DTO
/// </summary>
public class MenuNodeDto
{
    /// <summary>
    /// 選單ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 選單名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 選單類型 (SYSTEM/SUBSYSTEM/PROGRAM)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 圖示
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 子選單
    /// </summary>
    public List<MenuNodeDto> Children { get; set; } = new();
}

/// <summary>
/// 選單樹 DTO
/// </summary>
public class MenuTreeDto
{
    /// <summary>
    /// 選單列表
    /// </summary>
    public List<MenuNodeDto> Menus { get; set; } = new();
}

/// <summary>
/// 選單搜尋結果 DTO
/// </summary>
public class MenuSearchResultDto
{
    /// <summary>
    /// 搜尋結果
    /// </summary>
    public List<MenuSearchItemDto> Results { get; set; } = new();
}

/// <summary>
/// 選單搜尋項目 DTO
/// </summary>
public class MenuSearchItemDto
{
    /// <summary>
    /// 選單ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 選單名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 選單類型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 路徑
    /// </summary>
    public string Path { get; set; } = string.Empty;
}

/// <summary>
/// 選單收藏 DTO
/// </summary>
public class MenuFavoriteDto
{
    /// <summary>
    /// 收藏ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 選單ID
    /// </summary>
    public string MenuId { get; set; } = string.Empty;

    /// <summary>
    /// 選單名稱
    /// </summary>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 選單類型
    /// </summary>
    public string MenuType { get; set; } = string.Empty;

    /// <summary>
    /// URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 框架索引
    /// </summary>
    public int FrameIndex { get; set; }
}

/// <summary>
/// 新增收藏選單請求 DTO
/// </summary>
public class AddMenuFavoriteDto
{
    /// <summary>
    /// 選單ID
    /// </summary>
    public string MenuId { get; set; } = string.Empty;

    /// <summary>
    /// 選單類型
    /// </summary>
    public string MenuType { get; set; } = string.Empty;

    /// <summary>
    /// 框架索引
    /// </summary>
    public int FrameIndex { get; set; }
}

/// <summary>
/// 更新收藏選單排序 DTO
/// </summary>
public class UpdateFavoriteSortDto
{
    /// <summary>
    /// 收藏列表
    /// </summary>
    public List<FavoriteSortItemDto> Favorites { get; set; } = new();
}

/// <summary>
/// 收藏排序項目 DTO
/// </summary>
public class FavoriteSortItemDto
{
    /// <summary>
    /// 收藏ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// 系統標題資訊 DTO
/// </summary>
public class SystemHeaderInfoDto
{
    /// <summary>
    /// 專案標題
    /// </summary>
    public string ProjectTitle { get; set; } = string.Empty;

    /// <summary>
    /// 專案標題（英文）
    /// </summary>
    public string ProjectTitleEn { get; set; } = string.Empty;

    /// <summary>
    /// 版本號
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// 歡迎訊息
    /// </summary>
    public string Welcome { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Logo路徑
    /// </summary>
    public string LogoPath { get; set; } = string.Empty;

    /// <summary>
    /// 顯示版本號
    /// </summary>
    public bool ShowVersion { get; set; }

    /// <summary>
    /// 當前日期
    /// </summary>
    public DateTime CurrentDate { get; set; }

    /// <summary>
    /// 到期日
    /// </summary>
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    /// 顯示到期日
    /// </summary>
    public bool ShowExpireDate { get; set; }

    /// <summary>
    /// 是否為測試環境
    /// </summary>
    public bool IsTestEnvironment { get; set; }
}

/// <summary>
/// 當前時間 DTO
/// </summary>
public class CurrentTimeDto
{
    /// <summary>
    /// 當前時間
    /// </summary>
    public DateTime CurrentTime { get; set; }

    /// <summary>
    /// 時區
    /// </summary>
    public string Timezone { get; set; } = string.Empty;
}

/// <summary>
/// 模組 DTO
/// </summary>
public class ModuleDto
{
    /// <summary>
    /// 模組ID
    /// </summary>
    public string ModuleId { get; set; } = string.Empty;

    /// <summary>
    /// 模組名稱
    /// </summary>
    public string ModuleName { get; set; } = string.Empty;

    /// <summary>
    /// 模組名稱（英文）
    /// </summary>
    public string? ModuleNameEn { get; set; }

    /// <summary>
    /// 選中狀態圖片路徑
    /// </summary>
    public string? ImgOnUrl { get; set; }

    /// <summary>
    /// 未選中狀態圖片路徑
    /// </summary>
    public string? ImgOffUrl { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// 子系統 DTO
/// </summary>
public class SubsystemDto
{
    /// <summary>
    /// 系統ID
    /// </summary>
    public string SystemId { get; set; } = string.Empty;

    /// <summary>
    /// 系統名稱
    /// </summary>
    public string SystemName { get; set; } = string.Empty;

    /// <summary>
    /// 系統URL
    /// </summary>
    public string? SystemUrl { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// 訊息 DTO
/// </summary>
public class MessageDto
{
    /// <summary>
    /// 訊息ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// 標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 內容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 訊息類型 (SYSTEM/PERSONAL/ANNOUNCEMENT)
    /// </summary>
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// 優先級 (HIGH/NORMAL/LOW)
    /// </summary>
    public string Priority { get; set; } = "NORMAL";

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }
}

/// <summary>
/// 訊息查詢 DTO
/// </summary>
public class MessageQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 訊息類型
    /// </summary>
    public string? MessageType { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool? IsRead { get; set; }

    /// <summary>
    /// 關鍵字
    /// </summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 批次標記已讀 DTO
/// </summary>
public class BatchReadDto
{
    /// <summary>
    /// 訊息ID列表
    /// </summary>
    public List<string> MessageIds { get; set; } = new();
}

/// <summary>
/// 未讀數量 DTO
/// </summary>
public class UnreadCountDto
{
    /// <summary>
    /// 未讀數量
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 按類型分類的未讀數量
    /// </summary>
    public Dictionary<string, int> UnreadByType { get; set; } = new();
}

