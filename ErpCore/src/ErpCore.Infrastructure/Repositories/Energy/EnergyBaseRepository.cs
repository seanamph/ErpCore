using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Energy;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Energy;

/// <summary>
/// 能源基礎 Repository 實作 (SYSO100-SYSO130 - 能源基礎功能)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EnergyBaseRepository : BaseRepository, IEnergyBaseRepository
{
    public EnergyBaseRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EnergyBase?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EnergyBases 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<EnergyBase>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源基礎資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EnergyBase?> GetByEnergyIdAsync(string energyId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EnergyBases 
                WHERE EnergyId = @EnergyId";

            return await QueryFirstOrDefaultAsync<EnergyBase>(sql, new { EnergyId = energyId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源基礎資料失敗: {energyId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EnergyBase>> QueryAsync(EnergyBaseQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EnergyBases
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.EnergyId))
            {
                sql += " AND EnergyId LIKE @EnergyId";
                parameters.Add("EnergyId", $"%{query.EnergyId}%");
            }

            if (!string.IsNullOrEmpty(query.EnergyName))
            {
                sql += " AND EnergyName LIKE @EnergyName";
                parameters.Add("EnergyName", $"%{query.EnergyName}%");
            }

            if (!string.IsNullOrEmpty(query.EnergyType))
            {
                sql += " AND EnergyType = @EnergyType";
                parameters.Add("EnergyType", query.EnergyType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY EnergyId ASC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<EnergyBase>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢能源基礎資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(EnergyBaseQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM EnergyBases
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.EnergyId))
            {
                sql += " AND EnergyId LIKE @EnergyId";
                parameters.Add("EnergyId", $"%{query.EnergyId}%");
            }

            if (!string.IsNullOrEmpty(query.EnergyName))
            {
                sql += " AND EnergyName LIKE @EnergyName";
                parameters.Add("EnergyName", $"%{query.EnergyName}%");
            }

            if (!string.IsNullOrEmpty(query.EnergyType))
            {
                sql += " AND EnergyType = @EnergyType";
                parameters.Add("EnergyType", query.EnergyType);
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
            _logger.LogError("查詢能源基礎資料數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(EnergyBase entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO EnergyBases 
                (EnergyId, EnergyName, EnergyType, Unit, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@EnergyId, @EnergyName, @EnergyType, @Unit, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增能源基礎資料失敗: {entity.EnergyId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(EnergyBase entity)
    {
        try
        {
            const string sql = @"
                UPDATE EnergyBases 
                SET EnergyName = @EnergyName, 
                    EnergyType = @EnergyType, 
                    Unit = @Unit, 
                    Status = @Status, 
                    Notes = @Notes, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改能源基礎資料失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM EnergyBases 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除能源基礎資料失敗: {tKey}", ex);
            throw;
        }
    }
}

