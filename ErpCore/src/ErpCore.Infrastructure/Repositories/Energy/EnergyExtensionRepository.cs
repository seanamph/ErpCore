using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Energy;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Energy;

/// <summary>
/// 能源擴展 Repository 實作 (SYSOU10-SYSOU33 - 能源擴展功能)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EnergyExtensionRepository : BaseRepository, IEnergyExtensionRepository
{
    public EnergyExtensionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EnergyExtension?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EnergyExtensions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<EnergyExtension>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源擴展資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EnergyExtension?> GetByExtensionIdAsync(string extensionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EnergyExtensions 
                WHERE ExtensionId = @ExtensionId";

            return await QueryFirstOrDefaultAsync<EnergyExtension>(sql, new { ExtensionId = extensionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源擴展資料失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EnergyExtension>> QueryAsync(EnergyExtensionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EnergyExtensions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ExtensionId))
            {
                sql += " AND ExtensionId LIKE @ExtensionId";
                parameters.Add("ExtensionId", $"%{query.ExtensionId}%");
            }

            if (!string.IsNullOrEmpty(query.EnergyId))
            {
                sql += " AND EnergyId = @EnergyId";
                parameters.Add("EnergyId", query.EnergyId);
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
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
                sql += " ORDER BY ExtensionId ASC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<EnergyExtension>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢能源擴展資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(EnergyExtensionQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM EnergyExtensions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ExtensionId))
            {
                sql += " AND ExtensionId LIKE @ExtensionId";
                parameters.Add("ExtensionId", $"%{query.ExtensionId}%");
            }

            if (!string.IsNullOrEmpty(query.EnergyId))
            {
                sql += " AND EnergyId = @EnergyId";
                parameters.Add("EnergyId", query.EnergyId);
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
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
            _logger.LogError("查詢能源擴展資料數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(EnergyExtension entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO EnergyExtensions 
                (ExtensionId, EnergyId, ExtensionType, ExtensionValue, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ExtensionId, @EnergyId, @ExtensionType, @ExtensionValue, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增能源擴展資料失敗: {entity.ExtensionId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(EnergyExtension entity)
    {
        try
        {
            const string sql = @"
                UPDATE EnergyExtensions 
                SET EnergyId = @EnergyId, 
                    ExtensionType = @ExtensionType, 
                    ExtensionValue = @ExtensionValue, 
                    Status = @Status, 
                    Notes = @Notes, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改能源擴展資料失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM EnergyExtensions 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除能源擴展資料失敗: {tKey}", ex);
            throw;
        }
    }
}

