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
/// 角色服務實作
/// </summary>
public class RoleService : BaseService, IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IRolePermissionRepository _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public RoleService(
        IRoleRepository repository,
        IRolePermissionRepository permissionRepository,
        IUserRoleRepository userRoleRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _permissionRepository = permissionRepository;
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
            ValidateCreateDto(dto);

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
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
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
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(roleId);
            if (entity == null)
            {
                throw new InvalidOperationException($"角色不存在: {roleId}");
            }

            entity.RoleName = dto.RoleName;
            entity.RoleNote = dto.RoleNote;
            entity.UpdatedBy = GetCurrentUserId();
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
            var entity = await _repository.GetByIdAsync(roleId);
            if (entity == null)
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

    public async Task DeleteRolesBatchAsync(BatchDeleteRoleDto dto)
    {
        try
        {
            foreach (var roleId in dto.RoleIds)
            {
                await DeleteRoleAsync(roleId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除角色失敗", ex);
            throw;
        }
    }

    public async Task<string> CopyRoleAsync(string roleId, CopyRoleDto dto)
    {
        try
        {
            // 檢查來源角色是否存在
            var sourceRole = await _repository.GetByIdAsync(roleId);
            if (sourceRole == null)
            {
                throw new InvalidOperationException($"來源角色不存在: {roleId}");
            }

            // 檢查新角色代碼是否已存在
            var exists = await _repository.ExistsAsync(dto.NewRoleId);
            if (exists)
            {
                throw new InvalidOperationException($"角色代碼已存在: {dto.NewRoleId}");
            }

            // 建立新角色
            var newRole = new Role
            {
                RoleId = dto.NewRoleId,
                RoleName = dto.NewRoleName,
                RoleNote = sourceRole.RoleNote,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(newRole);

            // 複製角色權限設定
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // 查詢來源角色的所有權限
                    const string selectSql = @"
                        SELECT ButtonId 
                        FROM RoleButtons 
                        WHERE RoleId = @SourceRoleId";

                    var buttonIds = (await connection.QueryAsync<string>(
                        selectSql, 
                        new { SourceRoleId = roleId }, 
                        transaction)).ToList();

                    if (buttonIds.Any())
                    {
                        // 批量新增權限到新角色
                        var createdBy = GetCurrentUserId();
                        await _permissionRepository.BatchCreateAsync(newRole.RoleId, buttonIds, createdBy);
                        _logger.LogInfo($"複製角色權限成功: 來源角色={roleId}, 新角色={newRole.RoleId}, 權限數量={buttonIds.Count}");
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"複製角色權限失敗: 來源角色={roleId}, 新角色={newRole.RoleId}", ex);
                // 不拋出異常，僅記錄日誌，因為角色已建立成功
            }

            return newRole.RoleId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"複製角色失敗: {roleId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateRoleDto dto)
    {
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
    }

    public async Task<CopyRoleResultDto> CopyRoleToTargetAsync(CopyRoleRequestDto dto)
    {
        try
        {
            // 1. 驗證來源角色和目的角色不能相同
            if (dto.SourceRoleId == dto.TargetRoleId)
            {
                throw new InvalidOperationException("來源角色和目的角色不能相同");
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
            var sourcePermissions = await _permissionRepository.GetByRoleIdAsync(dto.SourceRoleId);
            if (!sourcePermissions.Any())
            {
                throw new InvalidOperationException("來源角色沒有權限設定");
            }

            // 5. 執行複製（在交易中）
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 清除目的角色權限
                await _permissionRepository.DeleteByRoleIdAsync(dto.TargetRoleId, transaction);
                _logger.LogInfo($"清除目的角色權限: {dto.TargetRoleId}");

                // 複製權限
                var currentUserId = GetCurrentUserId();
                var permissionsCopied = await _permissionRepository.CopyFromRoleAsync(
                    dto.SourceRoleId,
                    dto.TargetRoleId,
                    currentUserId,
                    transaction
                );
                _logger.LogInfo($"複製角色權限成功: 來源={dto.SourceRoleId}, 目的={dto.TargetRoleId}, 數量={permissionsCopied}");

                int usersCopied = 0;
                if (dto.CopyUsers)
                {
                    // 清除目的角色使用者
                    await _userRoleRepository.DeleteByRoleIdAsync(dto.TargetRoleId, transaction);
                    _logger.LogInfo($"清除目的角色使用者: {dto.TargetRoleId}");

                    // 複製使用者
                    usersCopied = await _userRoleRepository.CopyFromRoleAsync(
                        dto.SourceRoleId,
                        dto.TargetRoleId,
                        currentUserId,
                        transaction
                    );
                    _logger.LogInfo($"複製角色使用者成功: 來源={dto.SourceRoleId}, 目的={dto.TargetRoleId}, 數量={usersCopied}");
                }

                transaction.Commit();

                return new CopyRoleResultDto
                {
                    SourceRoleId = dto.SourceRoleId,
                    TargetRoleId = dto.TargetRoleId,
                    PermissionsCopied = permissionsCopied,
                    UsersCopied = usersCopied
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
            _logger.LogError($"複製角色到目標角色失敗: {dto.SourceRoleId} -> {dto.TargetRoleId}", ex);
            throw;
        }
    }

    public async Task<ValidateCopyResultDto> ValidateCopyRoleAsync(ValidateCopyRequestDto dto)
    {
        try
        {
            var result = new ValidateCopyResultDto();

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
            if (result.SourceRoleExists)
            {
                var permissions = await _permissionRepository.GetByRoleIdAsync(dto.SourceRoleId);
                result.SourceHasPermissions = permissions.Any();
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

