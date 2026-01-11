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
/// 使用者欄位權限服務實作 (SYS0340)
/// </summary>
public class UserFieldPermissionService : BaseService, IUserFieldPermissionService
{
    private readonly IUserFieldPermissionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public UserFieldPermissionService(
        IUserFieldPermissionRepository repository,
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

    public async Task<List<DatabaseDto>> GetDatabasesAsync()
    {
        try
        {
            const string sql = @"
                SELECT 
                    DbName,
                    DbDescription
                FROM DatabaseInfo
                WHERE IsActive = 1
                ORDER BY DbName";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<DatabaseDto>(sql);

            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢資料庫列表失敗", ex);
            throw;
        }
    }

    public async Task<List<TableDto>> GetTablesAsync(string dbName)
    {
        try
        {
            const string sql = @"
                SELECT 
                    TableName,
                    TableDescription
                FROM TableInfo
                WHERE DbName = @DbName AND IsActive = 1
                ORDER BY TableName";

            var parameters = new DynamicParameters();
            parameters.Add("DbName", dbName);

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<TableDto>(sql, parameters);

            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢表格列表失敗: {dbName}", ex);
            throw;
        }
    }

    public async Task<List<FieldDto>> GetFieldsAsync(string dbName, string tableName)
    {
        try
        {
            const string sql = @"
                SELECT 
                    FieldName,
                    FieldType,
                    FieldLength,
                    FieldDescription
                FROM FieldInfo
                WHERE DbName = @DbName AND TableName = @TableName AND IsActive = 1
                ORDER BY FieldName";

            var parameters = new DynamicParameters();
            parameters.Add("DbName", dbName);
            parameters.Add("TableName", tableName);

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<FieldDto>(sql, parameters);

            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢欄位列表失敗: {dbName}.{tableName}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserFieldPermissionDto>> GetPermissionsAsync(UserFieldPermissionQueryDto query)
    {
        try
        {
            var repositoryQuery = new UserFieldPermissionQuery
            {
                UserId = query.UserId,
                DbName = query.DbName,
                TableName = query.TableName,
                FieldName = query.FieldName,
                PermissionType = query.PermissionType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 查詢使用者名稱
            var userIds = result.Items.Select(x => x.UserId).Distinct().ToList();
            var users = new Dictionary<string, string>();
            if (userIds.Any())
            {
                foreach (var userId in userIds)
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user != null)
                    {
                        users[userId] = user.UserName;
                    }
                }
            }

            var dtos = result.Items.Select(x => new UserFieldPermissionDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = users.ContainsKey(x.UserId) ? users[x.UserId] : null,
                DbName = x.DbName,
                TableName = x.TableName,
                FieldName = x.FieldName,
                PermissionType = x.PermissionType,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<UserFieldPermissionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者欄位權限列表失敗", ex);
            throw;
        }
    }

    public async Task<Guid> CreatePermissionAsync(CreateUserFieldPermissionDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {dto.UserId}");
            }

            // 檢查是否已存在
            var existing = await _repository.QueryAsync(new UserFieldPermissionQuery
            {
                UserId = dto.UserId,
                DbName = dto.DbName,
                TableName = dto.TableName,
                FieldName = dto.FieldName,
                PageIndex = 1,
                PageSize = 1
            });

            if (existing.Items.Any())
            {
                throw new InvalidOperationException("該使用者欄位權限已存在");
            }

            // 清除使用者角色權限對應（設定使用者直接權限時，需移除該使用者的角色權限對應）
            var userRoles = await _userRoleRepository.GetByUserIdAsync(dto.UserId);
            var userRoleList = userRoles.ToList();
            if (userRoleList.Count > 0)
            {
                await _userRoleRepository.DeleteRangeAsync(userRoleList);
                _logger.LogInfo($"清除使用者角色權限對應: {dto.UserId}, 清除數量: {userRoleList.Count}");
            }

            var entity = new UserFieldPermission
            {
                UserId = dto.UserId,
                DbName = dto.DbName,
                TableName = dto.TableName,
                FieldName = dto.FieldName,
                PermissionType = dto.PermissionType,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var id = await _repository.CreateAsync(entity);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增使用者欄位權限失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdatePermissionAsync(Guid id, UpdateUserFieldPermissionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者欄位權限不存在: {id}");
            }

            entity.PermissionType = dto.PermissionType;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            return await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者欄位權限失敗: {id}", ex);
            throw;
        }
    }

    public async Task<bool> DeletePermissionAsync(Guid id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者欄位權限不存在: {id}");
            }

            return await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者欄位權限失敗: {id}", ex);
            throw;
        }
    }

    public async Task<int> BatchSetPermissionsAsync(BatchSetUserFieldPermissionDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {dto.UserId}");
            }

            // 清除使用者角色權限對應（設定使用者直接權限時，需移除該使用者的角色權限對應）
            var userRoles = await _userRoleRepository.GetByUserIdAsync(dto.UserId);
            var userRoleList = userRoles.ToList();
            if (userRoleList.Count > 0)
            {
                await _userRoleRepository.DeleteRangeAsync(userRoleList);
                _logger.LogInfo($"清除使用者角色權限對應: {dto.UserId}, 清除數量: {userRoleList.Count}");
            }

            // 先刪除該使用者的所有欄位權限
            var existing = await _repository.QueryAsync(new UserFieldPermissionQuery
            {
                UserId = dto.UserId,
                PageIndex = 1,
                PageSize = int.MaxValue
            });

            if (existing.Items.Any())
            {
                var ids = existing.Items.Select(x => x.Id).ToList();
                await _repository.BatchDeleteAsync(ids);
            }

            // 新增新的權限
            var entities = dto.Permissions.Select(p => new UserFieldPermission
            {
                UserId = dto.UserId,
                DbName = p.DbName,
                TableName = p.TableName,
                FieldName = p.FieldName,
                PermissionType = p.PermissionType,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            }).ToList();

            var count = await _repository.BatchCreateAsync(entities);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次設定使用者欄位權限失敗", ex);
            throw;
        }
    }
}

