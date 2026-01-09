using System.Data;
using Dapper;
using ErpCore.Domain.Entities.MirModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.MirModule;

/// <summary>
/// MIRV000 資料 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class MirV000DataRepository : BaseRepository, IMirV000DataRepository
{
    public MirV000DataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<MirV000Data?> GetByIdAsync(string dataId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM MirV000Data 
                WHERE DataId = @DataId";

            return await QueryFirstOrDefaultAsync<MirV000Data>(sql, new { DataId = dataId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢MIRV000資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<MirV000Data>> QueryAsync(MirV000DataQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM MirV000Data 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.DataId))
            {
                sql += " AND DataId LIKE @DataId";
                parameters.Add("DataId", $"%{query.DataId}%");
            }

            if (!string.IsNullOrEmpty(query.DataName))
            {
                sql += " AND DataName LIKE @DataName";
                parameters.Add("DataName", $"%{query.DataName}%");
            }

            if (!string.IsNullOrEmpty(query.DataType))
            {
                sql += " AND DataType = @DataType";
                parameters.Add("DataType", query.DataType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY SortOrder, DataId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var offset = (query.PageIndex - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<MirV000Data>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢MIRV000資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(MirV000DataQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM MirV000Data 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.DataId))
            {
                sql += " AND DataId LIKE @DataId";
                parameters.Add("DataId", $"%{query.DataId}%");
            }

            if (!string.IsNullOrEmpty(query.DataName))
            {
                sql += " AND DataName LIKE @DataName";
                parameters.Add("DataName", $"%{query.DataName}%");
            }

            if (!string.IsNullOrEmpty(query.DataType))
            {
                sql += " AND DataType = @DataType";
                parameters.Add("DataType", query.DataType);
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
            _logger.LogError("查詢MIRV000資料數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(MirV000Data entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO MirV000Data 
                (DataId, DataName, DataValue, DataType, Status, SortOrder, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@DataId, @DataName, @DataValue, @DataType, @Status, @SortOrder, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return entity.DataId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增MIRV000資料失敗: {entity.DataId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(MirV000Data entity)
    {
        try
        {
            const string sql = @"
                UPDATE MirV000Data 
                SET DataName = @DataName,
                    DataValue = @DataValue,
                    DataType = @DataType,
                    Status = @Status,
                    SortOrder = @SortOrder,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE DataId = @DataId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改MIRV000資料失敗: {entity.DataId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string dataId)
    {
        try
        {
            const string sql = @"
                DELETE FROM MirV000Data 
                WHERE DataId = @DataId";

            await ExecuteAsync(sql, new { DataId = dataId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除MIRV000資料失敗: {dataId}", ex);
            throw;
        }
    }
}

