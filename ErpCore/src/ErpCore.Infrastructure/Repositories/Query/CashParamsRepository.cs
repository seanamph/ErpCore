using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 零用金參數 Repository 實作 (SYSQ110)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CashParamsRepository : BaseRepository, ICashParamsRepository
{
    public CashParamsRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CashParams?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CashParams 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<CashParams>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金參數失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<CashParams>> GetAllAsync()
    {
        try
        {
            const string sql = @"SELECT * FROM CashParams ORDER BY TKey";

            return await QueryAsync<CashParams>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金參數列表失敗", ex);
            throw;
        }
    }

    public async Task<CashParams> CreateAsync(CashParams entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO CashParams (UnitId, ApexpLid, PtaxLid, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@UnitId, @ApexpLid, @PtaxLid, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.UnitId,
                entity.ApexpLid,
                entity.PtaxLid,
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
            _logger.LogError("新增零用金參數失敗", ex);
            throw;
        }
    }

    public async Task<CashParams> UpdateAsync(CashParams entity)
    {
        try
        {
            const string sql = @"
                UPDATE CashParams 
                SET UnitId = @UnitId,
                    ApexpLid = @ApexpLid,
                    PtaxLid = @PtaxLid,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.UnitId,
                entity.ApexpLid,
                entity.PtaxLid,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金參數失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM CashParams WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金參數失敗: {tKey}", ex);
            throw;
        }
    }
}

