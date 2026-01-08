using ErpCore.Application.DTOs.Core;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 框架功能服務介面
/// </summary>
public interface IFrameworkService
{
    /// <summary>
    /// 查詢使用者選單樹
    /// </summary>
    Task<MenuTreeDto> GetMenuTreeAsync(string? sysId, string? searchKeyword);

    /// <summary>
    /// 搜尋選單
    /// </summary>
    Task<MenuSearchResultDto> SearchMenusAsync(string keyword, string? sysId);

    /// <summary>
    /// 查詢使用者收藏選單
    /// </summary>
    Task<List<MenuFavoriteDto>> GetFavoritesAsync(int? frameIndex, string? searchKeyword);

    /// <summary>
    /// 新增收藏選單
    /// </summary>
    Task<MenuFavoriteDto> AddFavoriteAsync(AddMenuFavoriteDto dto);

    /// <summary>
    /// 刪除收藏選單
    /// </summary>
    Task RemoveFavoriteAsync(long favoriteId);

    /// <summary>
    /// 更新收藏選單排序
    /// </summary>
    Task UpdateFavoriteSortAsync(UpdateFavoriteSortDto dto);

    /// <summary>
    /// 切換收藏選單框架
    /// </summary>
    Task SwitchFrameAsync(long favoriteId, int frameIndex);

    /// <summary>
    /// 查詢系統標題資訊
    /// </summary>
    Task<SystemHeaderInfoDto> GetHeaderInfoAsync();

    /// <summary>
    /// 查詢當前時間
    /// </summary>
    CurrentTimeDto GetCurrentTime();

    /// <summary>
    /// 查詢使用者可用的模組列表
    /// </summary>
    Task<List<ModuleDto>> GetUserModulesAsync();

    /// <summary>
    /// 查詢模組的子系統列表
    /// </summary>
    Task<List<SubsystemDto>> GetModuleSubsystemsAsync(string moduleId);

    /// <summary>
    /// 查詢訊息列表
    /// </summary>
    Task<PagedResult<MessageDto>> GetMessagesAsync(MessageQueryDto query);

    /// <summary>
    /// 查詢單筆訊息
    /// </summary>
    Task<MessageDto?> GetMessageAsync(string messageId);

    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    Task MarkAsReadAsync(string messageId);

    /// <summary>
    /// 批次標記訊息為已讀
    /// </summary>
    Task BatchMarkAsReadAsync(BatchReadDto dto);

    /// <summary>
    /// 查詢日期範圍內的訊息
    /// </summary>
    Task<List<MessageDto>> GetMessagesByDateAsync(DateTime? date, DateTime? startDate, DateTime? endDate);

    /// <summary>
    /// 查詢未讀訊息數量
    /// </summary>
    Task<UnreadCountDto> GetUnreadCountAsync();
}

