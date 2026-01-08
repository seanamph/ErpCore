using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃條件 Repository 實作 (SYSE110-SYSE140)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseTermRepository : BaseRepository, ILeaseTermRepository
{
    public LeaseTermRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LeaseTerm?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseTerms 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<LeaseTerm>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃條件失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseTerm>> GetByLeaseIdAndVersionAsync(string leaseId, string version)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseTerms 
                WHERE LeaseId = @LeaseId AND Version = @Version
                ORDER BY TKey";

            return await QueryAsync<LeaseTerm>(sql, new { LeaseId = leaseId, Version = version });
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號和版本查詢條件失敗: {leaseId}, {version}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseTerm>> QueryAsync(LeaseTermQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LeaseTerms 
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

            if (!string.IsNullOrEmpty(query.TermType))
            {
                sql += " AND TermType = @TermType";
                parameters.Add("TermType", query.TermType);
            }

            sql += " ORDER BY TKey DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LeaseTerm>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃條件列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseTermQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LeaseTerms 
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

            if (!string.IsNullOrEmpty(query.TermType))
            {
                sql += " AND TermType = @TermType";
                parameters.Add("TermType", query.TermType);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃條件數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM LeaseTerms 
                WHERE TKey = @TKey";

            var count = await ExecuteScalarAsync<int>(sql, new { TKey = tKey });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃條件是否存在失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<LeaseTerm> CreateAsync(LeaseTerm leaseTerm)
    {
        try
        {
            const string sql = @"
                INSERT INTO LeaseTerms (
                    LeaseId, Version, TermType, TermName, TermValue, TermAmount, TermDate, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @LeaseId, @Version, @TermType, @TermName, @TermValue, @TermAmount, @TermDate, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                leaseTerm.LeaseId,
                leaseTerm.Version,
                leaseTerm.TermType,
                leaseTerm.TermName,
                leaseTerm.TermValue,
                leaseTerm.TermAmount,
                leaseTerm.TermDate,
                leaseTerm.Memo,
                leaseTerm.CreatedBy,
                leaseTerm.CreatedAt,
                leaseTerm.UpdatedBy,
                leaseTerm.UpdatedAt
            });

            leaseTerm.TKey = tKey;
            return leaseTerm;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃條件失敗: {leaseTerm.LeaseId}", ex);
            throw;
        }
    }

    public async Task<LeaseTerm> UpdateAsync(LeaseTerm leaseTerm)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseTerms SET
                    TermType = @TermType,
                    TermName = @TermName,
                    TermValue = @TermValue,
                    TermAmount = @TermAmount,
                    TermDate = @TermDate,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                leaseTerm.TKey,
                leaseTerm.TermType,
                leaseTerm.TermName,
                leaseTerm.TermValue,
                leaseTerm.TermAmount,
                leaseTerm.TermDate,
                leaseTerm.Memo,
                leaseTerm.UpdatedBy,
                leaseTerm.UpdatedAt
            });

            return leaseTerm;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃條件失敗: {leaseTerm.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseTerms 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃條件失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteByLeaseIdAndVersionAsync(string leaseId, string version)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseTerms 
                WHERE LeaseId = @LeaseId AND Version = @Version";

            await ExecuteAsync(sql, new { LeaseId = leaseId, Version = version });
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號和版本刪除條件失敗: {leaseId}, {version}", ex);
            throw;
        }
    }
}

