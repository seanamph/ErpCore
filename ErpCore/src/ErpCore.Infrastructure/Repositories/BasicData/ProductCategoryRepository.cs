using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 商品分類 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ProductCategoryRepository : BaseRepository, IProductCategoryRepository
{
    public ProductCategoryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ProductCategory?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ProductCategories 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ProductCategory>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商品分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ProductCategory>> QueryAsync(ProductCategoryQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ProductCategories
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ClassId))
            {
                sql += " AND ClassId LIKE @ClassId";
                parameters.Add("ClassId", $"%{query.ClassId}%");
            }

            if (!string.IsNullOrEmpty(query.ClassName))
            {
                sql += " AND ClassName LIKE @ClassName";
                parameters.Add("ClassName", $"%{query.ClassName}%");
            }

            if (!string.IsNullOrEmpty(query.ClassMode))
            {
                sql += " AND ClassMode = @ClassMode";
                parameters.Add("ClassMode", query.ClassMode);
            }

            if (!string.IsNullOrEmpty(query.ClassType))
            {
                sql += " AND ClassType = @ClassType";
                parameters.Add("ClassType", query.ClassType);
            }

            if (!string.IsNullOrEmpty(query.BClassId))
            {
                sql += " AND BClassId = @BClassId";
                parameters.Add("BClassId", query.BClassId);
            }

            if (!string.IsNullOrEmpty(query.MClassId))
            {
                sql += " AND MClassId = @MClassId";
                parameters.Add("MClassId", query.MClassId);
            }

            if (query.ParentTKey.HasValue)
            {
                sql += " AND ParentTKey = @ParentTKey";
                parameters.Add("ParentTKey", query.ParentTKey.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ClassId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ProductCategory>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ProductCategories
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ClassId))
            {
                countSql += " AND ClassId LIKE @ClassId";
                countParameters.Add("ClassId", $"%{query.ClassId}%");
            }
            if (!string.IsNullOrEmpty(query.ClassName))
            {
                countSql += " AND ClassName LIKE @ClassName";
                countParameters.Add("ClassName", $"%{query.ClassName}%");
            }
            if (!string.IsNullOrEmpty(query.ClassMode))
            {
                countSql += " AND ClassMode = @ClassMode";
                countParameters.Add("ClassMode", query.ClassMode);
            }
            if (!string.IsNullOrEmpty(query.ClassType))
            {
                countSql += " AND ClassType = @ClassType";
                countParameters.Add("ClassType", query.ClassType);
            }
            if (!string.IsNullOrEmpty(query.BClassId))
            {
                countSql += " AND BClassId = @BClassId";
                countParameters.Add("BClassId", query.BClassId);
            }
            if (!string.IsNullOrEmpty(query.MClassId))
            {
                countSql += " AND MClassId = @MClassId";
                countParameters.Add("MClassId", query.MClassId);
            }
            if (query.ParentTKey.HasValue)
            {
                countSql += " AND ParentTKey = @ParentTKey";
                countParameters.Add("ParentTKey", query.ParentTKey.Value);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ProductCategory>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分類列表失敗", ex);
            throw;
        }
    }

    public async Task<List<ProductCategory>> GetTreeAsync(ProductCategoryTreeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ProductCategories
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ClassType))
            {
                sql += " AND ClassType = @ClassType";
                parameters.Add("ClassType", query.ClassType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY ClassMode, ClassId";

            var items = await QueryAsync<ProductCategory>(sql, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分類樹狀結構失敗", ex);
            throw;
        }
    }

    public async Task<ProductCategory> CreateAsync(ProductCategory category)
    {
        try
        {
            const string sql = @"
                INSERT INTO ProductCategories (
                    ClassId, ClassName, ClassType, ClassMode, BClassId, MClassId, ParentTKey,
                    StypeId, StypeId2, DepreStypeId, DepreStypeId2, StypeTax,
                    ItemCount, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ClassId, @ClassName, @ClassType, @ClassMode, @BClassId, @MClassId, @ParentTKey,
                    @StypeId, @StypeId2, @DepreStypeId, @DepreStypeId2, @StypeTax,
                    @ItemCount, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<ProductCategory>(sql, category);
            if (result == null)
            {
                throw new InvalidOperationException("新增商品分類失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增商品分類失敗: {category.ClassId}", ex);
            throw;
        }
    }

    public async Task<ProductCategory> UpdateAsync(ProductCategory category)
    {
        try
        {
            const string sql = @"
                UPDATE ProductCategories SET
                    ClassName = @ClassName,
                    ClassType = @ClassType,
                    BClassId = @BClassId,
                    MClassId = @MClassId,
                    ParentTKey = @ParentTKey,
                    StypeId = @StypeId,
                    StypeId2 = @StypeId2,
                    DepreStypeId = @DepreStypeId,
                    DepreStypeId2 = @DepreStypeId2,
                    StypeTax = @StypeTax,
                    ItemCount = @ItemCount,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            var result = await QueryFirstOrDefaultAsync<ProductCategory>(sql, category);
            if (result == null)
            {
                throw new InvalidOperationException($"商品分類不存在: {category.TKey}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改商品分類失敗: {category.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM ProductCategories 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除商品分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string classId, string classMode, long? parentTKey, long? excludeTKey = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM ProductCategories 
                WHERE ClassId = @ClassId AND ClassMode = @ClassMode";
            
            var parameters = new DynamicParameters();
            parameters.Add("ClassId", classId);
            parameters.Add("ClassMode", classMode);

            if (parentTKey.HasValue)
            {
                sql += " AND ParentTKey = @ParentTKey";
                parameters.Add("ParentTKey", parentTKey.Value);
            }
            else
            {
                sql += " AND ParentTKey IS NULL";
            }

            if (excludeTKey.HasValue)
            {
                sql += " AND TKey != @ExcludeTKey";
                parameters.Add("ExcludeTKey", excludeTKey.Value);
            }

            var count = await QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查商品分類是否存在失敗: {classId}", ex);
            throw;
        }
    }

    public async Task<bool> HasChildrenAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ProductCategories 
                WHERE ParentTKey = @TKey";

            var count = await QuerySingleAsync<int>(sql, new { TKey = tKey });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查是否有子分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<ProductCategory>> GetBClassListAsync(ProductCategoryListQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ProductCategories
                WHERE ClassMode = '1'";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ClassType))
            {
                sql += " AND ClassType = @ClassType";
                parameters.Add("ClassType", query.ClassType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY ClassId";

            var items = await QueryAsync<ProductCategory>(sql, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢大分類列表失敗", ex);
            throw;
        }
    }

    public async Task<List<ProductCategory>> GetMClassListAsync(ProductCategoryListQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ProductCategories
                WHERE ClassMode = '2'";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.BClassId))
            {
                sql += " AND BClassId = @BClassId";
                parameters.Add("BClassId", query.BClassId);
            }

            if (!string.IsNullOrEmpty(query.ClassType))
            {
                sql += " AND ClassType = @ClassType";
                parameters.Add("ClassType", query.ClassType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY ClassId";

            var items = await QueryAsync<ProductCategory>(sql, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢中分類列表失敗", ex);
            throw;
        }
    }

    public async Task<List<ProductCategory>> GetSClassListAsync(ProductCategoryListQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ProductCategories
                WHERE ClassMode = '3'";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.BClassId))
            {
                sql += " AND BClassId = @BClassId";
                parameters.Add("BClassId", query.BClassId);
            }

            if (!string.IsNullOrEmpty(query.MClassId))
            {
                sql += " AND MClassId = @MClassId";
                parameters.Add("MClassId", query.MClassId);
            }

            if (!string.IsNullOrEmpty(query.ClassType))
            {
                sql += " AND ClassType = @ClassType";
                parameters.Add("ClassType", query.ClassType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY ClassId";

            var items = await QueryAsync<ProductCategory>(sql, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢小分類列表失敗", ex);
            throw;
        }
    }
}

