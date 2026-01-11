using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色之使用者設定維護服務實作 (SYS0230)
/// </summary>
public class RoleUserAssignmentService : BaseService, IRoleUserAssignmentService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public RoleUserAssignmentService(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserPermissionRepository userPermissionRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userPermissionRepository = userPermissionRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<RoleUserListItemDto>> GetRoleUsersAsync(RoleUserQueryDto query)
    {
        try
        {
            // 驗證角色是否存在
            var role = await _roleRepository.GetByIdAsync(query.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {query.RoleId}");
            }

            // 轉換查詢條件
            var roleUserQuery = new RoleUserQuery
            {
                RoleId = query.RoleId,
                OrgId = query.OrgId,
                StoreId = query.StoreId,
                UserType = query.UserType,
                Filter = query.Filter,
                Page = query.Page,
                PageSize = query.PageSize
            };

            // 查詢角色使用者列表
            var result = await _userRoleRepository.GetRoleUsersAsync(roleUserQuery);

            // 轉換為 DTO
            var items = result.Items.Select(item => new RoleUserListItemDto
            {
                UserId = item.UserId,
                UserName = item.UserName,
                OrgId = item.OrgId,
                OrgName = item.OrgName,
                IsAssigned = item.IsAssigned
            }).ToList();

            return new PagedResult<RoleUserListItemDto>
            {
                Items = items,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色使用者列表失敗: {query.RoleId}", ex);
            throw;
        }
    }

    public async Task<BatchAssignRoleUsersResultDto> BatchAssignRoleUsersAsync(string roleId, BatchAssignRoleUsersDto dto)
    {
        try
        {
            // 驗證角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            var addedCount = 0;
            var removedCount = 0;
            var currentUserId = GetCurrentUserId();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // 處理新增
                foreach (var userId in dto.AddUserIds)
                {
                    // 驗證使用者是否存在
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user == null)
                    {
                        _logger.LogWarning($"使用者不存在，跳過: {userId}");
                        continue;
                    }

                    // 檢查是否已存在
                    var exists = await _userRoleRepository.ExistsAsync(userId, roleId);
                    if (exists)
                    {
                        _logger.LogWarning($"使用者已分配此角色，跳過: {userId}, {roleId}");
                        continue;
                    }

                    // 清除使用者直接權限（重要：當角色分配給使用者時，需清除使用者的直接權限設定）
                    await _userPermissionRepository.DeleteByUserIdAsync(userId);
                    _logger.LogInfo($"清除使用者直接權限: {userId}");

                    // 新增對應關係
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        CreatedBy = currentUserId,
                        CreatedAt = DateTime.Now,
                        CreatedPriority = null,
                        CreatedGroup = null
                    };
                    await _userRoleRepository.CreateAsync(userRole);
                    addedCount++;
                }

                // 處理移除
                foreach (var userId in dto.RemoveUserIds)
                {
                    var exists = await _userRoleRepository.ExistsAsync(userId, roleId);
                    if (!exists)
                    {
                        _logger.LogWarning($"使用者未分配此角色，跳過: {userId}, {roleId}");
                        continue;
                    }

                    await _userRoleRepository.DeleteAsync(userId, roleId);
                    removedCount++;
                }

                transaction.Commit();

                _logger.LogInfo($"批量設定角色使用者成功: RoleId={roleId}, 新增={addedCount}, 移除={removedCount}");

                return new BatchAssignRoleUsersResultDto
                {
                    AddedCount = addedCount,
                    RemovedCount = removedCount
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量設定角色使用者失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task AssignUserToRoleAsync(string roleId, string userId)
    {
        try
        {
            // 驗證角色是否存在
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            // 驗證使用者是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 檢查是否已存在
            var exists = await _userRoleRepository.ExistsAsync(userId, roleId);
            if (exists)
            {
                throw new InvalidOperationException($"使用者已分配此角色: {userId}, {roleId}");
            }

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // 清除使用者直接權限（重要：當角色分配給使用者時，需清除使用者的直接權限設定）
                await _userPermissionRepository.DeleteByUserIdAsync(userId);
                _logger.LogInfo($"清除使用者直接權限: {userId}");

                // 新增對應關係
                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    CreatedPriority = null,
                    CreatedGroup = null
                };
                await _userRoleRepository.CreateAsync(userRole);

                transaction.Commit();

                _logger.LogInfo($"新增角色使用者成功: RoleId={roleId}, UserId={userId}");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增角色使用者失敗: {roleId}, {userId}", ex);
            throw;
        }
    }

    public async Task RemoveUserFromRoleAsync(string roleId, string userId)
    {
        try
        {
            // 驗證對應關係是否存在
            var userRole = await _userRoleRepository.GetByUserIdAndRoleIdAsync(userId, roleId);
            if (userRole == null)
            {
                throw new InvalidOperationException($"使用者未分配此角色: {userId}, {roleId}");
            }

            await _userRoleRepository.DeleteAsync(userId, roleId);

            _logger.LogInfo($"移除角色使用者成功: RoleId={roleId}, UserId={userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"移除角色使用者失敗: {roleId}, {userId}", ex);
            throw;
        }
    }
}
