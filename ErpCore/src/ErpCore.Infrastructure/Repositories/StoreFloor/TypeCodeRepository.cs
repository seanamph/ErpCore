using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StoreFloor;

/// <summary>
/// 類型代碼 Repository 實作 (SYS6405-SYS6490 - 類型代碼維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TypeCodeRepository : BaseRepository, ITypeCodeRepository
{
    public TypeCodeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TypeCode?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TypeCodes 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<TypeCode>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢類型代碼失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TypeCode>> QueryAsync(TypeCodeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM TypeCodes
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TypeCode))
            {
                sql += " AND TypeCode LIKE @TypeCode";
                parameters.Add("TypeCode", $"%{query.TypeCode}%");
            }

            if (!string.IsNullOrEmpty(query.TypeName))
            {
                sql += " AND TypeName LIKE @TypeName";
                parameters.Add("TypeName", $"%{query.TypeName}%");
            }

            if (!string.IsNullOrEmpty(query.Category))
            {
                sql += " AND Category = @Category";
                parameters.Add("Category", query.Category);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "SortOrder";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<TypeCode>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<TypeCode>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢類型代碼列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(TypeCodeQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM TypeCodes
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TypeCode))
            {
                sql += " AND TypeCode LIKE @TypeCode";
                parameters.Add("TypeCode", $"%{query.TypeCode}%");
            }

            if (!string.IsNullOrEmpty(query.TypeName))
            {
                sql += " AND TypeName LIKE @TypeName";
                parameters.Add("TypeName", $"%{query.TypeName}%");
            }

            if (!string.IsNullOrEmpty(query.Category))
            {
                sql += " AND Category = @Category";
                parameters.Add("Category", query.Category);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢類型代碼總數失敗", ex);
            throw;
        }
    }

    public async Task<TypeCode> CreateAsync(TypeCode typeCode)
    {
        try
        {
            const string sql = @"
                INSERT INTO TypeCodes (
                    TypeCode, TypeName, TypeNameEn, Category, Description, SortOrder, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @TypeCode, @TypeName, @TypeNameEn, @Category, @Description, @SortOrder, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("TypeCode", typeCode.TypeCode);
            parameters.Add("TypeName", typeCode.TypeName);
            parameters.Add("TypeNameEn", typeCode.TypeNameEn);
            parameters.Add("Category", typeCode.Category);
            parameters.Add("Description", typeCode.Description);
            parameters.Add("SortOrder", typeCode.SortOrder);
            parameters.Add("Status", typeCode.Status);
            parameters.Add("CreatedBy", typeCode.CreatedBy);
            parameters.Add("CreatedAt", typeCode.CreatedAt);
            parameters.Add("UpdatedBy", typeCode.UpdatedBy);
            parameters.Add("UpdatedAt", typeCode.UpdatedAt);

            var tKey = await QuerySingleAsync<long>(sql, parameters);
            typeCode.TKey = tKey;

            return typeCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增類型代碼失敗: {typeCode.TypeCode}", ex);
            throw;
        }
    }

    public async Task<TypeCode> UpdateAsync(TypeCode typeCode)
    {
        try
        {
            const string sql = @"
                UPDATE TypeCodes SET
                    TypeName = @TypeName,
                    TypeNameEn = @TypeNameEn,
                    Category = @Category,
                    Description = @Description,
                    SortOrder = @SortOrder,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var parameters = new DynamicParameters();
            parameters.Add("TKey", typeCode.TKey);
            parameters.Add("TypeName", typeCode.TypeName);
            parameters.Add("TypeNameEn", typeCode.TypeNameEn);
            parameters.Add("Category", typeCode.Category);
            parameters.Add("Description", typeCode.Description);
            parameters.Add("SortOrder", typeCode.SortOrder);
            parameters.Add("Status", typeCode.Status);
            parameters.Add("UpdatedBy", typeCode.UpdatedBy);
            parameters.Add("UpdatedAt", typeCode.UpdatedAt);

            await ExecuteAsync(sql, parameters);

            return typeCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改類型代碼失敗: {typeCode.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM TypeCodes
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除類型代碼失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string typeCode, string? category)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM TypeCodes
                WHERE TypeCode = @TypeCode";
            
            var parameters = new DynamicParameters();
            parameters.Add("TypeCode", typeCode);

            if (!string.IsNullOrEmpty(category))
            {
                sql += " AND Category = @Category";
                parameters.Add("Category", category);
            }
            else
            {
                sql += " AND Category IS NULL";
            }

            var count = await QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查類型代碼是否存在失敗: {typeCode}/{category}", ex);
            throw;
        }
    }

    public async Task<int> GetUsageCountAsync(string typeCode, string? category)
    {
        try
        {
            // 查詢類型代碼的使用次數
            // 這裡需要根據實際業務邏輯查詢相關表
            // 假設有一個 TypeCodeUsages 表記錄使用記錄
            // 如果表不存在，返回0
            const string sql = @"
                SELECT COUNT(*) 
                FROM TypeCodeUsages 
                WHERE TypeCode = @TypeCode";
            
            var parameters = new DynamicParameters();
            parameters.Add("TypeCode", typeCode);

            if (!string.IsNullOrEmpty(category))
            {
                const string sqlWithCategory = @"
                    SELECT COUNT(*) 
                    FROM TypeCodeUsages 
                    WHERE TypeCode = @TypeCode AND Category = @Category";
                parameters.Add("Category", category);
                
                try
                {
                    return await QuerySingleAsync<int>(sqlWithCategory, parameters);
                }
                catch
                {
                    // 如果表不存在，返回0
                    return 0;
                }
            }

            try
            {
                return await QuerySingleAsync<int>(sql, parameters);
            }
            catch
            {
                // 如果表不存在，返回0
                return 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢類型代碼使用次數失敗: {typeCode}/{category}", ex);
            // 發生錯誤時返回0，不影響主流程
            return 0;
        }
    }
}

