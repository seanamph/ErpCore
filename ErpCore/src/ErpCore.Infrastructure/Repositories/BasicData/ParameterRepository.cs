using System.Data;
using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 參數 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ParameterRepository : BaseRepository, IParameterRepository
{
    public ParameterRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Parameter?> GetByKeyAsync(string title, string tag)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Parameters 
                WHERE Title = @Title AND Tag = @Tag";

            return await QueryFirstOrDefaultAsync<Parameter>(sql, new { Title = title, Tag = tag });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢參數失敗: {title}/{tag}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Parameter>> QueryAsync(ParameterQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Parameters
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.Title))
            {
                sql += " AND Title LIKE @Title";
                parameters.Add("Title", $"%{query.Title}%");
            }

            if (!string.IsNullOrEmpty(query.Tag))
            {
                sql += " AND Tag LIKE @Tag";
                parameters.Add("Tag", $"%{query.Tag}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "Title, Tag" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Parameter>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Parameters
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.Title))
            {
                countSql += " AND Title LIKE @Title";
                countParameters.Add("Title", $"%{query.Title}%");
            }
            if (!string.IsNullOrEmpty(query.Tag))
            {
                countSql += " AND Tag LIKE @Tag";
                countParameters.Add("Tag", $"%{query.Tag}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.SystemId))
            {
                countSql += " AND SystemId = @SystemId";
                countParameters.Add("SystemId", query.SystemId);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Parameter>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢參數列表失敗", ex);
            throw;
        }
    }

    public async Task<List<Parameter>> GetByTitleAsync(string title)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Parameters 
                WHERE Title = @Title
                ORDER BY SeqNo, Tag";

            var items = await QueryAsync<Parameter>(sql, new { Title = title });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據標題查詢參數列表失敗: {title}", ex);
            throw;
        }
    }

    public async Task<Parameter> CreateAsync(Parameter parameter)
    {
        try
        {
            const string sql = @"
                INSERT INTO Parameters (
                    Title, Tag, SeqNo, Content, Content2, Notes, Status, ReadOnly, SystemId,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @Title, @Tag, @SeqNo, @Content, @Content2, @Notes, @Status, @ReadOnly, @SystemId,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Parameter>(sql, parameter);
            if (result == null)
            {
                throw new InvalidOperationException("新增參數失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增參數失敗: {parameter.Title}/{parameter.Tag}", ex);
            throw;
        }
    }

    public async Task<Parameter> UpdateAsync(Parameter parameter)
    {
        try
        {
            const string sql = @"
                UPDATE Parameters SET
                    SeqNo = @SeqNo,
                    Content = @Content,
                    Content2 = @Content2,
                    Notes = @Notes,
                    Status = @Status,
                    ReadOnly = @ReadOnly,
                    SystemId = @SystemId,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE Title = @Title AND Tag = @Tag";

            var result = await QueryFirstOrDefaultAsync<Parameter>(sql, parameter);
            if (result == null)
            {
                throw new InvalidOperationException($"參數不存在: {parameter.Title}/{parameter.Tag}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改參數失敗: {parameter.Title}/{parameter.Tag}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string title, string tag)
    {
        try
        {
            const string sql = @"
                DELETE FROM Parameters 
                WHERE Title = @Title AND Tag = @Tag";

            await ExecuteAsync(sql, new { Title = title, Tag = tag });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除參數失敗: {title}/{tag}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string title, string tag)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Parameters 
                WHERE Title = @Title AND Tag = @Tag";

            var count = await QuerySingleAsync<int>(sql, new { Title = title, Tag = tag });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查參數是否存在失敗: {title}/{tag}", ex);
            throw;
        }
    }
}

