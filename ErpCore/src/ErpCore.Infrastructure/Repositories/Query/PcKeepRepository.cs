using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 保管人及額度設定 Repository 實作 (SYSQ120)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PcKeepRepository : BaseRepository, IPcKeepRepository
{
    public PcKeepRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PcKeep?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcKeep 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<PcKeep>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢保管人及額度失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcKeep?> GetByKeepEmpIdAsync(string keepEmpId, string? siteId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcKeep 
                WHERE KeepEmpId = @KeepEmpId 
                AND (@SiteId IS NULL OR SiteId = @SiteId)";

            return await QueryFirstOrDefaultAsync<PcKeep>(sql, new { KeepEmpId = keepEmpId, SiteId = siteId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢保管人及額度失敗: {keepEmpId}, {siteId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PcKeep>> QueryAsync(PcKeepQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PcKeep 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.KeepEmpId))
            {
                sql += " AND KeepEmpId LIKE @KeepEmpId";
                parameters.Add("KeepEmpId", $"%{query.KeepEmpId}%");
            }

            sql += " ORDER BY SiteId, KeepEmpId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<PcKeep>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢保管人及額度列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PcKeepQuery query)
    {
        try
        {
            var sql = @"SELECT COUNT(*) FROM PcKeep WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.KeepEmpId))
            {
                sql += " AND KeepEmpId LIKE @KeepEmpId";
                parameters.Add("KeepEmpId", $"%{query.KeepEmpId}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢保管人及額度數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string keepEmpId, string? siteId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PcKeep 
                WHERE KeepEmpId = @KeepEmpId 
                AND (@SiteId IS NULL OR SiteId = @SiteId)";

            var count = await ExecuteScalarAsync<int>(sql, new { KeepEmpId = keepEmpId, SiteId = siteId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查保管人及額度是否存在失敗: {keepEmpId}, {siteId}", ex);
            throw;
        }
    }

    public async Task<PcKeep> CreateAsync(PcKeep entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO PcKeep (SiteId, KeepEmpId, PcQuota, Notes, BUser, BTime, CUser, CTime, CPriority, CGroup)
                VALUES (@SiteId, @KeepEmpId, @PcQuota, @Notes, @BUser, @BTime, @CUser, @CTime, @CPriority, @CGroup);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.SiteId,
                entity.KeepEmpId,
                entity.PcQuota,
                entity.Notes,
                entity.BUser,
                entity.BTime,
                entity.CUser,
                entity.CTime,
                entity.CPriority,
                entity.CGroup
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增保管人及額度失敗", ex);
            throw;
        }
    }

    public async Task<PcKeep> UpdateAsync(PcKeep entity)
    {
        try
        {
            const string sql = @"
                UPDATE PcKeep 
                SET PcQuota = @PcQuota,
                    Notes = @Notes,
                    CUser = @CUser,
                    CTime = @CTime
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.PcQuota,
                entity.Notes,
                entity.CUser,
                entity.CTime
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改保管人及額度失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM PcKeep WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除保管人及額度失敗: {tKey}", ex);
            throw;
        }
    }
}

