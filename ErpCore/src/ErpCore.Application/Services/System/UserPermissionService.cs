using System.Data;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者權限服務實作 (SYS0320)
/// </summary>
public class UserPermissionService : BaseService, IUserPermissionService
{
    private readonly IUserPermissionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public UserPermissionService(
        IUserPermissionRepository repository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<UserPermissionDto>> GetUserPermissionsAsync(string userId, UserPermissionQueryDto query)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            var repositoryQuery = new UserPermissionQuery
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

            var result = await _repository.QueryPermissionsAsync(userId, repositoryQuery);

            var dtos = result.Items.Select(x => new UserPermissionDto
            {
                TKey = x.TKey,
                UserId = x.UserId,
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

            return new PagedResult<UserPermissionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者權限列表失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<List<UserPermissionStatsDto>> GetSystemStatsAsync(string userId)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            var stats = await _repository.GetSystemStatsAsync(userId);
            return stats.Select(x => new UserPermissionStatsDto
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
            _logger.LogError($"查詢使用者權限統計失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> CreateUserPermissionsAsync(string userId, CreateUserPermissionDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            if (dto.ButtonIds == null || dto.ButtonIds.Count == 0)
            {
                throw new InvalidOperationException("按鈕代碼列表不能為空");
            }

            var clearedRoleCount = 0;

            // 清除使用者角色權限對應（如果設定為清除）
            if (dto.ClearRolePermissions)
            {
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
                var userRoleList = userRoles.ToList();
                if (userRoleList.Count > 0)
                {
                    await _userRoleRepository.DeleteRangeAsync(userRoleList);
                    clearedRoleCount = userRoleList.Count;
                    _logger.LogInfo($"清除使用者角色權限對應: {userId}, 清除數量: {clearedRoleCount}");
                }
            }

            // 新增使用者權限
            var addedCount = await _repository.BatchCreateAsync(userId, dto.ButtonIds, GetCurrentUserId());

            return new BatchOperationResult
            {
                UpdatedCount = addedCount,
                SkippedCount = dto.ButtonIds.Count - addedCount,
                ClearedRoleCount = clearedRoleCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchSetSystemPermissionsAsync(string userId, BatchSetUserSystemPermissionDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            var clearedRoleCount = 0;

            // 清除使用者角色權限對應（如果設定為清除）
            if (dto.ClearRolePermissions)
            {
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
                var userRoleList = userRoles.ToList();
                if (userRoleList.Count > 0)
                {
                    await _userRoleRepository.DeleteRangeAsync(userRoleList);
                    clearedRoleCount = userRoleList.Count;
                    _logger.LogInfo($"清除使用者角色權限對應: {userId}, 清除數量: {clearedRoleCount}");
                }
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();

            foreach (var item in dto.SystemPermissions)
            {
                var count = await _repository.SetPermissionsBySystemAsync(userId, item.SystemId, item.IsAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated,
                ClearedRoleCount = clearedRoleCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定使用者系統權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchSetMenuPermissionsAsync(string userId, BatchSetUserMenuPermissionDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            var clearedRoleCount = 0;

            // 清除使用者角色權限對應（如果設定為清除）
            if (dto.ClearRolePermissions)
            {
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
                var userRoleList = userRoles.ToList();
                if (userRoleList.Count > 0)
                {
                    await _userRoleRepository.DeleteRangeAsync(userRoleList);
                    clearedRoleCount = userRoleList.Count;
                    _logger.LogInfo($"清除使用者角色權限對應: {userId}, 清除數量: {clearedRoleCount}");
                }
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();

            foreach (var item in dto.MenuPermissions)
            {
                var count = await _repository.SetPermissionsBySubSystemAsync(userId, item.SubSystemId, item.IsAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated,
                ClearedRoleCount = clearedRoleCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定使用者選單權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchSetProgramPermissionsAsync(string userId, BatchSetUserProgramPermissionDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            var clearedRoleCount = 0;

            // 清除使用者角色權限對應（如果設定為清除）
            if (dto.ClearRolePermissions)
            {
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
                var userRoleList = userRoles.ToList();
                if (userRoleList.Count > 0)
                {
                    await _userRoleRepository.DeleteRangeAsync(userRoleList);
                    clearedRoleCount = userRoleList.Count;
                    _logger.LogInfo($"清除使用者角色權限對應: {userId}, 清除數量: {clearedRoleCount}");
                }
            }

            var totalUpdated = 0;
            var createdBy = GetCurrentUserId();

            foreach (var item in dto.ProgramPermissions)
            {
                var count = await _repository.SetPermissionsByProgramAsync(userId, item.ProgramId, item.IsAuthorized, createdBy);
                totalUpdated += count;
            }

            return new BatchOperationResult
            {
                UpdatedCount = totalUpdated,
                ClearedRoleCount = clearedRoleCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定使用者作業權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchSetButtonPermissionsAsync(string userId, BatchSetUserButtonPermissionDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            var clearedRoleCount = 0;

            // 清除使用者角色權限對應（如果設定為清除）
            if (dto.ClearRolePermissions)
            {
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
                var userRoleList = userRoles.ToList();
                if (userRoleList.Count > 0)
                {
                    await _userRoleRepository.DeleteRangeAsync(userRoleList);
                    clearedRoleCount = userRoleList.Count;
                    _logger.LogInfo($"清除使用者角色權限對應: {userId}, 清除數量: {clearedRoleCount}");
                }
            }

            var buttonIds = dto.ButtonPermissions
                .Where(x => x.IsAuthorized)
                .Select(x => x.ButtonId)
                .ToList();

            var addedCount = 0;
            if (buttonIds.Count > 0)
            {
                addedCount = await _repository.BatchCreateAsync(userId, buttonIds, GetCurrentUserId());
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
                var permissions = await _repository.QueryPermissionsAsync(userId, new UserPermissionQuery
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
                UpdatedCount = addedCount + removedCount,
                ClearedRoleCount = clearedRoleCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定使用者按鈕權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteUserPermissionAsync(string userId, long tKey)
    {
        try
        {
            // 檢查權限是否存在
            var permission = await _repository.GetByIdAsync(tKey);
            if (permission == null)
            {
                throw new InvalidOperationException($"使用者權限不存在: {tKey}");
            }

            if (permission.UserId != userId)
            {
                throw new InvalidOperationException($"權限不屬於該使用者: {userId}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者權限失敗: {userId} - {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchDeleteUserPermissionsAsync(string userId, BatchDeleteUserPermissionDto dto)
    {
        try
        {
            if (dto.TKeys == null || dto.TKeys.Count == 0)
            {
                return new BatchOperationResult();
            }

            // 驗證所有權限都屬於該使用者
            foreach (var tKey in dto.TKeys)
            {
                var permission = await _repository.GetByIdAsync(tKey);
                if (permission == null)
                {
                    throw new InvalidOperationException($"使用者權限不存在: {tKey}");
                }

                if (permission.UserId != userId)
                {
                    throw new InvalidOperationException($"權限不屬於該使用者: {userId}");
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
            _logger.LogError($"批量刪除使用者權限失敗: {userId}", ex);
            throw;
        }
    }
}

