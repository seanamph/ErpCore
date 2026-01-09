using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerCustomJgjn;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;

/// <summary>
/// JGJN資料 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class JgjNDataRepository : BaseRepository, IJgjNDataRepository
{
    public JgjNDataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<JgjNData?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM JgjNData 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<JgjNData>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<JgjNData?> GetByDataIdAndModuleCodeAsync(string dataId, string moduleCode)
    {
        try
        {
            const string sql = @"
                SELECT * FROM JgjNData 
                WHERE DataId = @DataId AND ModuleCode = @ModuleCode";

            return await QueryFirstOrDefaultAsync<JgjNData>(sql, new { DataId = dataId, ModuleCode = moduleCode });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN資料失敗: {dataId}, {moduleCode}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<JgjNData>> QueryAsync(JgjNDataQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM JgjNData
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ModuleCode))
            {
                sql += " AND ModuleCode = @ModuleCode";
                parameters.Add("ModuleCode", query.ModuleCode);
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

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (DataId LIKE @Keyword OR DataName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY CreatedAt DESC";
            }

            // 分頁
            if (query.PageSize > 0)
            {
                var offset = (query.PageIndex - 1) * query.PageSize;
                sql += $" OFFSET {offset} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";
            }

            return await QueryAsync<JgjNData>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(JgjNDataQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM JgjNData
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ModuleCode))
            {
                sql += " AND ModuleCode = @ModuleCode";
                parameters.Add("ModuleCode", query.ModuleCode);
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

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (DataId LIKE @Keyword OR DataName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN資料數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(JgjNData entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO JgjNData 
                (DataId, ModuleCode, DataName, DataValue, DataType, Status, SortOrder, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@DataId, @ModuleCode, @DataName, @DataValue, @DataType, @Status, @SortOrder, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            return await ExecuteScalarAsync<long>(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增JGJN資料失敗: {entity.DataId}, {entity.ModuleCode}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(JgjNData entity)
    {
        try
        {
            const string sql = @"
                UPDATE JgjNData 
                SET DataName = @DataName, DataValue = @DataValue, DataType = @DataType, 
                    Status = @Status, SortOrder = @SortOrder, Memo = @Memo, 
                    UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新JGJN資料失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM JgjNData 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除JGJN資料失敗: {tKey}", ex);
            throw;
        }
    }
}

