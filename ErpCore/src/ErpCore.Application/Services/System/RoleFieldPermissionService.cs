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
/// 角色欄位權限服務實作 (SYS0330)
/// </summary>
public class RoleFieldPermissionService : BaseService, IRoleFieldPermissionService
{
    private readonly IRoleFieldPermissionRepository _repository;
    private readonly IRoleRepository _roleRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public RoleFieldPermissionService(
        IRoleFieldPermissionRepository repository,
        IRoleRepository roleRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _roleRepository = roleRepository;
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

    public async Task<PagedResult<RoleFieldPermissionDto>> GetPermissionsAsync(RoleFieldPermissionQueryDto query)
    {
        try
        {
            var repositoryQuery = new RoleFieldPermissionQuery
            {
                RoleId = query.RoleId,
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

            // 查詢角色名稱
            var roleIds = result.Items.Select(x => x.RoleId).Distinct().ToList();
            var roles = new Dictionary<string, string>();
            if (roleIds.Any())
            {
                foreach (var roleId in roleIds)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role != null)
                    {
                        roles[roleId] = role.RoleName;
                    }
                }
            }

            var dtos = result.Items.Select(x => new RoleFieldPermissionDto
            {
                Id = x.Id,
                RoleId = x.RoleId,
                RoleName = roles.ContainsKey(x.RoleId) ? roles[x.RoleId] : null,
                DbName = x.DbName,
                TableName = x.TableName,
                FieldName = x.FieldName,
                PermissionType = x.PermissionType,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<RoleFieldPermissionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢角色欄位權限列表失敗", ex);
            throw;
        }
    }

    public async Task<Guid> CreatePermissionAsync(CreateRoleFieldPermissionDto dto)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {dto.RoleId}");
            }

            // 檢查是否已存在
            var existing = await _repository.QueryAsync(new RoleFieldPermissionQuery
            {
                RoleId = dto.RoleId,
                DbName = dto.DbName,
                TableName = dto.TableName,
                FieldName = dto.FieldName,
                PageIndex = 1,
                PageSize = 1
            });

            if (existing.Items.Any())
            {
                throw new InvalidOperationException("該角色欄位權限已存在");
            }

            var entity = new RoleFieldPermission
            {
                RoleId = dto.RoleId,
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
            _logger.LogError("新增角色欄位權限失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdatePermissionAsync(Guid id, UpdateRoleFieldPermissionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"角色欄位權限不存在: {id}");
            }

            entity.PermissionType = dto.PermissionType;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            return await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改角色欄位權限失敗: {id}", ex);
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
                throw new InvalidOperationException($"角色欄位權限不存在: {id}");
            }

            return await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除角色欄位權限失敗: {id}", ex);
            throw;
        }
    }

    public async Task<int> BatchSetPermissionsAsync(BatchSetRoleFieldPermissionDto dto)
    {
        try
        {
            // 檢查角色是否存在
            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"角色不存在: {dto.RoleId}");
            }

            // 先刪除該角色的所有欄位權限
            var existing = await _repository.QueryAsync(new RoleFieldPermissionQuery
            {
                RoleId = dto.RoleId,
                PageIndex = 1,
                PageSize = int.MaxValue
            });

            if (existing.Items.Any())
            {
                var ids = existing.Items.Select(x => x.Id).ToList();
                await _repository.BatchDeleteAsync(ids);
            }

            // 新增新的權限
            var entities = dto.Permissions.Select(p => new RoleFieldPermission
            {
                RoleId = dto.RoleId,
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
            _logger.LogError("批次設定角色欄位權限失敗", ex);
            throw;
        }
    }
}

