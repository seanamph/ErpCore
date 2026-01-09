using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 基礎資料 Repository 實作 (SYS5110-SYS5150 - 基礎資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Std5000BaseDataRepository : BaseRepository, IStd5000BaseDataRepository
{
    public Std5000BaseDataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Std5000BaseData?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000BaseData 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Std5000BaseData>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000基礎資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Std5000BaseData?> GetByDataIdAndTypeAsync(string dataId, string dataType)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000BaseData 
                WHERE DataId = @DataId AND DataType = @DataType";

            return await QueryFirstOrDefaultAsync<Std5000BaseData>(sql, new { DataId = dataId, DataType = dataType });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000基礎資料失敗: {dataId}, {dataType}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Std5000BaseData>> QueryAsync(Std5000BaseDataQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Std5000BaseData
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
                sql += " ORDER BY DataType ASC, DataId ASC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Std5000BaseData>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000基礎資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Std5000BaseDataQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Std5000BaseData
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

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (DataId LIKE @Keyword OR DataName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000基礎資料總數失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Std5000BaseData entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Std5000BaseData 
                (DataId, DataName, DataType, Status, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@DataId, @DataName, @DataType, @Status, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("DataId", entity.DataId);
            parameters.Add("DataName", entity.DataName);
            parameters.Add("DataType", entity.DataType);
            parameters.Add("Status", entity.Status);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("CreatedAt", entity.CreatedAt);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            return await QuerySingleAsync<long>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000基礎資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Std5000BaseData entity)
    {
        try
        {
            const string sql = @"
                UPDATE Std5000BaseData 
                SET DataName = @DataName, 
                    Status = @Status, 
                    Memo = @Memo, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var parameters = new DynamicParameters();
            parameters.Add("TKey", entity.TKey);
            parameters.Add("DataName", entity.DataName);
            parameters.Add("Status", entity.Status);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改STD5000基礎資料失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Std5000BaseData 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000基礎資料失敗: {tKey}", ex);
            throw;
        }
    }
}

