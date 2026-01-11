using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色權限服務實作 (SYS0310)
/// </summary>
public class RolePermissionService : BaseService, IRolePermissionService
{
    private readonly IRolePermissionRepository _repository;
    private readonly IRoleRepository _roleRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public RolePermissionService(
        IRolePermissionRepository repository,
        IRoleRepository roleRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _roleRepository = roleRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<RolePermissionDto>> GetRolePermissionsAsync(string roleId, RolePermissionQueryDto query)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var repositoryQuery = new RolePermissionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SystemId = query.SystemId,
                SubSystemId = query.SubSystemId,
                ProgramId = query.ProgramId,
                ButtonId = query.ButtonId
            };

            var result = await _repository.QueryPermissionsAsync(roleId, repositoryQuery);

            var dtos = result.Items.Select(x => new RolePermissionDto
            {
                TKey = x.TKey,
                RoleId = x.RoleId,
                ButtonId = x.ButtonId,
                SystemId = x.SystemId,
                SystemName = x.SystemName,
                SubSystemId = x.SubSystemId,
                SubSystemName = x.SubSystemName,
                ProgramId = x.ProgramId,
                ProgramName = x.ProgramName,
                ButtonName = x.ButtonName,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<RolePermissionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色權限列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RolePermissionStatsDto>> GetSystemStatsAsync(string roleId)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var stats = await _repository.GetSystemStatsAsync(roleId);
            return stats.Select(x => new RolePermissionStatsDto
            {
                SystemId = x.SystemId,
                SystemName = x.SystemName,
                TotalButtons = x.TotalButtons,
                AuthorizedButtons = x.AuthorizedButtons,
                IsFullyAuthorized = x.IsFullyAuthorized,
                AuthorizedRate = x.AuthorizedRate
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色權限統計失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> CreateRolePermissionsAsync(string roleId, CreateRolePermissionDto dto)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            if (dto.ButtonIds == null || dto.ButtonIds.Count == 0)
            {
                throw new InvalidOperationException("按鈕代碼列表不能為空");
            }

            var addedCount = await _repository.BatchCreateAsync(roleId, dto.ButtonIds, GetCurrentUserId());

            return new BatchOperationResult
            {
                UpdatedCount = addedCount,
                SkippedCount = dto.ButtonIds.Count - addedCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增角色權限失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchSetSystemPermissionsAsync(string roleId, BatchSetRoleSystemPermissionDto dto)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();

            foreach (var item in dto.SystemPermissions)
            {
                var count = await _repository.SetPermissionsBySystemAsync(roleId, item.SystemId, item.IsAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定角色系統權限失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchSetMenuPermissionsAsync(string roleId, BatchSetRoleMenuPermissionDto dto)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();

            foreach (var item in dto.MenuPermissions)
            {
                var count = await _repository.SetPermissionsBySubSystemAsync(roleId, item.SubSystemId, item.IsAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定角色選單權限失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchSetProgramPermissionsAsync(string roleId, BatchSetRoleProgramPermissionDto dto)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();

            foreach (var item in dto.ProgramPermissions)
            {
                var count = await _repository.SetPermissionsByProgramAsync(roleId, item.ProgramId, item.IsAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定角色作業權限失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchSetButtonPermissionsAsync(string roleId, BatchSetRoleButtonPermissionDto dto)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var buttonIds = dto.ButtonPermissions
                .Where(x => x.IsAuthorized)
                .Select(x => x.ButtonId)
                .ToList();

            var addedCount = 0;
            if (buttonIds.Count > 0)
            {
                addedCount = await _repository.BatchCreateAsync(roleId, buttonIds, GetCurrentUserId());
            }

            // 刪除未授權的按鈕
            var removeButtonIds = dto.ButtonPermissions
                .Where(x => !x.IsAuthorized)
                .Select(x => x.ButtonId)
                .ToList();

            var removedCount = 0;
            if (removeButtonIds.Count > 0)
            {
                // 查詢需要刪除的 TKey
                var permissions = await _repository.QueryPermissionsAsync(roleId, new RolePermissionQuery
                {
                    PageIndex = 1,
                    PageSize = int.MaxValue
                });

                var tKeysToDelete = permissions.Items
                    .Where(x => removeButtonIds.Contains(x.ButtonId))
                    .Select(x => x.TKey)
                    .ToList();

                if (tKeysToDelete.Count > 0)
                {
                    removedCount = await _repository.BatchDeleteAsync(tKeysToDelete);
                }
            }

            return new BatchOperationResult
            {
                UpdatedCount = addedCount + removedCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定角色按鈕權限失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task DeleteRolePermissionAsync(string roleId, long tKey)
    {
        try
        {
            // 檢查權限是否存在
            var permission = await _repository.GetByIdAsync(tKey);
            if (permission == null)
            {
                throw new InvalidOperationException($"角色權限不存在: {tKey}");
            }

            if (permission.RoleId != roleId)
            {
                throw new InvalidOperationException($"權限不屬於該角色: {roleId}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除角色權限失敗: {roleId} - {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchDeleteRolePermissionsAsync(string roleId, BatchDeleteRolePermissionDto dto)
    {
        try
        {
            if (dto.TKeys == null || dto.TKeys.Count == 0)
            {
                return new BatchOperationResult();
            }

            // 驗證所有權限都屬於該角色
            foreach (var tKey in dto.TKeys)
            {
                var permission = await _repository.GetByIdAsync(tKey);
                if (permission == null)
                {
                    throw new InvalidOperationException($"角色權限不存在: {tKey}");
                }

                if (permission.RoleId != roleId)
                {
                    throw new InvalidOperationException($"權限不屬於該角色: {roleId}");
                }
            }

            var deletedCount = await _repository.BatchDeleteAsync(dto.TKeys);

            return new BatchOperationResult
            {
                UpdatedCount = deletedCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量刪除角色權限失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RoleSystemListDto>> GetRoleSystemsAsync(string roleId)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var systems = await _repository.GetRoleSystemsAsync(roleId);
            return systems.Select(x => new RoleSystemListDto
            {
                SystemId = x.SystemId,
                SystemName = x.SystemName,
                TotalButtons = x.TotalButtons,
                AuthorizedButtons = x.AuthorizedButtons,
                IsFullyAuthorized = x.IsFullyAuthorized,
                AuthorizedRate = x.AuthorizedRate
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色系統列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RoleMenuListDto>> GetRoleMenusAsync(string roleId, string? systemId = null)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var menus = await _repository.GetRoleMenusAsync(roleId, systemId);
            return menus.Select(x => new RoleMenuListDto
            {
                MenuId = x.MenuId,
                MenuName = x.MenuName,
                SystemId = x.SystemId,
                TotalButtons = x.TotalButtons,
                AuthorizedButtons = x.AuthorizedButtons,
                IsFullyAuthorized = x.IsFullyAuthorized
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色選單列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RoleProgramListDto>> GetRoleProgramsAsync(string roleId, string? menuId = null)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var programs = await _repository.GetRoleProgramsAsync(roleId, menuId);
            return programs.Select(x => new RoleProgramListDto
            {
                ProgramId = x.ProgramId,
                ProgramName = x.ProgramName,
                MenuId = x.MenuId,
                TotalButtons = x.TotalButtons,
                AuthorizedButtons = x.AuthorizedButtons,
                IsFullyAuthorized = x.IsFullyAuthorized
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色作業列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RoleButtonListDto>> GetRoleButtonsAsync(string roleId, string? programId = null)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var buttons = await _repository.GetRoleButtonsAsync(roleId, programId);
            return buttons.Select(x => new RoleButtonListDto
            {
                ButtonId = x.ButtonId,
                ButtonName = x.ButtonName,
                ProgramId = x.ProgramId,
                Funs = x.Funs,
                PageId = x.PageId,
                IsAuthorized = x.IsAuthorized
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色按鈕列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<RolePermissionDto> UpdateRolePermissionAsync(string roleId, long tKey, UpdateRolePermissionDto dto)
    {
        try
        {
            // 檢查權限是否存在
            var permission = await _repository.GetByIdAsync(tKey);
            if (permission == null)
            {
                throw new InvalidOperationException($"角色權限不存在: {tKey}");
            }

            if (permission.RoleId != roleId)
            {
                throw new InvalidOperationException($"權限不屬於該角色: {roleId}");
            }

            // 檢查新的按鈕是否存在
            if (string.IsNullOrEmpty(dto.ButtonId))
            {
                throw new InvalidOperationException("按鈕代碼不能為空");
            }

            // 更新權限
            permission.ButtonId = dto.ButtonId;
            var updated = await _repository.UpdateAsync(permission);

            // 查詢更新後的完整資訊
            var permissions = await _repository.QueryPermissionsAsync(roleId, new RolePermissionQuery
            {
                PageIndex = 1,
                PageSize = 1
            });

            var result = permissions.Items.FirstOrDefault(x => x.TKey == updated.TKey);
            if (result == null)
            {
                throw new InvalidOperationException("查詢更新後的權限失敗");
            }

            return new RolePermissionDto
            {
                TKey = result.TKey,
                RoleId = result.RoleId,
                ButtonId = result.ButtonId,
                SystemId = result.SystemId,
                SystemName = result.SystemName,
                SubSystemId = result.SubSystemId,
                SubSystemName = result.SubSystemName,
                ProgramId = result.ProgramId,
                ProgramName = result.ProgramName,
                ButtonName = result.ButtonName,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改角色權限失敗: {roleId} - {tKey}", ex);
            throw;
        }
    }
}

