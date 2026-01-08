using Dapper;
using ErpCore.Domain.Entities.OtherManagement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.OtherManagement;

/// <summary>
/// J系統功能 Repository 實作 (SYSJ000)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SYSJFunctionRepository : BaseRepository, ISYSJFunctionRepository
{
    public SYSJFunctionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SYSJFunction?> GetByIdAsync(long tKey)
    {
        const string sql = @"
            SELECT * FROM SYSJFunctions 
            WHERE TKey = @TKey";

        return await QueryFirstOrDefaultAsync<SYSJFunction>(sql, new { TKey = tKey });
    }

    public async Task<SYSJFunction?> GetByFunctionIdAsync(string functionId)
    {
        const string sql = @"
            SELECT * FROM SYSJFunctions 
            WHERE FunctionId = @FunctionId";

        return await QueryFirstOrDefaultAsync<SYSJFunction>(sql, new { FunctionId = functionId });
    }

    public async Task<IEnumerable<SYSJFunction>> QueryAsync(SYSJFunctionQuery query)
    {
        var sql = @"
            SELECT * FROM SYSJFunctions 
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

        return await QueryAsync<SYSJFunction>(sql, parameters);
    }

    public async Task<int> GetCountAsync(SYSJFunctionQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM SYSJFunctions 
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

    public async Task<long> CreateAsync(SYSJFunction entity)
    {
        const string sql = @"
            INSERT INTO SYSJFunctions 
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

    public async Task UpdateAsync(SYSJFunction entity)
    {
        const string sql = @"
            UPDATE SYSJFunctions SET
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
            DELETE FROM SYSJFunctions 
            WHERE TKey = @TKey";

        await ExecuteAsync(sql, new { TKey = tKey });
    }

    public async Task<bool> ExistsAsync(string functionId)
    {
        const string sql = @"
            SELECT COUNT(*) FROM SYSJFunctions 
            WHERE FunctionId = @FunctionId";

        var count = await ExecuteScalarAsync<int>(sql, new { FunctionId = functionId });
        return count > 0;
    }
}

