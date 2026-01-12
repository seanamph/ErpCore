using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者排程 Repository 實作 (SYS0116)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class UserScheduleRepository : BaseRepository, IUserScheduleRepository
{
    public UserScheduleRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<UserSchedule?> GetByIdAsync(Guid scheduleId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserSchedules 
                WHERE ScheduleId = @ScheduleId";

            return await QueryFirstOrDefaultAsync<UserSchedule>(sql, new { ScheduleId = scheduleId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢排程失敗: {scheduleId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserSchedule>> QueryAsync(UserScheduleQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM UserSchedules
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND UserId LIKE @UserId";
                parameters.Add("UserId", $"%{query.UserId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.ScheduleType))
            {
                sql += " AND ScheduleType = @ScheduleType";
                parameters.Add("ScheduleType", query.ScheduleType);
            }

            if (query.ScheduleDateFrom.HasValue)
            {
                sql += " AND ScheduleDate >= @ScheduleDateFrom";
                parameters.Add("ScheduleDateFrom", query.ScheduleDateFrom.Value);
            }

            if (query.ScheduleDateTo.HasValue)
            {
                sql += " AND ScheduleDate <= @ScheduleDateTo";
                parameters.Add("ScheduleDateTo", query.ScheduleDateTo.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ScheduleDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<UserSchedule>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM UserSchedules
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.UserId))
            {
                countSql += " AND UserId LIKE @UserId";
                countParameters.Add("UserId", $"%{query.UserId}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.ScheduleType))
            {
                countSql += " AND ScheduleType = @ScheduleType";
                countParameters.Add("ScheduleType", query.ScheduleType);
            }
            if (query.ScheduleDateFrom.HasValue)
            {
                countSql += " AND ScheduleDate >= @ScheduleDateFrom";
                countParameters.Add("ScheduleDateFrom", query.ScheduleDateFrom.Value);
            }
            if (query.ScheduleDateTo.HasValue)
            {
                countSql += " AND ScheduleDate <= @ScheduleDateTo";
                countParameters.Add("ScheduleDateTo", query.ScheduleDateTo.Value);
            }

            var totalCount = await QueryFirstOrDefaultAsync<int>(countSql, countParameters);

            return new PagedResult<UserSchedule>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢排程列表失敗", ex);
            throw;
        }
    }

    public async Task<UserSchedule> CreateAsync(UserSchedule schedule)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserSchedules (
                    ScheduleId, UserId, ScheduleDate, ScheduleType, Status,
                    ScheduleData, ExecuteResult, ErrorMessage, ExecutedAt,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @ScheduleId, @UserId, @ScheduleDate, @ScheduleType, @Status,
                    @ScheduleData, @ExecuteResult, @ErrorMessage, @ExecutedAt,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            await ExecuteAsync(sql, schedule);
            return schedule;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增排程失敗: {schedule.ScheduleId}", ex);
            throw;
        }
    }

    public async Task<UserSchedule> UpdateAsync(UserSchedule schedule)
    {
        try
        {
            const string sql = @"
                UPDATE UserSchedules SET
                    UserId = @UserId,
                    ScheduleDate = @ScheduleDate,
                    ScheduleType = @ScheduleType,
                    Status = @Status,
                    ScheduleData = @ScheduleData,
                    ExecuteResult = @ExecuteResult,
                    ErrorMessage = @ErrorMessage,
                    ExecutedAt = @ExecutedAt,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ScheduleId = @ScheduleId";

            await ExecuteAsync(sql, schedule);
            return schedule;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改排程失敗: {schedule.ScheduleId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(Guid scheduleId)
    {
        try
        {
            const string sql = @"
                DELETE FROM UserSchedules
                WHERE ScheduleId = @ScheduleId";

            await ExecuteAsync(sql, new { ScheduleId = scheduleId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除排程失敗: {scheduleId}", ex);
            throw;
        }
    }

    public async Task<List<UserSchedule>> GetPendingSchedulesAsync(DateTime executeTime)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserSchedules
                WHERE Status = 'PENDING'
                  AND ScheduleDate <= @ExecuteTime
                ORDER BY ScheduleDate ASC";

            var items = await QueryAsync<UserSchedule>(sql, new { ExecuteTime = executeTime });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢待執行排程失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(Guid scheduleId, string status, string? errorMessage = null, string? executeResult = null)
    {
        try
        {
            var sql = @"
                UPDATE UserSchedules SET
                    Status = @Status,
                    ExecutedAt = @ExecutedAt,
                    ErrorMessage = @ErrorMessage,
                    ExecuteResult = @ExecuteResult,
                    UpdatedAt = @UpdatedAt
                WHERE ScheduleId = @ScheduleId";

            await ExecuteAsync(sql, new
            {
                ScheduleId = scheduleId,
                Status = status,
                ExecutedAt = status == "COMPLETED" || status == "FAILED" ? DateTime.Now : (DateTime?)null,
                ErrorMessage = errorMessage,
                ExecuteResult = executeResult,
                UpdatedAt = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新排程狀態失敗: {scheduleId}", ex);
            throw;
        }
    }
}
