using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 查詢功能 Repository 實作 (SYSQ000)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class QueryFunctionRepository : BaseRepository, IQueryFunctionRepository
{
    public QueryFunctionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<QueryFunction?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM QueryFunctions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<QueryFunction>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢功能查詢失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<QueryFunction?> GetByQueryIdAsync(string queryId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM QueryFunctions 
                WHERE QueryId = @QueryId";

            return await QueryFirstOrDefaultAsync<QueryFunction>(sql, new { QueryId = queryId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢功能查詢失敗: {queryId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<QueryFunction>> GetPagedAsync(QueryFunctionQuery query)
    {
        try
        {
            var whereConditions = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(query.QueryId))
            {
                whereConditions.Add("QueryId LIKE @QueryId");
                parameters.Add("QueryId", $"%{query.QueryId}%");
            }

            if (!string.IsNullOrWhiteSpace(query.QueryName))
            {
                whereConditions.Add("QueryName LIKE @QueryName");
                parameters.Add("QueryName", $"%{query.QueryName}%");
            }

            if (!string.IsNullOrWhiteSpace(query.QueryType))
            {
                whereConditions.Add("QueryType = @QueryType");
                parameters.Add("QueryType", query.QueryType);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                whereConditions.Add("Status = @Status");
                parameters.Add("Status", query.Status);
            }

            var whereClause = whereConditions.Any() ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";

            var sortField = string.IsNullOrWhiteSpace(query.SortField) ? "SeqNo" : query.SortField;
            var sortOrder = string.IsNullOrWhiteSpace(query.SortOrder) ? "ASC" : query.SortOrder;
            var orderBy = $"ORDER BY {sortField} {sortOrder}";

            var offset = (query.PageIndex - 1) * query.PageSize;

            var countSql = $@"
                SELECT COUNT(*) FROM QueryFunctions 
                {whereClause}";

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            var dataSql = $@"
                SELECT * FROM QueryFunctions 
                {whereClause}
                {orderBy}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<QueryFunction>(dataSql, parameters);

            return new PagedResult<QueryFunction>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢功能列表查詢失敗", ex);
            throw;
        }
    }

    public async Task<QueryFunction> CreateAsync(QueryFunction entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO QueryFunctions (QueryId, QueryName, QueryType, QuerySql, QueryConfig, SeqNo, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup)
                VALUES (@QueryId, @QueryName, @QueryType, @QuerySql, @QueryConfig, @SeqNo, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.QueryId,
                entity.QueryName,
                entity.QueryType,
                entity.QuerySql,
                entity.QueryConfig,
                entity.SeqNo,
                entity.Status,
                entity.Notes,
                entity.CreatedBy,
                entity.CreatedAt,
                entity.UpdatedBy,
                entity.UpdatedAt,
                entity.CreatedPriority,
                entity.CreatedGroup
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增查詢功能失敗", ex);
            throw;
        }
    }

    public async Task<QueryFunction> UpdateAsync(QueryFunction entity)
    {
        try
        {
            const string sql = @"
                UPDATE QueryFunctions 
                SET QueryName = @QueryName,
                    QueryType = @QueryType,
                    QuerySql = @QuerySql,
                    QueryConfig = @QueryConfig,
                    SeqNo = @SeqNo,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.QueryName,
                entity.QueryType,
                entity.QuerySql,
                entity.QueryConfig,
                entity.SeqNo,
                entity.Status,
                entity.Notes,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改查詢功能失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM QueryFunctions WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除查詢功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string queryId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM QueryFunctions 
                WHERE QueryId = @QueryId";

            var count = await QuerySingleAsync<int>(sql, new { QueryId = queryId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查查詢功能是否存在失敗: {queryId}", ex);
            throw;
        }
    }
}

