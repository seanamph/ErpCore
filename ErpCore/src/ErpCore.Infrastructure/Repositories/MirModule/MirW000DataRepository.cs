using System.Data;
using Dapper;
using ErpCore.Domain.Entities.MirModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.MirModule;

/// <summary>
/// MIRW000 資料 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class MirW000DataRepository : BaseRepository, IMirW000DataRepository
{
    public MirW000DataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<MirW000Data?> GetByIdAsync(string dataId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM MirW000Data 
                WHERE DataId = @DataId";

            return await QueryFirstOrDefaultAsync<MirW000Data>(sql, new { DataId = dataId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢MIRW000資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<MirW000Data>> QueryAsync(MirW000DataQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM MirW000Data 
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

            return await QueryAsync<MirW000Data>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢MIRW000資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(MirW000DataQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM MirW000Data 
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
            _logger.LogError("查詢MIRW000資料數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(MirW000Data entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO MirW000Data 
                (DataId, DataName, DataValue, DataType, Status, SortOrder, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@DataId, @DataName, @DataValue, @DataType, @Status, @SortOrder, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return entity.DataId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增MIRW000資料失敗: {entity.DataId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(MirW000Data entity)
    {
        try
        {
            const string sql = @"
                UPDATE MirW000Data 
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
            _logger.LogError($"修改MIRW000資料失敗: {entity.DataId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string dataId)
    {
        try
        {
            const string sql = @"
                DELETE FROM MirW000Data 
                WHERE DataId = @DataId";

            await ExecuteAsync(sql, new { DataId = dataId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除MIRW000資料失敗: {dataId}", ex);
            throw;
        }
    }
}

