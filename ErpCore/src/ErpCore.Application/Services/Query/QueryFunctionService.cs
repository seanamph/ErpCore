using Dapper;
using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 查詢功能服務實作 (SYSQ000)
/// </summary>
public class QueryFunctionService : BaseService, IQueryFunctionService
{
    private readonly IQueryFunctionRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public QueryFunctionService(
        IQueryFunctionRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<QueryFunctionDto>> GetQueryFunctionsAsync(QueryFunctionQueryDto query)
    {
        try
        {
            var queryModel = new QueryFunctionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                QueryId = query.QueryId,
                QueryName = query.QueryName,
                QueryType = query.QueryType,
                Status = query.Status
            };

            var result = await _repository.GetPagedAsync(queryModel);
            return new PagedResult<QueryFunctionDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢功能列表查詢失敗", ex);
            throw;
        }
    }

    public async Task<QueryFunctionDto> GetQueryFunctionByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"查詢功能不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢功能查詢失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<QueryFunctionDto> GetQueryFunctionByQueryIdAsync(string queryId)
    {
        try
        {
            var entity = await _repository.GetByQueryIdAsync(queryId);
            if (entity == null)
            {
                throw new InvalidOperationException($"查詢功能不存在: {queryId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢功能查詢失敗: {queryId}", ex);
            throw;
        }
    }

    public async Task<long> CreateQueryFunctionAsync(CreateQueryFunctionDto dto)
    {
        try
        {
            // 檢查查詢功能代碼是否已存在
            if (await _repository.ExistsAsync(dto.QueryId))
            {
                throw new InvalidOperationException($"查詢功能代碼已存在: {dto.QueryId}");
            }

            var entity = new QueryFunction
            {
                QueryId = dto.QueryId,
                QueryName = dto.QueryName,
                QueryType = dto.QueryType,
                QuerySql = dto.QuerySql,
                QueryConfig = dto.QueryConfig,
                SeqNo = dto.SeqNo,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = dto.CreatedPriority,
                CreatedGroup = dto.CreatedGroup
            };

            var result = await _repository.CreateAsync(entity);
            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增查詢功能失敗", ex);
            throw;
        }
    }

    public async Task UpdateQueryFunctionAsync(long tKey, UpdateQueryFunctionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"查詢功能不存在: {tKey}");
            }

            entity.QueryName = dto.QueryName;
            entity.QueryType = dto.QueryType;
            entity.QuerySql = dto.QuerySql;
            entity.QueryConfig = dto.QueryConfig;
            entity.SeqNo = dto.SeqNo;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改查詢功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteQueryFunctionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"查詢功能不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除查詢功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<QueryResultDto> ExecuteQueryFunctionAsync(long tKey, ExecuteQueryDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"查詢功能不存在: {tKey}");
            }

            if (entity.Status != "1")
            {
                throw new InvalidOperationException($"查詢功能已停用: {tKey}");
            }

            if (string.IsNullOrWhiteSpace(entity.QuerySql))
            {
                throw new InvalidOperationException($"查詢功能SQL語句為空: {tKey}");
            }

            // 執行查詢（注意：這裡需要進行SQL注入防護）
            // 注意：實際執行動態SQL需要更嚴格的安全檢查，這裡僅為示例
            var sql = entity.QuerySql;
            var parameters = dto.Parameters ?? new Dictionary<string, object>();

            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.QueryAsync(sql, parameters);
            
            var result = new QueryResultDto();
            
            // 轉換查詢結果
            if (rows.Any())
            {
                var firstRow = rows.First() as IDictionary<string, object>;
                if (firstRow != null)
                {
                    result.Columns = firstRow.Keys.ToList();
                    
                    foreach (var row in rows)
                    {
                        var rowDict = row as IDictionary<string, object>;
                        if (rowDict != null)
                        {
                            var rowValues = result.Columns.Select(col => rowDict.ContainsKey(col) ? rowDict[col] ?? DBNull.Value : DBNull.Value).ToList();
                            result.Rows.Add(rowValues);
                        }
                    }
                }
            }

            result.TotalCount = result.Rows.Count;

            _logger.LogInfo($"執行查詢功能成功: {tKey}, 返回 {result.TotalCount} 筆資料");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行查詢功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(long tKey, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"查詢功能不存在: {tKey}");
            }

            entity.Status = status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新查詢功能狀態失敗: {tKey}", ex);
            throw;
        }
    }

    private QueryFunctionDto MapToDto(QueryFunction entity)
    {
        return new QueryFunctionDto
        {
            TKey = entity.TKey,
            QueryId = entity.QueryId,
            QueryName = entity.QueryName,
            QueryType = entity.QueryType,
            QuerySql = entity.QuerySql,
            QueryConfig = entity.QueryConfig,
            SeqNo = entity.SeqNo,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt,
            CreatedPriority = entity.CreatedPriority,
            CreatedGroup = entity.CreatedGroup
        };
    }
}

