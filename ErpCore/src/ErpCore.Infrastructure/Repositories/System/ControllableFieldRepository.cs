using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 可管控欄位 Repository 實作 (SYS0510)
/// </summary>
public class ControllableFieldRepository : BaseRepository, IControllableFieldRepository
{
    public ControllableFieldRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ControllableField>> QueryAsync(ControllableFieldQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ControllableFields
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FieldId))
            {
                sql += " AND FieldId LIKE @FieldId";
                parameters.Add("FieldId", $"%{query.FieldId}%");
            }

            if (!string.IsNullOrEmpty(query.FieldName))
            {
                sql += " AND FieldName LIKE @FieldName";
                parameters.Add("FieldName", $"%{query.FieldName}%");
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

            if (query.IsActive.HasValue)
            {
                sql += " AND IsActive = @IsActive";
                parameters.Add("IsActive", query.IsActive.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "FieldId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ControllableField>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ControllableFields
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.FieldId))
            {
                countSql += " AND FieldId LIKE @FieldId";
                countParameters.Add("FieldId", $"%{query.FieldId}%");
            }
            if (!string.IsNullOrEmpty(query.FieldName))
            {
                countSql += " AND FieldName LIKE @FieldName";
                countParameters.Add("FieldName", $"%{query.FieldName}%");
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
            if (query.IsActive.HasValue)
            {
                countSql += " AND IsActive = @IsActive";
                countParameters.Add("IsActive", query.IsActive.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ControllableField>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢可管控欄位列表失敗", ex);
            throw;
        }
    }

    public async Task<ControllableField?> GetByIdAsync(string fieldId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ControllableFields 
                WHERE FieldId = @FieldId";

            return await QueryFirstOrDefaultAsync<ControllableField>(sql, new { FieldId = fieldId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢可管控欄位失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task<ControllableField> CreateAsync(ControllableField controllableField)
    {
        try
        {
            const string sql = @"
                INSERT INTO ControllableFields (
                    FieldId, FieldName, DbName, TableName, FieldNameInDb,
                    FieldType, FieldDescription, IsRequired, IsActive, SortOrder,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @FieldId, @FieldName, @DbName, @TableName, @FieldNameInDb,
                    @FieldType, @FieldDescription, @IsRequired, @IsActive, @SortOrder,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<ControllableField>(sql, controllableField);
            if (result == null)
            {
                throw new InvalidOperationException("新增可管控欄位失敗");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增可管控欄位失敗: {controllableField.FieldId}", ex);
            throw;
        }
    }

    public async Task<ControllableField> UpdateAsync(ControllableField controllableField)
    {
        try
        {
            const string sql = @"
                UPDATE ControllableFields SET
                    FieldName = @FieldName,
                    DbName = @DbName,
                    TableName = @TableName,
                    FieldNameInDb = @FieldNameInDb,
                    FieldType = @FieldType,
                    FieldDescription = @FieldDescription,
                    IsRequired = @IsRequired,
                    IsActive = @IsActive,
                    SortOrder = @SortOrder,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE FieldId = @FieldId";

            var result = await QueryFirstOrDefaultAsync<ControllableField>(sql, controllableField);
            if (result == null)
            {
                throw new InvalidOperationException($"可管控欄位不存在: {controllableField.FieldId}");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改可管控欄位失敗: {controllableField.FieldId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string fieldId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ControllableFields
                WHERE FieldId = @FieldId";

            await ExecuteAsync(sql, new { FieldId = fieldId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除可管控欄位失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteAsync(List<string> fieldIds)
    {
        try
        {
            if (fieldIds == null || fieldIds.Count == 0)
            {
                return 0;
            }

            const string sql = @"DELETE FROM ControllableFields WHERE FieldId IN @FieldIds";
            return await ExecuteAsync(sql, new { FieldIds = fieldIds });
        }
        catch (Exception ex)
        {
            _logger.LogError("批量刪除可管控欄位失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string fieldId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ControllableFields
                WHERE FieldId = @FieldId";

            var count = await QuerySingleAsync<int>(sql, new { FieldId = fieldId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查可管控欄位是否存在失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task<List<ControllableField>> GetByDbTableAsync(string dbName, string tableName)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ControllableFields
                WHERE DbName = @DbName AND TableName = @TableName
                ORDER BY SortOrder, FieldId";

            var items = await QueryAsync<ControllableField>(sql, new { DbName = dbName, TableName = tableName });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢可管控欄位失敗: {dbName} - {tableName}", ex);
            throw;
        }
    }
}

