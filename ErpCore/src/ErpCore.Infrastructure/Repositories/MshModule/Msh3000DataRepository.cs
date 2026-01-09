using System.Data;
using Dapper;
using ErpCore.Domain.Entities.MshModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.MshModule;

/// <summary>
/// MSH3000 資料 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Msh3000DataRepository : BaseRepository, IMsh3000DataRepository
{
    public Msh3000DataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Msh3000Data?> GetByIdAsync(string dataId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Msh3000Data 
                WHERE DataId = @DataId";

            return await QueryFirstOrDefaultAsync<Msh3000Data>(sql, new { DataId = dataId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢MSH3000資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Msh3000Data>> QueryAsync(Msh3000DataQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Msh3000Data 
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

            return await QueryAsync<Msh3000Data>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢MSH3000資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Msh3000DataQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Msh3000Data 
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
            _logger.LogError("查詢MSH3000資料數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(Msh3000Data entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Msh3000Data 
                (DataId, DataName, DataValue, DataType, ImagePath, Status, SortOrder, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@DataId, @DataName, @DataValue, @DataType, @ImagePath, @Status, @SortOrder, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return entity.DataId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增MSH3000資料失敗: {entity.DataId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Msh3000Data entity)
    {
        try
        {
            const string sql = @"
                UPDATE Msh3000Data 
                SET DataName = @DataName,
                    DataValue = @DataValue,
                    DataType = @DataType,
                    ImagePath = @ImagePath,
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
            _logger.LogError($"修改MSH3000資料失敗: {entity.DataId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string dataId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Msh3000Data 
                WHERE DataId = @DataId";

            await ExecuteAsync(sql, new { DataId = dataId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除MSH3000資料失敗: {dataId}", ex);
            throw;
        }
    }
}

