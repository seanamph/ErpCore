using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃會計分類 Repository 實作 (SYSE110-SYSE140)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseAccountingCategoryRepository : BaseRepository, ILeaseAccountingCategoryRepository
{
    public LeaseAccountingCategoryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LeaseAccountingCategory?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseAccountingCategories 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<LeaseAccountingCategory>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃會計分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseAccountingCategory>> GetByLeaseIdAndVersionAsync(string leaseId, string version)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseAccountingCategories 
                WHERE LeaseId = @LeaseId AND Version = @Version
                ORDER BY TKey";

            return await QueryAsync<LeaseAccountingCategory>(sql, new { LeaseId = leaseId, Version = version });
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號和版本查詢會計分類失敗: {leaseId}, {version}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseAccountingCategory>> QueryAsync(LeaseAccountingCategoryQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LeaseAccountingCategories 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId LIKE @LeaseId";
                parameters.Add("LeaseId", $"%{query.LeaseId}%");
            }

            if (!string.IsNullOrEmpty(query.Version))
            {
                sql += " AND Version = @Version";
                parameters.Add("Version", query.Version);
            }

            if (!string.IsNullOrEmpty(query.CategoryId))
            {
                sql += " AND CategoryId LIKE @CategoryId";
                parameters.Add("CategoryId", $"%{query.CategoryId}%");
            }

            sql += " ORDER BY TKey DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LeaseAccountingCategory>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃會計分類列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseAccountingCategoryQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LeaseAccountingCategories 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId LIKE @LeaseId";
                parameters.Add("LeaseId", $"%{query.LeaseId}%");
            }

            if (!string.IsNullOrEmpty(query.Version))
            {
                sql += " AND Version = @Version";
                parameters.Add("Version", query.Version);
            }

            if (!string.IsNullOrEmpty(query.CategoryId))
            {
                sql += " AND CategoryId LIKE @CategoryId";
                parameters.Add("CategoryId", $"%{query.CategoryId}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃會計分類數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM LeaseAccountingCategories 
                WHERE TKey = @TKey";

            var count = await ExecuteScalarAsync<int>(sql, new { TKey = tKey });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃會計分類是否存在失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<LeaseAccountingCategory> CreateAsync(LeaseAccountingCategory category)
    {
        try
        {
            const string sql = @"
                INSERT INTO LeaseAccountingCategories (
                    LeaseId, Version, CategoryId, CategoryName, Amount, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @LeaseId, @Version, @CategoryId, @CategoryName, @Amount, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                category.LeaseId,
                category.Version,
                category.CategoryId,
                category.CategoryName,
                category.Amount,
                category.Memo,
                category.CreatedBy,
                category.CreatedAt,
                category.UpdatedBy,
                category.UpdatedAt
            });

            category.TKey = tKey;
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃會計分類失敗: {category.LeaseId}", ex);
            throw;
        }
    }

    public async Task<LeaseAccountingCategory> UpdateAsync(LeaseAccountingCategory category)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseAccountingCategories SET
                    CategoryId = @CategoryId,
                    CategoryName = @CategoryName,
                    Amount = @Amount,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                category.TKey,
                category.CategoryId,
                category.CategoryName,
                category.Amount,
                category.Memo,
                category.UpdatedBy,
                category.UpdatedAt
            });

            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃會計分類失敗: {category.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseAccountingCategories 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃會計分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteByLeaseIdAndVersionAsync(string leaseId, string version)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseAccountingCategories 
                WHERE LeaseId = @LeaseId AND Version = @Version";

            await ExecuteAsync(sql, new { LeaseId = leaseId, Version = version });
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號和版本刪除會計分類失敗: {leaseId}, {version}", ex);
            throw;
        }
    }
}

