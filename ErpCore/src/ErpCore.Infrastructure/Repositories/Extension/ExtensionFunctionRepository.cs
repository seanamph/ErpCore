using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Extension;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Extension;

/// <summary>
/// 擴展功能 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ExtensionFunctionRepository : BaseRepository, IExtensionFunctionRepository
{
    public ExtensionFunctionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ExtensionFunction?> GetByIdAsync(long tKey)
    {
        const string sql = @"
            SELECT * FROM ExtensionFunctions 
            WHERE TKey = @TKey";

        return await QueryFirstOrDefaultAsync<ExtensionFunction>(sql, new { TKey = tKey });
    }

    public async Task<ExtensionFunction?> GetByExtensionIdAsync(string extensionId)
    {
        const string sql = @"
            SELECT * FROM ExtensionFunctions 
            WHERE ExtensionId = @ExtensionId";

        return await QueryFirstOrDefaultAsync<ExtensionFunction>(sql, new { ExtensionId = extensionId });
    }

    public async Task<IEnumerable<ExtensionFunction>> QueryAsync(ExtensionFunctionQuery query)
    {
        var sql = @"
            SELECT * FROM ExtensionFunctions 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ExtensionId))
        {
            sql += " AND ExtensionId LIKE @ExtensionId";
            parameters.Add("ExtensionId", $"%{query.ExtensionId}%");
        }

        if (!string.IsNullOrEmpty(query.ExtensionName))
        {
            sql += " AND ExtensionName LIKE @ExtensionName";
            parameters.Add("ExtensionName", $"%{query.ExtensionName}%");
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
        var sortField = string.IsNullOrEmpty(query.SortField) ? "SeqNo" : query.SortField;
        var sortOrder = string.IsNullOrEmpty(query.SortOrder) ? "ASC" : query.SortOrder;
        sql += $" ORDER BY {sortField} {sortOrder}";

        // 分頁
        var offset = (query.PageIndex - 1) * query.PageSize;
        sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<ExtensionFunction>(sql, parameters);
    }

    public async Task<int> GetCountAsync(ExtensionFunctionQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM ExtensionFunctions 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.ExtensionId))
        {
            sql += " AND ExtensionId LIKE @ExtensionId";
            parameters.Add("ExtensionId", $"%{query.ExtensionId}%");
        }

        if (!string.IsNullOrEmpty(query.ExtensionName))
        {
            sql += " AND ExtensionName LIKE @ExtensionName";
            parameters.Add("ExtensionName", $"%{query.ExtensionName}%");
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

        return await ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<long> CreateAsync(ExtensionFunction entity)
    {
        const string sql = @"
            INSERT INTO ExtensionFunctions 
            (ExtensionId, ExtensionName, ExtensionType, ExtensionValue, ExtensionConfig, 
             SeqNo, Status, Version, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, 
             CreatedPriority, CreatedGroup)
            VALUES 
            (@ExtensionId, @ExtensionName, @ExtensionType, @ExtensionValue, @ExtensionConfig, 
             @SeqNo, @Status, @Version, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 
             @CreatedPriority, @CreatedGroup);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        return await ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task UpdateAsync(ExtensionFunction entity)
    {
        const string sql = @"
            UPDATE ExtensionFunctions SET
                ExtensionName = @ExtensionName,
                ExtensionType = @ExtensionType,
                ExtensionValue = @ExtensionValue,
                ExtensionConfig = @ExtensionConfig,
                SeqNo = @SeqNo,
                Status = @Status,
                Version = @Version,
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
            DELETE FROM ExtensionFunctions 
            WHERE TKey = @TKey";

        await ExecuteAsync(sql, new { TKey = tKey });
    }

    public async Task<bool> ExistsAsync(string extensionId)
    {
        const string sql = @"
            SELECT COUNT(*) FROM ExtensionFunctions 
            WHERE ExtensionId = @ExtensionId";

        var count = await ExecuteScalarAsync<int>(sql, new { ExtensionId = extensionId });
        return count > 0;
    }

    public async Task BatchCreateAsync(IEnumerable<ExtensionFunction> entities)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sql = @"
                INSERT INTO ExtensionFunctions 
                (ExtensionId, ExtensionName, ExtensionType, ExtensionValue, ExtensionConfig, 
                 SeqNo, Status, Version, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, 
                 CreatedPriority, CreatedGroup)
                VALUES 
                (@ExtensionId, @ExtensionName, @ExtensionType, @ExtensionValue, @ExtensionConfig, 
                 @SeqNo, @Status, @Version, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 
                 @CreatedPriority, @CreatedGroup);";

            await connection.ExecuteAsync(sql, entities, transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}

