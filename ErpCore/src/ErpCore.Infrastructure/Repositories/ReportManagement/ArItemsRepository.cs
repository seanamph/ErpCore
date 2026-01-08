using System.Data;
using Dapper;
using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.ReportManagement;

/// <summary>
/// 收款項目 Repository 實作 (SYSR110-SYSR120)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ArItemsRepository : BaseRepository, IArItemsRepository
{
    public ArItemsRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ArItems?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ArItems 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ArItems>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款項目失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ArItems?> GetBySiteIdAndAritemIdAsync(string siteId, string aritemId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ArItems 
                WHERE SiteId = @SiteId AND AritemId = @AritemId";

            return await QueryFirstOrDefaultAsync<ArItems>(sql, new { SiteId = siteId, AritemId = aritemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款項目失敗: SiteId={siteId}, AritemId={aritemId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ArItems>> GetAllAsync()
    {
        try
        {
            const string sql = @"SELECT * FROM ArItems ORDER BY SiteId, AritemId";

            return await QueryAsync<ArItems>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢收款項目列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ArItems>> GetBySiteIdAsync(string siteId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ArItems 
                WHERE SiteId = @SiteId 
                ORDER BY AritemId";

            return await QueryAsync<ArItems>(sql, new { SiteId = siteId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款項目列表失敗: SiteId={siteId}", ex);
            throw;
        }
    }

    public async Task<ArItems> CreateAsync(ArItems entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO ArItems (SiteId, AritemId, AritemName, StypeId, Notes, Buser, Btime, Cpriority, Cgroup, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@SiteId, @AritemId, @AritemName, @StypeId, @Notes, @Buser, @Btime, @Cpriority, @Cgroup, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.SiteId,
                entity.AritemId,
                entity.AritemName,
                entity.StypeId,
                entity.Notes,
                entity.Buser,
                entity.Btime,
                entity.Cpriority,
                entity.Cgroup,
                entity.CreatedBy,
                entity.CreatedAt,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增收款項目失敗", ex);
            throw;
        }
    }

    public async Task<ArItems> UpdateAsync(ArItems entity)
    {
        try
        {
            const string sql = @"
                UPDATE ArItems 
                SET AritemName = @AritemName,
                    StypeId = @StypeId,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.AritemName,
                entity.StypeId,
                entity.Notes,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新收款項目失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM ArItems WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除收款項目失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string siteId, string aritemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) FROM ArItems 
                WHERE SiteId = @SiteId AND AritemId = @AritemId";

            var count = await ExecuteScalarAsync<int>(sql, new { SiteId = siteId, AritemId = aritemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查收款項目是否存在失敗: SiteId={siteId}, AritemId={aritemId}", ex);
            throw;
        }
    }
}

