using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色服務實作 (SYS0210)
/// </summary>
public class RoleService : BaseService, IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IRolePermissionRepository? _rolePermissionRepository;
    private readonly IUserRoleRepository? _userRoleRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public RoleService(
        IRoleRepository repository,
        ILoggerService logger,
        IUserContext userContext,
        IDbConnectionFactory connectionFactory,
        IRolePermissionRepository? rolePermissionRepository = null,
        IUserRoleRepository? userRoleRepository = null) : base(logger, userContext)
    {
        _repository = repository;
        _rolePermissionRepository = rolePermissionRepository;
        _userRoleRepository = userRoleRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<RoleDto>> GetRolesAsync(RoleQueryDto query)
    {
        try
        {
            var repositoryQuery = new RoleQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                RoleId = query.RoleId,
                RoleName = query.RoleName
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new RoleDto
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName,
                RoleNote = x.RoleNote,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<RoleDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢角色列表失敗", ex);
            throw;
        }
    }

    public async Task<RoleDto> GetRoleByIdAsync(string roleId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(roleId);
            if (entity == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            return new RoleDto
            {
                RoleId = entity.RoleId,
                RoleName = entity.RoleName,
                RoleNote = entity.RoleNote,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<string> CreateRoleAsync(CreateRoleDto dto)
    {
        try
        {
            // 驗證資料
            if (string.IsNullOrWhiteSpace(dto.RoleId))
            {
                throw new ArgumentException("角色代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                throw new ArgumentException("角色名稱不能為空");
            }

            if (dto.RoleId.Length > 50)
            {
                throw new ArgumentException("角色代碼長度不能超過50字元");
            }

            if (dto.RoleName.Length > 100)
            {
                throw new ArgumentException("角色名稱長度不能超過100字元");
            }

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.RoleId);
            if (exists)
            {
                throw new InvalidOperationException($"角色已存在: {dto.RoleId}");
            }

            var entity = new Role
            {
                RoleId = dto.RoleId,
                RoleName = dto.RoleName,
                RoleNote = dto.RoleNote,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now,
                CreatedPriority = _userContext.UserPriority,
                CreatedGroup = _userContext.UserGroup
            };

            await _repository.CreateAsync(entity);

            return entity.RoleId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增角色失敗: {dto.RoleId}", ex);
            throw;
        }
    }

    public async Task UpdateRoleAsync(string roleId, UpdateRoleDto dto)
    {
        try
        {
            // 驗證資料
            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                throw new ArgumentException("角色名稱不能為空");
            }

            if (dto.RoleName.Length > 100)
            {
                throw new ArgumentException("角色名稱長度不能超過100字元");
            }

            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(roleId);
            if (entity == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            entity.RoleName = dto.RoleName;
            entity.RoleNote = dto.RoleNote;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改角色失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task DeleteRoleAsync(string roleId)
    {
        try
        {
            // 檢查是否存在
            var exists = await _repository.ExistsAsync(roleId);
            if (!exists)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            // 檢查是否有使用者使用此角色
            var hasUsers = await _repository.HasUsersAsync(roleId);
            if (hasUsers)
            {
                throw new InvalidOperationException($"角色已被使用者使用，無法刪除: {roleId}");
            }

            await _repository.DeleteAsync(roleId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除角色失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<string> CopyRoleAsync(string roleId, CopyRoleDto dto)
    {
        try
        {
            // 驗證資料
            if (string.IsNullOrWhiteSpace(dto.NewRoleId))
            {
                throw new ArgumentException("新角色代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.NewRoleName))
            {
                throw new ArgumentException("新角色名稱不能為空");
            }

            // 檢查原角色是否存在
            var sourceRole = await _repository.GetByIdAsync(roleId);
            if (sourceRole == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            // 檢查新角色代碼是否已存在
            var exists = await _repository.ExistsAsync(dto.NewRoleId);
            if (exists)
            {
                throw new InvalidOperationException($"角色已存在: {dto.NewRoleId}");
            }

            // 建立新角色
            var newRole = new Role
            {
                RoleId = dto.NewRoleId,
                RoleName = dto.NewRoleName,
                RoleNote = sourceRole.RoleNote,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now,
                CreatedPriority = _userContext.UserPriority,
                CreatedGroup = _userContext.UserGroup
            };

            await _repository.CreateAsync(newRole);

            // 複製權限設定（如果有 RolePermissionRepository）
            if (_rolePermissionRepository != null)
            {
                // TODO: 實作複製權限設定的邏輯
                // var permissions = await _rolePermissionRepository.GetByRoleIdAsync(roleId);
                // foreach (var permission in permissions)
                // {
                //     await _rolePermissionRepository.CreateAsync(new RolePermission { ... });
                // }
            }

            return newRole.RoleId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"複製角色失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<CopyRoleResultDto> CopyRoleToTargetAsync(CopyRoleToTargetDto dto)
    {
        try
        {
            // 1. 驗證來源和目的角色不能相同
            if (dto.SourceRoleId == dto.TargetRoleId)
            {
                throw new ArgumentException("來源角色和目的角色不能相同");
            }

            // 2. 驗證來源角色存在
            var sourceRole = await _repository.GetByIdAsync(dto.SourceRoleId);
            if (sourceRole == null)
            {
                throw new InvalidOperationException($"來源角色不存在: {dto.SourceRoleId}");
            }

            // 3. 驗證目的角色存在
            var targetRole = await _repository.GetByIdAsync(dto.TargetRoleId);
            if (targetRole == null)
            {
                throw new InvalidOperationException($"目的角色不存在: {dto.TargetRoleId}");
            }

            // 4. 驗證來源角色有權限設定
            if (_rolePermissionRepository == null)
            {
                throw new InvalidOperationException("角色權限 Repository 未注入");
            }

            var sourcePermissions = await _rolePermissionRepository.GetByRoleIdAsync(dto.SourceRoleId);
            if (!sourcePermissions.Any())
            {
                throw new InvalidOperationException($"來源角色沒有權限設定: {dto.SourceRoleId}");
            }

            // 5. 執行複製（在交易中）
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 清除目的角色權限
                await _rolePermissionRepository.DeleteByRoleIdAsync(dto.TargetRoleId, transaction);

                // 複製權限
                var permissionsCopied = await _rolePermissionRepository.CopyFromRoleAsync(
                    dto.SourceRoleId,
                    dto.TargetRoleId,
                    _userContext.UserId ?? string.Empty,
                    transaction
                );

                int usersCopied = 0;
                if (dto.CopyUsers)
                {
                    if (_userRoleRepository == null)
                    {
                        throw new InvalidOperationException("使用者角色 Repository 未注入");
                    }

                    // 清除目的角色使用者
                    await _userRoleRepository.DeleteByRoleIdAsync(dto.TargetRoleId, transaction);

                    // 複製使用者
                    usersCopied = await _userRoleRepository.CopyFromRoleAsync(
                        dto.SourceRoleId,
                        dto.TargetRoleId,
                        _userContext.UserId ?? string.Empty,
                        transaction
                    );
                }

                await transaction.CommitAsync();

                _logger.LogInfo($"角色複製成功: {dto.SourceRoleId} -> {dto.TargetRoleId}, 權限: {permissionsCopied} 筆, 使用者: {usersCopied} 筆");

                return new CopyRoleResultDto
                {
                    SourceRoleId = dto.SourceRoleId,
                    TargetRoleId = dto.TargetRoleId,
                    PermissionsCopied = permissionsCopied,
                    UsersCopied = usersCopied
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"複製角色失敗: {dto.SourceRoleId} -> {dto.TargetRoleId}", ex);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"複製角色到目標角色失敗: {dto.SourceRoleId} -> {dto.TargetRoleId}", ex);
            throw;
        }
    }

    public async Task<ValidateCopyRoleResultDto> ValidateCopyRoleAsync(ValidateCopyRoleDto dto)
    {
        try
        {
            var result = new ValidateCopyRoleResultDto();

            // 檢查是否相同
            if (dto.SourceRoleId == dto.TargetRoleId)
            {
                result.IsSameRole = true;
                return result;
            }

            result.IsSameRole = false;

            // 檢查來源角色
            var sourceRole = await _repository.GetByIdAsync(dto.SourceRoleId);
            result.SourceRoleExists = sourceRole != null;

            // 檢查目的角色
            var targetRole = await _repository.GetByIdAsync(dto.TargetRoleId);
            result.TargetRoleExists = targetRole != null;

            // 檢查來源角色權限
            if (result.SourceRoleExists && _rolePermissionRepository != null)
            {
                var permissions = await _rolePermissionRepository.GetByRoleIdAsync(dto.SourceRoleId);
                result.SourceHasPermissions = permissions.Any();
            }
            else
            {
                result.SourceHasPermissions = false;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"驗證角色複製失敗: {dto.SourceRoleId} -> {dto.TargetRoleId}", ex);
            throw;
        }
    }
}
