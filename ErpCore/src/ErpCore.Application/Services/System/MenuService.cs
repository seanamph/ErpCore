using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 子系統項目服務實作 (SYS0420)
/// </summary>
public class MenuService : BaseService, IMenuService
{
    private readonly IMenuRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public MenuService(
        IMenuRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<MenuDto>> GetMenusAsync(MenuQueryDto query)
    {
        try
        {
            var repositoryQuery = new MenuQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                MenuId = query.Filters?.MenuId,
                MenuName = query.Filters?.MenuName,
                SystemId = query.Filters?.SystemId,
                ParentMenuId = query.Filters?.ParentMenuId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 需要单独查询 SystemName 和 ParentMenuName
            var dtos = new List<MenuDto>();
            foreach (var item in result.Items)
            {
                var dto = new MenuDto
                {
                    TKey = item.TKey,
                    MenuId = item.MenuId,
                    MenuName = item.MenuName,
                    SeqNo = item.SeqNo,
                    SystemId = item.SystemId,
                    ParentMenuId = item.ParentMenuId,
                    Status = item.Status,
                    CreatedBy = item.CreatedBy,
                    CreatedAt = item.CreatedAt,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedAt = item.UpdatedAt
                };

                // 查询主系统名称
                if (!string.IsNullOrEmpty(item.SystemId))
                {
                    var systemName = await GetSystemNameAsync(item.SystemId);
                    dto.SystemName = systemName;
                }

                // 查询上层子系统名称
                if (!string.IsNullOrEmpty(item.ParentMenuId) && item.ParentMenuId != "0")
                {
                    var parentMenuName = await GetMenuNameAsync(item.ParentMenuId);
                    dto.ParentMenuName = parentMenuName;
                }

                dtos.Add(dto);
            }

            return new PagedResult<MenuDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢子系統列表失敗", ex);
            throw;
        }
    }

    public async Task<MenuDto> GetMenuAsync(string menuId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(menuId);
            if (entity == null)
            {
                throw new InvalidOperationException($"子系統不存在: {menuId}");
            }

            var dto = new MenuDto
            {
                TKey = entity.TKey,
                MenuId = entity.MenuId,
                MenuName = entity.MenuName,
                SeqNo = entity.SeqNo,
                SystemId = entity.SystemId,
                ParentMenuId = entity.ParentMenuId,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };

            // 查询主系统名称
            if (!string.IsNullOrEmpty(entity.SystemId))
            {
                dto.SystemName = await GetSystemNameAsync(entity.SystemId);
            }

            // 查询上层子系统名称
            if (!string.IsNullOrEmpty(entity.ParentMenuId) && entity.ParentMenuId != "0")
            {
                dto.ParentMenuName = await GetMenuNameAsync(entity.ParentMenuId);
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢子系統失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task<string> CreateMenuAsync(CreateMenuDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.MenuId);
            if (exists)
            {
                throw new InvalidOperationException($"子系統已存在: {dto.MenuId}");
            }

            // 檢查主系統是否存在
            var systemExists = await CheckSystemExistsAsync(dto.SystemId);
            if (!systemExists)
            {
                throw new InvalidOperationException($"主系統不存在: {dto.SystemId}");
            }

            // 檢查上層子系統是否存在（如果不是 '0'）
            if (!string.IsNullOrEmpty(dto.ParentMenuId) && dto.ParentMenuId != "0")
            {
                var parentExists = await _repository.ExistsAsync(dto.ParentMenuId);
                if (!parentExists)
                {
                    throw new InvalidOperationException($"上層子系統不存在: {dto.ParentMenuId}");
                }
            }

            // 檢查是否會造成循環引用
            if (dto.ParentMenuId == dto.MenuId)
            {
                throw new InvalidOperationException("上層子系統代碼不能等於自己");
            }

            var entity = new Menu
            {
                MenuId = dto.MenuId,
                MenuName = dto.MenuName,
                SeqNo = dto.SeqNo,
                SystemId = dto.SystemId,
                ParentMenuId = dto.ParentMenuId == "0" ? null : dto.ParentMenuId,
                Status = "1",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = null
            };

            await _repository.CreateAsync(entity);

            return entity.MenuId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增子系統失敗: {dto.MenuId}", ex);
            throw;
        }
    }

    public async Task UpdateMenuAsync(string menuId, UpdateMenuDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(menuId);
            if (entity == null)
            {
                throw new InvalidOperationException($"子系統不存在: {menuId}");
            }

            // 檢查主系統是否存在
            var systemExists = await CheckSystemExistsAsync(dto.SystemId);
            if (!systemExists)
            {
                throw new InvalidOperationException($"主系統不存在: {dto.SystemId}");
            }

            // 檢查上層子系統是否存在（如果不是 '0'）
            if (!string.IsNullOrEmpty(dto.ParentMenuId) && dto.ParentMenuId != "0")
            {
                var parentExists = await _repository.ExistsAsync(dto.ParentMenuId);
                if (!parentExists)
                {
                    throw new InvalidOperationException($"上層子系統不存在: {dto.ParentMenuId}");
                }
            }

            // 檢查是否會造成循環引用
            if (dto.ParentMenuId == menuId)
            {
                throw new InvalidOperationException("上層子系統代碼不能等於自己");
            }

            entity.MenuName = dto.MenuName;
            entity.SeqNo = dto.SeqNo;
            entity.SystemId = dto.SystemId;
            entity.ParentMenuId = dto.ParentMenuId == "0" ? null : dto.ParentMenuId;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改子系統失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task DeleteMenuAsync(string menuId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(menuId);
            if (entity == null)
            {
                throw new InvalidOperationException($"子系統不存在: {menuId}");
            }

            // 檢查是否有下層子系統
            var hasChildren = await _repository.HasChildrenAsync(menuId);
            if (hasChildren)
            {
                throw new InvalidOperationException("此子系統下存在下層子系統，無法刪除");
            }

            // 檢查是否有作業關聯
            var hasPrograms = await _repository.HasProgramsAsync(menuId);
            if (hasPrograms)
            {
                throw new InvalidOperationException("此子系統下存在作業，無法刪除");
            }

            await _repository.DeleteAsync(menuId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除子系統失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task DeleteMenusBatchAsync(BatchDeleteMenusDto dto)
    {
        try
        {
            foreach (var menuId in dto.MenuIds)
            {
                await DeleteMenuAsync(menuId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除子系統失敗", ex);
            throw;
        }
    }

    private async Task<string?> GetSystemNameAsync(string systemId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT SystemName
                FROM Systems
                WHERE SystemId = @SystemId";

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { SystemId = systemId });
            return result;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> GetMenuNameAsync(string menuId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT MenuName
                FROM Menus
                WHERE MenuId = @MenuId";

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { MenuId = menuId });
            return result;
        }
        catch
        {
            return null;
        }
    }

    private async Task<bool> CheckSystemExistsAsync(string systemId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT COUNT(*) FROM Systems 
                WHERE SystemId = @SystemId";

            var count = await connection.QuerySingleAsync<int>(sql, new { SystemId = systemId });
            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    private void ValidateCreateDto(CreateMenuDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.MenuId))
        {
            throw new ArgumentException("子系統項目代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.MenuName))
        {
            throw new ArgumentException("子系統項目名稱不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.SystemId))
        {
            throw new ArgumentException("主系統代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ParentMenuId))
        {
            throw new ArgumentException("上層子系統代碼不能為空");
        }
    }
}
