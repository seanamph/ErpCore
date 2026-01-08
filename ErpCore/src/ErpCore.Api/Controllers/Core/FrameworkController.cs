using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Core;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Core;

/// <summary>
/// 框架功能控制器
/// 提供選單、標題、主選單、收藏、訊息等框架功能
/// </summary>
[Route("api/v1/core/framework")]
public class FrameworkController : BaseController
{
    private readonly IFrameworkService _service;

    public FrameworkController(
        IFrameworkService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region 選單功能 (FrameMenu)

    /// <summary>
    /// 查詢使用者選單樹
    /// </summary>
    [HttpGet("menus/tree")]
    public async Task<ActionResult<ApiResponse<MenuTreeDto>>> GetMenuTree(
        [FromQuery] string? sysId,
        [FromQuery] string? searchKeyword)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMenuTreeAsync(sysId, searchKeyword);
            return result;
        }, "查詢選單樹失敗");
    }

    /// <summary>
    /// 搜尋選單
    /// </summary>
    [HttpGet("menus/search")]
    public async Task<ActionResult<ApiResponse<MenuSearchResultDto>>> SearchMenus(
        [FromQuery] string keyword,
        [FromQuery] string? sysId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SearchMenusAsync(keyword, sysId);
            return result;
        }, "搜尋選單失敗");
    }

    /// <summary>
    /// 查詢使用者收藏選單
    /// </summary>
    [HttpGet("menus/favorites")]
    public async Task<ActionResult<ApiResponse<List<MenuFavoriteDto>>>> GetFavorites(
        [FromQuery] int? frameIndex,
        [FromQuery] string? searchKeyword)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetFavoritesAsync(frameIndex, searchKeyword);
            return result;
        }, "查詢收藏選單失敗");
    }

    /// <summary>
    /// 新增收藏選單
    /// </summary>
    [HttpPost("menus/favorites")]
    public async Task<ActionResult<ApiResponse<MenuFavoriteDto>>> AddFavorite(
        [FromBody] AddMenuFavoriteDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.AddFavoriteAsync(dto);
            return result;
        }, "新增收藏選單失敗");
    }

    /// <summary>
    /// 刪除收藏選單
    /// </summary>
    [HttpDelete("menus/favorites/{favoriteId}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveFavorite(long favoriteId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.RemoveFavoriteAsync(favoriteId);
        }, "刪除收藏選單失敗");
    }

    /// <summary>
    /// 更新收藏選單排序
    /// </summary>
    [HttpPut("menus/favorites/sort")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateFavoriteSort(
        [FromBody] UpdateFavoriteSortDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateFavoriteSortAsync(dto);
        }, "更新收藏選單排序失敗");
    }

    /// <summary>
    /// 切換收藏選單框架
    /// </summary>
    [HttpPost("menus/favorites/switch-frame")]
    public async Task<ActionResult<ApiResponse<object>>> SwitchFrame(
        [FromBody] SwitchFrameDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.SwitchFrameAsync(dto.FavoriteId, dto.FrameIndex);
        }, "切換收藏選單框架失敗");
    }

    #endregion

    #region 標題功能 (FrameHead)

    /// <summary>
    /// 查詢系統標題資訊
    /// </summary>
    [HttpGet("header-info")]
    public async Task<ActionResult<ApiResponse<SystemHeaderInfoDto>>> GetHeaderInfo()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetHeaderInfoAsync();
            return result;
        }, "查詢系統標題資訊失敗");
    }

    /// <summary>
    /// 查詢當前時間
    /// </summary>
    [HttpGet("current-time")]
    public ActionResult<ApiResponse<CurrentTimeDto>> GetCurrentTime()
    {
        return ExecuteAsync(() =>
        {
            var result = _service.GetCurrentTime();
            return Task.FromResult(result);
        }, "查詢當前時間失敗");
    }

    #endregion

    #region 主選單功能 (FrameMainMenu)

    /// <summary>
    /// 查詢使用者可用的模組列表
    /// </summary>
    [HttpGet("main-menu/modules")]
    public async Task<ActionResult<ApiResponse<List<ModuleDto>>>> GetUserModules()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUserModulesAsync();
            return result;
        }, "查詢模組列表失敗");
    }

    /// <summary>
    /// 查詢模組的子系統列表
    /// </summary>
    [HttpGet("main-menu/modules/{moduleId}/subsystems")]
    public async Task<ActionResult<ApiResponse<List<SubsystemDto>>>> GetModuleSubsystems(string moduleId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetModuleSubsystemsAsync(moduleId);
            return result;
        }, "查詢子系統列表失敗");
    }

    #endregion

    #region 訊息功能 (FrameMessage)

    /// <summary>
    /// 查詢訊息列表
    /// </summary>
    [HttpGet("messages")]
    public async Task<ActionResult<ApiResponse<PagedResult<MessageDto>>>> GetMessages(
        [FromQuery] MessageQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMessagesAsync(query);
            return result;
        }, "查詢訊息列表失敗");
    }

    /// <summary>
    /// 查詢單筆訊息
    /// </summary>
    [HttpGet("messages/{messageId}")]
    public async Task<ActionResult<ApiResponse<MessageDto>>> GetMessage(string messageId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMessageAsync(messageId);
            if (result == null)
            {
                throw new InvalidOperationException("訊息不存在");
            }
            return result;
        }, "查詢單筆訊息失敗");
    }

    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    [HttpPost("messages/{messageId}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(string messageId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.MarkAsReadAsync(messageId);
        }, "標記訊息為已讀失敗");
    }

    /// <summary>
    /// 批次標記訊息為已讀
    /// </summary>
    [HttpPost("messages/batch-read")]
    public async Task<ActionResult<ApiResponse<object>>> BatchMarkAsRead(
        [FromBody] BatchReadDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchMarkAsReadAsync(dto);
        }, "批次標記訊息為已讀失敗");
    }

    /// <summary>
    /// 查詢日期範圍內的訊息
    /// </summary>
    [HttpGet("messages/by-date")]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetMessagesByDate(
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMessagesByDateAsync(date, startDate, endDate);
            return result;
        }, "查詢日期範圍內的訊息失敗");
    }

    /// <summary>
    /// 查詢未讀訊息數量
    /// </summary>
    [HttpGet("messages/unread-count")]
    public async Task<ActionResult<ApiResponse<UnreadCountDto>>> GetUnreadCount()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUnreadCountAsync();
            return result;
        }, "查詢未讀訊息數量失敗");
    }

    #endregion
}

/// <summary>
/// 切換框架 DTO
/// </summary>
public class SwitchFrameDto
{
    /// <summary>
    /// 收藏ID
    /// </summary>
    public long FavoriteId { get; set; }

    /// <summary>
    /// 框架索引
    /// </summary>
    public int FrameIndex { get; set; }
}

