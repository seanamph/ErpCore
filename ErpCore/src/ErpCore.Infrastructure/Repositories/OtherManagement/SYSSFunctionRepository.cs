using Dapper;
using ErpCore.Domain.Entities.OtherManagement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.OtherManagement;

/// <summary>
/// S系統功能 Repository 實作 (SYSS000)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SYSSFunctionRepository : BaseRepository, ISYSSFunctionRepository
{
    public SYSSFunctionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SYSSFunction?> GetByIdAsync(long tKey)
    {
        const string sql = @"
            SELECT * FROM SYSSFunctions 
            WHERE TKey = @TKey";

        return await QueryFirstOrDefaultAsync<SYSSFunction>(sql, new { TKey = tKey });
    }

    public async Task<SYSSFunction?> GetByFunctionIdAsync(string functionId)
    {
        const string sql = @"
            SELECT * FROM SYSSFunctions 
            WHERE FunctionId = @FunctionId";

        return await QueryFirstOrDefaultAsync<SYSSFunction>(sql, new { FunctionId = functionId });
    }

    public async Task<IEnumerable<SYSSFunction>> QueryAsync(SYSSFunctionQuery query)
    {
        var sql = @"
            SELECT * FROM SYSSFunctions 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.FunctionId))
        {
            sql += " AND FunctionId LIKE @FunctionId";
            parameters.Add("FunctionId", $"%{query.FunctionId}%");
        }

        if (!string.IsNullOrEmpty(query.FunctionName))
        {
            sql += " AND FunctionName LIKE @FunctionName";
            parameters.Add("FunctionName", $"%{query.FunctionName}%");
        }

        if (!string.IsNullOrEmpty(query.FunctionType))
        {
            sql += " AND FunctionType = @FunctionType";
            parameters.Add("FunctionType", query.FunctionType);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", query.Status);
        }

        // 排序
        var sortField = string.IsNullOrEmpty(query.SortField) ? "SeqNo" : query.SortField;
        var sortOrder = string.IsNullOrEmpty(query.SortOrder) ? "ASC" : query.SortOrder;
        sql += $" ORDER BY {sortField} {sortOrder}";

        // 分頁
        var offset = (query.PageIndex - 1) * query.PageSize;
        sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<SYSSFunction>(sql, parameters);
    }

    public async Task<int> GetCountAsync(SYSSFunctionQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM SYSSFunctions 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.FunctionId))
        {
            sql += " AND FunctionId LIKE @FunctionId";
            parameters.Add("FunctionId", $"%{query.FunctionId}%");
        }

        if (!string.IsNullOrEmpty(query.FunctionName))
        {
            sql += " AND FunctionName LIKE @FunctionName";
            parameters.Add("FunctionName", $"%{query.FunctionName}%");
        }

        if (!string.IsNullOrEmpty(query.FunctionType))
        {
            sql += " AND FunctionType = @FunctionType";
            parameters.Add("FunctionType", query.FunctionType);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", query.Status);
        }

        return await ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<long> CreateAsync(SYSSFunction entity)
    {
        const string sql = @"
            INSERT INTO SYSSFunctions 
            (FunctionId, FunctionName, FunctionType, FunctionValue, FunctionConfig, 
             SeqNo, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, 
             CreatedPriority, CreatedGroup)
            VALUES 
            (@FunctionId, @FunctionName, @FunctionType, @FunctionValue, @FunctionConfig, 
             @SeqNo, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 
             @CreatedPriority, @CreatedGroup);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        return await ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task UpdateAsync(SYSSFunction entity)
    {
        const string sql = @"
            UPDATE SYSSFunctions SET
                FunctionName = @FunctionName,
                FunctionType = @FunctionType,
                FunctionValue = @FunctionValue,
                FunctionConfig = @FunctionConfig,
                SeqNo = @SeqNo,
                Status = @Status,
                Notes = @Notes,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = @UpdatedAt,
                CreatedPriority = @CreatedPriority,
                CreatedGroup = @CreatedGroup
            WHERE TKey = @TKey";

        await ExecuteAsync(sql, entity);
    }

    public async Task DeleteAsync(long tKey)
    {
        const string sql = @"
            DELETE FROM SYSSFunctions 
            WHERE TKey = @TKey";

        await ExecuteAsync(sql, new { TKey = tKey });
    }

    public async Task<bool> ExistsAsync(string functionId)
    {
        const string sql = @"
            SELECT COUNT(*) FROM SYSSFunctions 
            WHERE FunctionId = @FunctionId";

        var count = await ExecuteScalarAsync<int>(sql, new { FunctionId = functionId });
        return count > 0;
    }
}

