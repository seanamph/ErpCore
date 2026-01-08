using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 項目權限服務實作 (SYS0360)
/// </summary>
public class ItemPermissionService : BaseService, IItemPermissionService
{
    private readonly IItemPermissionRepository _repository;
    private readonly IItemCorrespondRepository _itemCorrespondRepository;

    public ItemPermissionService(
        IItemPermissionRepository repository,
        IItemCorrespondRepository itemCorrespondRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _itemCorrespondRepository = itemCorrespondRepository;
    }

    public async Task<PagedResult<ItemPermissionDto>> GetItemPermissionsAsync(string itemId, ItemPermissionQueryDto query)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var repositoryQuery = new ItemPermissionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SystemId = query.SystemId,
                MenuId = query.MenuId,
                ProgramId = query.ProgramId,
                ButtonKey = query.ButtonKey
            };

            var result = await _repository.QueryPermissionsAsync(itemId, repositoryQuery);

            var dtos = result.Items.Select(x => new ItemPermissionDto
            {
                TKey = x.TKey,
                ItemId = x.ItemId,
                ProgramId = x.ProgramId,
                PageId = x.PageId,
                ButtonId = x.ButtonId,
                ButtonKey = x.ButtonKey,
                SystemId = x.SystemId,
                SystemName = x.SystemName,
                SubSystemId = x.SubSystemId,
                SubSystemName = x.SubSystemName,
                ProgramName = x.ProgramName,
                ButtonName = x.ButtonName,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ItemPermissionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目權限列表失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<List<ItemSystemDto>> GetSystemListAsync(string itemId)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var result = await _repository.GetSystemListAsync(itemId);
            return result.Select(x => new ItemSystemDto
            {
                SystemId = x.SystemId,
                SystemName = x.SystemName,
                PermissionCount = x.PermissionCount,
                TotalCount = x.TotalCount,
                Status = x.Status
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目系統列表失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<List<ItemMenuDto>> GetMenuListAsync(string itemId, string systemId)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var result = await _repository.GetMenuListAsync(itemId, systemId);
            return result.Select(x => new ItemMenuDto
            {
                MenuId = x.MenuId,
                MenuName = x.MenuName,
                PermissionCount = x.PermissionCount,
                TotalCount = x.TotalCount,
                Status = x.Status
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目選單列表失敗: {itemId} - {systemId}", ex);
            throw;
        }
    }

    public async Task<List<ItemProgramDto>> GetProgramListAsync(string itemId, string menuId)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var result = await _repository.GetProgramListAsync(itemId, menuId);
            return result.Select(x => new ItemProgramDto
            {
                ProgramId = x.ProgramId,
                ProgramName = x.ProgramName,
                PermissionCount = x.PermissionCount,
                TotalCount = x.TotalCount,
                Status = x.Status
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目作業列表失敗: {itemId} - {menuId}", ex);
            throw;
        }
    }

    public async Task<List<ItemButtonDto>> GetButtonListAsync(string itemId, string programId)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var result = await _repository.GetButtonListAsync(itemId, programId);
            return result.Select(x => new ItemButtonDto
            {
                ButtonKey = x.ButtonKey,
                ButtonId = x.ButtonId,
                ButtonName = x.ButtonName,
                IsAuthorized = x.IsAuthorized
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目按鈕列表失敗: {itemId} - {programId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> SetSystemPermissionsAsync(string itemId, SetItemSystemPermissionDto dto)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();
            var isAuthorized = dto.Action.ToLower() == "grant";

            foreach (var systemId in dto.SystemIds)
            {
                var count = await _repository.SetPermissionsBySystemAsync(itemId, systemId, isAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定項目系統權限失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> SetMenuPermissionsAsync(string itemId, SetItemMenuPermissionDto dto)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();
            var isAuthorized = dto.Action.ToLower() == "grant";

            foreach (var menuId in dto.MenuIds)
            {
                var count = await _repository.SetPermissionsByMenuAsync(itemId, menuId, isAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定項目選單權限失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> SetProgramPermissionsAsync(string itemId, SetItemProgramPermissionDto dto)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();
            var isAuthorized = dto.Action.ToLower() == "grant";

            foreach (var programId in dto.ProgramIds)
            {
                var count = await _repository.SetPermissionsByProgramAsync(itemId, programId, isAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定項目作業權限失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> SetButtonPermissionsAsync(string itemId, SetItemButtonPermissionDto dto)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var createdBy = GetCurrentUserId();
            var isAuthorized = dto.Action.ToLower() == "grant";

            var count = await _repository.SetPermissionsByButtonAsync(itemId, dto.ButtonKeys, isAuthorized, createdBy);

            return new BatchOperationResult
            {
                UpdatedCount = count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定項目按鈕權限失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task DeleteItemPermissionAsync(string itemId, long tKey)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var permission = await _repository.GetByIdAsync(tKey);
            if (permission == null || permission.ItemId != itemId)
            {
                throw new InvalidOperationException($"項目權限不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除項目權限失敗: {itemId} - {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchDeleteItemPermissionsAsync(string itemId, List<long> tKeys)
    {
        try
        {
            // 檢查項目是否存在
            var item = await _itemCorrespondRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                throw new InvalidOperationException($"項目不存在: {itemId}");
            }

            var count = await _repository.BatchDeleteAsync(tKeys);

            return new BatchOperationResult
            {
                UpdatedCount = count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量刪除項目權限失敗: {itemId}", ex);
            throw;
        }
    }
}

