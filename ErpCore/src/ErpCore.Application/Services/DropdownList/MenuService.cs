using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.DropdownList;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 選單服務實作
/// </summary>
public class MenuService : BaseService, IMenuService
{
    private readonly IMenuRepository _repository;

    public MenuService(
        IMenuRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<MenuDto?> GetMenuAsync(string menuId)
    {
        try
        {
            _logger.LogInfo($"查詢選單: {menuId}");
            var menu = await _repository.GetByIdAsync(menuId);
            if (menu == null) return null;

            return new MenuDto
            {
                MenuId = menu.MenuId,
                MenuName = menu.MenuName,
                SystemId = menu.SystemId,
                ParentMenuId = menu.ParentMenuId,
                SeqNo = menu.SeqNo,
                Icon = menu.Icon,
                Url = menu.Url,
                Status = menu.Status
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢選單失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<MenuDto>> GetMenusAsync(MenuQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢選單列表");
            var result = await _repository.QueryAsync(query);

            return new PagedResult<MenuDto>
            {
                Items = result.Items.Select(m => new MenuDto
                {
                    MenuId = m.MenuId,
                    MenuName = m.MenuName,
                    SystemId = m.SystemId,
                    ParentMenuId = m.ParentMenuId,
                    SeqNo = m.SeqNo,
                    Icon = m.Icon,
                    Url = m.Url,
                    Status = m.Status
                }).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢選單列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<MenuOptionDto>> GetMenuOptionsAsync(string? systemId = null, string? status = "1")
    {
        try
        {
            _logger.LogInfo("查詢選單選項");
            return await _repository.GetOptionsAsync(systemId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢選單選項失敗", ex);
            throw;
        }
    }
}

