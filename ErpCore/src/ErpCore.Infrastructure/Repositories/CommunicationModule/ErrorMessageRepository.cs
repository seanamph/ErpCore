using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CommunicationModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CommunicationModule;

/// <summary>
/// 錯誤訊息 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ErrorMessageRepository : BaseRepository, IErrorMessageRepository
{
    public ErrorMessageRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ErrorMessage?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ErrorMessages 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ErrorMessage>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢錯誤訊息失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<(IEnumerable<ErrorMessage> Items, int TotalCount)> QueryAsync(ErrorMessageQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ErrorMessages 
                WHERE 1=1";

            var countSql = @"
                SELECT COUNT(*) FROM ErrorMessages 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ErrorCode))
            {
                sql += " AND ErrorCode LIKE @ErrorCode";
                countSql += " AND ErrorCode LIKE @ErrorCode";
                parameters.Add("ErrorCode", $"%{query.ErrorCode}%");
            }

            if (!string.IsNullOrEmpty(query.ErrorType))
            {
                sql += " AND ErrorType = @ErrorType";
                countSql += " AND ErrorType = @ErrorType";
                parameters.Add("ErrorType", query.ErrorType);
            }

            if (query.HttpStatusCode.HasValue)
            {
                sql += " AND HttpStatusCode = @HttpStatusCode";
                countSql += " AND HttpStatusCode = @HttpStatusCode";
                parameters.Add("HttpStatusCode", query.HttpStatusCode.Value);
            }

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND UserId = @UserId";
                countSql += " AND UserId = @UserId";
                parameters.Add("UserId", query.UserId);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND CreatedAt >= @StartDate";
                countSql += " AND CreatedAt >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND CreatedAt <= @EndDate";
                countSql += " AND CreatedAt <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            var sortField = string.IsNullOrEmpty(query.SortField) ? "CreatedAt" : query.SortField;
            var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ErrorMessage>(sql, parameters);
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢錯誤訊息列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(ErrorMessage entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO ErrorMessages 
                (ErrorCode, ErrorType, HttpStatusCode, ErrorMessage, ErrorDetail, RequestUrl, RequestMethod, UserId, UserIp, UserAgent, StackTrace, CreatedAt)
                VALUES 
                (@ErrorCode, @ErrorType, @HttpStatusCode, @ErrorMessageText, @ErrorDetail, @RequestUrl, @RequestMethod, @UserId, @UserIp, @UserAgent, @StackTrace, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.ErrorCode,
                entity.ErrorType,
                entity.HttpStatusCode,
                ErrorMessageText = entity.ErrorMessageText,
                entity.ErrorDetail,
                entity.RequestUrl,
                entity.RequestMethod,
                entity.UserId,
                entity.UserIp,
                entity.UserAgent,
                entity.StackTrace,
                CreatedAt = DateTime.Now
            });

            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增錯誤訊息失敗", ex);
            throw;
        }
    }

    public async Task<ErrorMessageTemplate?> GetTemplateByErrorCodeAsync(string errorCode, string language = "zh-TW")
    {
        try
        {
            const string sql = @"
                SELECT * FROM ErrorMessageTemplates 
                WHERE ErrorCode = @ErrorCode AND Language = @Language AND IsActive = 1";

            return await QueryFirstOrDefaultAsync<ErrorMessageTemplate>(sql, new { ErrorCode = errorCode, Language = language });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢錯誤訊息模板失敗: {errorCode}, {language}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ErrorMessageTemplate>> GetTemplatesAsync(string? errorCode = null, string? language = null)
    {
        try
        {
            var sql = @"
                SELECT * FROM ErrorMessageTemplates 
                WHERE IsActive = 1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(errorCode))
            {
                sql += " AND ErrorCode = @ErrorCode";
                parameters.Add("ErrorCode", errorCode);
            }

            if (!string.IsNullOrEmpty(language))
            {
                sql += " AND Language = @Language";
                parameters.Add("Language", language);
            }

            sql += " ORDER BY ErrorCode, Language";

            return await QueryAsync<ErrorMessageTemplate>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢錯誤訊息模板列表失敗", ex);
            throw;
        }
    }
}

