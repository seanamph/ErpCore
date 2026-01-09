using System.Data;
using Dapper;
using ErpCore.Domain.Entities.UniversalModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.UniversalModule;

/// <summary>
/// 通用模組資料 Repository 實作 (UNIV000系列)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Univ000Repository : BaseRepository, IUniv000Repository
{
    public Univ000Repository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Univ000?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"SELECT * FROM Univ000 WHERE TKey = @TKey";
            return await QueryFirstOrDefaultAsync<Univ000>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢通用模組資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Univ000?> GetByDataIdAsync(string dataId)
    {
        try
        {
            const string sql = @"SELECT * FROM Univ000 WHERE DataId = @DataId";
            return await QueryFirstOrDefaultAsync<Univ000>(sql, new { DataId = dataId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢通用模組資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Univ000>> QueryAsync(Univ000Query query)
    {
        try
        {
            var sql = @"SELECT * FROM Univ000 WHERE 1=1";
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

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (DataId LIKE @Keyword OR DataName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            sql += string.IsNullOrEmpty(query.SortField) 
                ? " ORDER BY SortOrder ASC, DataId ASC"
                : $" ORDER BY {query.SortField} {(query.SortOrder == "DESC" ? "DESC" : "ASC")}";

            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Univ000>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢通用模組資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Univ000Query query)
    {
        try
        {
            var sql = @"SELECT COUNT(*) FROM Univ000 WHERE 1=1";
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

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (DataId LIKE @Keyword OR DataName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢通用模組資料總數失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Univ000 entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Univ000 (DataId, DataName, DataType, DataValue, Status, SortOrder, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@DataId, @DataName, @DataType, @DataValue, @Status, @SortOrder, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("DataId", entity.DataId);
            parameters.Add("DataName", entity.DataName);
            parameters.Add("DataType", entity.DataType);
            parameters.Add("DataValue", entity.DataValue);
            parameters.Add("Status", entity.Status);
            parameters.Add("SortOrder", entity.SortOrder);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("CreatedAt", entity.CreatedAt);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            return await QuerySingleAsync<long>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增通用模組資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Univ000 entity)
    {
        try
        {
            const string sql = @"
                UPDATE Univ000 
                SET DataName = @DataName, DataType = @DataType, DataValue = @DataValue, Status = @Status, 
                    SortOrder = @SortOrder, Memo = @Memo, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var parameters = new DynamicParameters();
            parameters.Add("TKey", entity.TKey);
            parameters.Add("DataName", entity.DataName);
            parameters.Add("DataType", entity.DataType);
            parameters.Add("DataValue", entity.DataValue);
            parameters.Add("Status", entity.Status);
            parameters.Add("SortOrder", entity.SortOrder);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改通用模組資料失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM Univ000 WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除通用模組資料失敗: {tKey}", ex);
            throw;
        }
    }
}

