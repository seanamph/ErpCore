using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者角色服務實作 (SYS0220)
/// </summary>
public class UserRoleService : BaseService, IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRoleService(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<UserRoleDto>> GetUserRolesAsync(string userId, int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            // 驗證使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 查詢使用者角色
            var userRoles = await _userRoleRepository.GetUserRolesAsync(userId, pageIndex, pageSize);

            // 查詢角色資訊
            var roleIds = userRoles.Items.Select(ur => ur.RoleId).ToList();
            var roles = new List<Role>();
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null)
                {
                    roles.Add(role);
                }
            }

            // 組裝回應資料
            var dtos = userRoles.Items.Select(ur =>
            {
                var role = roles.FirstOrDefault(r => r.RoleId == ur.RoleId);
                return new UserRoleDto
                {
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    RoleName = role?.RoleName,
                    CreatedBy = ur.CreatedBy,
                    CreatedAt = ur.CreatedAt
                };
            }).ToList();

            return new PagedResult<UserRoleDto>
            {
                Items = dtos,
                TotalCount = userRoles.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者角色列表失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<RoleDto>> GetAvailableRolesAsync(string userId, string? keyword = null, int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            // 查詢使用者已分配的角色ID
            var assignedRoleIds = await _userRoleRepository.GetRoleIdsByUserIdAsync(userId);

            // 查詢所有角色
            var roleQuery = new RoleQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleId = keyword,
                RoleName = keyword
            };

            var allRoles = await _roleRepository.QueryAsync(roleQuery);

            // 過濾已分配的角色
            var availableRoles = allRoles.Items
                .Where(r => !assignedRoleIds.Contains(r.RoleId))
                .ToList();

            var dtos = availableRoles.Select(r => new RoleDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                RoleNote = r.RoleNote,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                UpdatedBy = r.UpdatedBy,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            return new PagedResult<RoleDto>
            {
                Items = dtos,
                TotalCount = availableRoles.Count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢可用角色列表失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<AssignRoleResultDto> AssignRolesAsync(string userId, List<string> roleIds)
    {
        try
        {
            // 驗證使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 驗證角色是否存在
            var roles = new List<Role>();
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    throw new InvalidOperationException($"角色不存在: {roleId}");
                }
                roles.Add(role);
            }

            // 檢查是否已存在
            var existingUserRoles = await _userRoleRepository.GetByUserIdAndRoleIdsAsync(userId, roleIds);
            var existingRoleIds = existingUserRoles.Select(ur => ur.RoleId).ToList();
            var newRoleIds = roleIds.Except(existingRoleIds).ToList();

            if (newRoleIds.Count == 0)
            {
                throw new InvalidOperationException("所有角色已分配");
            }

            // 新增角色分配
            var currentUserId = GetCurrentUserId();
            var currentUserEntity = await _userRepository.GetByIdAsync(currentUserId);
            var userRoles = newRoleIds.Select(roleId => new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.Now,
                CreatedPriority = currentUserEntity?.CreatedPriority,
                CreatedGroup = currentUserEntity?.CreatedGroup
            }).ToList();

            await _userRoleRepository.CreateRangeAsync(userRoles);

            return new AssignRoleResultDto
            {
                AssignedCount = newRoleIds.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"分配角色失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<RemoveRoleResultDto> RemoveRolesAsync(string userId, List<string> roleIds)
    {
        try
        {
            // 驗證使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 查詢要刪除的使用者角色
            var userRoles = await _userRoleRepository.GetByUserIdAndRoleIdsAsync(userId, roleIds);
            if (!userRoles.Any())
            {
                throw new InvalidOperationException("未找到要移除的角色");
            }

            // 刪除角色分配
            await _userRoleRepository.DeleteRangeAsync(userRoles);

            return new RemoveRoleResultDto
            {
                RemovedCount = userRoles.Count()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"移除角色失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<BatchUpdateRoleResultDto> BatchUpdateRolesAsync(string userId, List<string> roleIds)
    {
        try
        {
            // 驗證使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 驗證角色是否存在
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    throw new InvalidOperationException($"角色不存在: {roleId}");
                }
            }

            // 查詢目前分配的角色
            var currentUserRoles = await _userRoleRepository.GetByUserIdAsync(userId);
            var currentRoleIds = currentUserRoles.Select(ur => ur.RoleId).ToList();

            // 計算需要新增和刪除的角色
            var roleIdsToAdd = roleIds.Except(currentRoleIds).ToList();
            var roleIdsToRemove = currentRoleIds.Except(roleIds).ToList();

            var currentUserId = GetCurrentUserId();
            var currentUserEntity = await _userRepository.GetByIdAsync(currentUserId);

            // 執行刪除
            if (roleIdsToRemove.Count > 0)
            {
                var userRolesToRemove = currentUserRoles
                    .Where(ur => roleIdsToRemove.Contains(ur.RoleId))
                    .ToList();
                await _userRoleRepository.DeleteRangeAsync(userRolesToRemove);
            }

            // 執行新增
            if (roleIdsToAdd.Count > 0)
            {
                var userRolesToAdd = roleIdsToAdd.Select(roleId => new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.Now,
                    CreatedPriority = currentUserEntity?.CreatedPriority,
                    CreatedGroup = currentUserEntity?.CreatedGroup
                }).ToList();
                await _userRoleRepository.CreateRangeAsync(userRolesToAdd);
            }

            return new BatchUpdateRoleResultDto
            {
                AddedCount = roleIdsToAdd.Count,
                RemovedCount = roleIdsToRemove.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量更新角色失敗: {userId}", ex);
            throw;
        }
    }
}

