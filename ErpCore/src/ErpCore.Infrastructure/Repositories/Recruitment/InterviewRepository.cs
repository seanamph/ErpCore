using Dapper;
using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Recruitment;

/// <summary>
/// 訪談 Repository 實作 (SYSC222)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class InterviewRepository : BaseRepository, IInterviewRepository
{
    public InterviewRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Interview?> GetByIdAsync(long interviewId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Interviews 
                WHERE InterviewId = @InterviewId";

            return await QueryFirstOrDefaultAsync<Interview>(sql, new { InterviewId = interviewId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢訪談失敗: {interviewId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Interview>> QueryAsync(InterviewQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Interviews
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProspectId))
            {
                sql += " AND ProspectId = @ProspectId";
                parameters.Add("ProspectId", query.ProspectId);
            }

            if (query.InterviewDateFrom.HasValue)
            {
                sql += " AND InterviewDate >= @InterviewDateFrom";
                parameters.Add("InterviewDateFrom", query.InterviewDateFrom.Value);
            }

            if (query.InterviewDateTo.HasValue)
            {
                sql += " AND InterviewDate <= @InterviewDateTo";
                parameters.Add("InterviewDateTo", query.InterviewDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.InterviewResult))
            {
                sql += " AND InterviewResult = @InterviewResult";
                parameters.Add("InterviewResult", query.InterviewResult);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Interviewer))
            {
                sql += " AND Interviewer = @Interviewer";
                parameters.Add("Interviewer", query.Interviewer);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "InterviewDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Interview>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Interviews
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ProspectId))
            {
                countSql += " AND ProspectId = @ProspectId";
                countParameters.Add("ProspectId", query.ProspectId);
            }
            if (query.InterviewDateFrom.HasValue)
            {
                countSql += " AND InterviewDate >= @InterviewDateFrom";
                countParameters.Add("InterviewDateFrom", query.InterviewDateFrom.Value);
            }
            if (query.InterviewDateTo.HasValue)
            {
                countSql += " AND InterviewDate <= @InterviewDateTo";
                countParameters.Add("InterviewDateTo", query.InterviewDateTo.Value);
            }
            if (!string.IsNullOrEmpty(query.InterviewResult))
            {
                countSql += " AND InterviewResult = @InterviewResult";
                countParameters.Add("InterviewResult", query.InterviewResult);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.Interviewer))
            {
                countSql += " AND Interviewer = @Interviewer";
                countParameters.Add("Interviewer", query.Interviewer);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Interview>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢訪談列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<Interview>> QueryByProspectIdAsync(string prospectId, InterviewQuery query)
    {
        try
        {
            var queryWithProspect = new InterviewQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ProspectId = prospectId,
                InterviewDateFrom = query.InterviewDateFrom,
                InterviewDateTo = query.InterviewDateTo,
                InterviewResult = query.InterviewResult,
                Status = query.Status,
                Interviewer = query.Interviewer
            };

            return await QueryAsync(queryWithProspect);
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據潛客查詢訪談列表失敗: {prospectId}", ex);
            throw;
        }
    }

    public async Task<Interview> CreateAsync(Interview interview)
    {
        try
        {
            const string sql = @"
                INSERT INTO Interviews (
                    ProspectId, InterviewDate, InterviewTime, InterviewType, Interviewer, InterviewLocation,
                    InterviewContent, InterviewResult, NextAction, NextActionDate, FollowUpDate, Notes, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ProspectId, @InterviewDate, @InterviewTime, @InterviewType, @Interviewer, @InterviewLocation,
                    @InterviewContent, @InterviewResult, @NextAction, @NextActionDate, @FollowUpDate, @Notes, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Interview>(sql, interview);
            if (result == null)
            {
                throw new InvalidOperationException("新增訪談失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增訪談失敗: {interview.ProspectId}", ex);
            throw;
        }
    }

    public async Task<Interview> UpdateAsync(Interview interview)
    {
        try
        {
            const string sql = @"
                UPDATE Interviews SET
                    ProspectId = @ProspectId,
                    InterviewDate = @InterviewDate,
                    InterviewTime = @InterviewTime,
                    InterviewType = @InterviewType,
                    Interviewer = @Interviewer,
                    InterviewLocation = @InterviewLocation,
                    InterviewContent = @InterviewContent,
                    InterviewResult = @InterviewResult,
                    NextAction = @NextAction,
                    NextActionDate = @NextActionDate,
                    FollowUpDate = @FollowUpDate,
                    Notes = @Notes,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE InterviewId = @InterviewId";

            var result = await QueryFirstOrDefaultAsync<Interview>(sql, interview);
            if (result == null)
            {
                throw new InvalidOperationException($"訪談不存在: {interview.InterviewId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改訪談失敗: {interview.InterviewId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long interviewId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Interviews
                WHERE InterviewId = @InterviewId";

            var rowsAffected = await ExecuteAsync(sql, new { InterviewId = interviewId });
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"訪談不存在: {interviewId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除訪談失敗: {interviewId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long interviewId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Interviews
                WHERE InterviewId = @InterviewId";

            var count = await QuerySingleAsync<int>(sql, new { InterviewId = interviewId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查訪談是否存在失敗: {interviewId}", ex);
            throw;
        }
    }
}

