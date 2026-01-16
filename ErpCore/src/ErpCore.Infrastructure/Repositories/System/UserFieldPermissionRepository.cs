using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者欄位權限 Repository 實作 (SYS0340)
/// </summary>
public class UserFieldPermissionRepository : BaseRepository, IUserFieldPermissionRepository
{
    public UserFieldPermissionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<UserFieldPermission>> QueryAsync(UserFieldPermissionQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    Id,
                    UserId,
                    DbName,
                    TableName,
                    FieldName,
                    PermissionType,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM UserFieldPermissions
                WHERE 1 = 1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND UserId = @UserId";
                parameters.Add("UserId", query.UserId);
            }

            if (!string.IsNullOrEmpty(query.DbName))
            {
                sql += " AND DbName = @DbName";
                parameters.Add("DbName", query.DbName);
            }

            if (!string.IsNullOrEmpty(query.TableName))
            {
                sql += " AND TableName = @TableName";
                parameters.Add("TableName", query.TableName);
            }

            if (!string.IsNullOrEmpty(query.FieldName))
            {
                sql += " AND FieldName LIKE @FieldName";
                parameters.Add("FieldName", $"%{query.FieldName}%");
            }

            if (!string.IsNullOrEmpty(query.PermissionType))
            {
                sql += " AND PermissionType = @PermissionType";
                parameters.Add("PermissionType", query.PermissionType);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CreatedAt" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<UserFieldPermission>(sql, parameters);

            // 查詢總數
            var countSql = "SELECT COUNT(*) FROM UserFieldPermissions WHERE 1 = 1";
            var countParameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.UserId))
            {
                countSql += " AND UserId = @UserId";
                countParameters.Add("UserId", query.UserId);
            }

            if (!string.IsNullOrEmpty(query.DbName))
            {
                countSql += " AND DbName = @DbName";
                countParameters.Add("DbName", query.DbName);
            }

            if (!string.IsNullOrEmpty(query.TableName))
            {
                countSql += " AND TableName = @TableName";
                countParameters.Add("TableName", query.TableName);
            }

            if (!string.IsNullOrEmpty(query.FieldName))
            {
                countSql += " AND FieldName LIKE @FieldName";
                countParameters.Add("FieldName", $"%{query.FieldName}%");
            }

            if (!string.IsNullOrEmpty(query.PermissionType))
            {
                countSql += " AND PermissionType = @PermissionType";
                countParameters.Add("PermissionType", query.PermissionType);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<UserFieldPermission>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者欄位權限列表失敗", ex);
            throw;
        }
    }

    public async Task<UserFieldPermission?> GetByIdAsync(Guid id)
    {
        try
        {
            const string sql = @"
                SELECT 
                    Id,
                    UserId,
                    DbName,
                    TableName,
                    FieldName,
                    PermissionType,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM UserFieldPermissions
                WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            return await QueryFirstOrDefaultAsync<UserFieldPermission>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者欄位權限失敗: {id}", ex);
            throw;
        }
    }

    public async Task<Guid> CreateAsync(UserFieldPermission entity)
    {
        try
        {
            var id = Guid.NewGuid();
            const string sql = @"
                INSERT INTO UserFieldPermissions (
                    Id,
                    UserId,
                    DbName,
                    TableName,
                    FieldName,
                    PermissionType,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                ) VALUES (
                    @Id,
                    @UserId,
                    @DbName,
                    @TableName,
                    @FieldName,
                    @PermissionType,
                    @CreatedBy,
                    GETDATE(),
                    @UpdatedBy,
                    GETDATE()
                )";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("UserId", entity.UserId);
            parameters.Add("DbName", entity.DbName);
            parameters.Add("TableName", entity.TableName);
            parameters.Add("FieldName", entity.FieldName);
            parameters.Add("PermissionType", entity.PermissionType);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("UpdatedBy", entity.UpdatedBy);

            await ExecuteAsync(sql, parameters);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增使用者欄位權限失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(UserFieldPermission entity)
    {
        try
        {
            const string sql = @"
                UPDATE UserFieldPermissions
                SET PermissionType = @PermissionType,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id);
            parameters.Add("PermissionType", entity.PermissionType);
            parameters.Add("UpdatedBy", entity.UpdatedBy);

            var affectedRows = await ExecuteAsync(sql, parameters);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者欄位權限失敗: {entity.Id}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            const string sql = "DELETE FROM UserFieldPermissions WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var affectedRows = await ExecuteAsync(sql, parameters);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者欄位權限失敗: {id}", ex);
            throw;
        }
    }

    public async Task<int> BatchCreateAsync(List<UserFieldPermission> entities)
    {
        try
        {
            if (entities == null || entities.Count == 0)
            {
                return 0;
            }

            const string sql = @"
                INSERT INTO UserFieldPermissions (
                    Id,
                    UserId,
                    DbName,
                    TableName,
                    FieldName,
                    PermissionType,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                ) VALUES (
                    @Id,
                    @UserId,
                    @DbName,
                    @TableName,
                    @FieldName,
                    @PermissionType,
                    @CreatedBy,
                    GETDATE(),
                    @UpdatedBy,
                    GETDATE()
                )";

            var parametersList = entities.Select(e =>
            {
                var p = new DynamicParameters();
                p.Add("Id", Guid.NewGuid());
                p.Add("UserId", e.UserId);
                p.Add("DbName", e.DbName);
                p.Add("TableName", e.TableName);
                p.Add("FieldName", e.FieldName);
                p.Add("PermissionType", e.PermissionType);
                p.Add("CreatedBy", e.CreatedBy);
                p.Add("UpdatedBy", e.UpdatedBy);
                return p;
            }).ToList();

            var totalAffectedRows = 0;
            using var connection = _connectionFactory.CreateConnection();
            foreach (var parameters in parametersList)
            {
                var affectedRows = await connection.ExecuteAsync(sql, parameters);
                totalAffectedRows += affectedRows;
            }
            return totalAffectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者欄位權限失敗", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteAsync(List<Guid> ids)
    {
        try
        {
            if (ids == null || ids.Count == 0)
            {
                return 0;
            }

            const string sql = "DELETE FROM UserFieldPermissions WHERE Id IN @Ids";

            var parameters = new DynamicParameters();
            parameters.Add("Ids", ids);

            var affectedRows = await ExecuteAsync(sql, parameters);
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除使用者欄位權限失敗", ex);
            throw;
        }
    }
}

