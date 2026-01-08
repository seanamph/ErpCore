using Dapper;
using ErpCore.Domain.Entities.OtherManagement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.OtherManagement;

/// <summary>
/// U系統功能 Repository 實作 (SYSU000)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SYSUFunctionRepository : BaseRepository, ISYSUFunctionRepository
{
    public SYSUFunctionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SYSUFunction?> GetByIdAsync(long tKey)
    {
        const string sql = @"
            SELECT * FROM SYSUFunctions 
            WHERE TKey = @TKey";

        return await QueryFirstOrDefaultAsync<SYSUFunction>(sql, new { TKey = tKey });
    }

    public async Task<SYSUFunction?> GetByFunctionIdAsync(string functionId)
    {
        const string sql = @"
            SELECT * FROM SYSUFunctions 
            WHERE FunctionId = @FunctionId";

        return await QueryFirstOrDefaultAsync<SYSUFunction>(sql, new { FunctionId = functionId });
    }

    public async Task<IEnumerable<SYSUFunction>> QueryAsync(SYSUFunctionQuery query)
    {
        var sql = @"
            SELECT * FROM SYSUFunctions 
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

        if (!string.IsNullOrEmpty(query.FunctionCategory))
        {
            sql += " AND FunctionCategory = @FunctionCategory";
            parameters.Add("FunctionCategory", query.FunctionCategory);
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

        return await QueryAsync<SYSUFunction>(sql, parameters);
    }

    public async Task<int> GetCountAsync(SYSUFunctionQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM SYSUFunctions 
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

        if (!string.IsNullOrEmpty(query.FunctionCategory))
        {
            sql += " AND FunctionCategory = @FunctionCategory";
            parameters.Add("FunctionCategory", query.FunctionCategory);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", query.Status);
        }

        return await ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<long> CreateAsync(SYSUFunction entity)
    {
        const string sql = @"
            INSERT INTO SYSUFunctions 
            (FunctionId, FunctionName, FunctionType, FunctionCategory, FunctionValue, FunctionConfig, 
             SeqNo, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, 
             CreatedPriority, CreatedGroup)
            VALUES 
            (@FunctionId, @FunctionName, @FunctionType, @FunctionCategory, @FunctionValue, @FunctionConfig, 
             @SeqNo, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 
             @CreatedPriority, @CreatedGroup);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        return await ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task UpdateAsync(SYSUFunction entity)
    {
        const string sql = @"
            UPDATE SYSUFunctions SET
                FunctionName = @FunctionName,
                FunctionType = @FunctionType,
                FunctionCategory = @FunctionCategory,
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
            DELETE FROM SYSUFunctions 
            WHERE TKey = @TKey";

        await ExecuteAsync(sql, new { TKey = tKey });
    }

    public async Task<bool> ExistsAsync(string functionId)
    {
        const string sql = @"
            SELECT COUNT(*) FROM SYSUFunctions 
            WHERE FunctionId = @FunctionId";

        var count = await ExecuteScalarAsync<int>(sql, new { FunctionId = functionId });
        return count > 0;
    }
}

