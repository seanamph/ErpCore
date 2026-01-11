using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者異常登入記錄 Repository 實作 (SYS0760)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LoginLogRepository : BaseRepository, ILoginLogRepository
{
    public LoginLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<LoginLog>> QueryAsync(LoginLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    L.T_KEY AS TKey,
                    L.EVENT_ID AS EventId,
                    L.USER_ID AS UserId,
                    L.LOGIN_IP AS LoginIp,
                    L.EVENT_TIME AS EventTime,
                    L.BUSER AS BUser,
                    L.BTIME AS BTime,
                    L.CUSER AS CUser,
                    L.CTIME AS CTime,
                    L.CPRIORITY AS CPriority,
                    L.CGROUP AS CGroup
                FROM LoginLog L
                LEFT JOIN Users U ON L.USER_ID = U.UserId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            // 異常事件代碼條件
            if (query.EventIds != null && query.EventIds.Any())
            {
                sql += " AND L.EVENT_ID IN @EventIds";
                parameters.Add("EventIds", query.EventIds);
            }

            // 使用者代碼條件 (模糊查詢)
            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                sql += " AND L.USER_ID LIKE @UserId";
                parameters.Add("UserId", $"%{query.UserId}%");
            }

            // 事件時間範圍
            if (query.EventTimeFrom.HasValue)
            {
                sql += " AND L.EVENT_TIME >= @EventTimeFrom";
                parameters.Add("EventTimeFrom", query.EventTimeFrom.Value);
            }

            if (query.EventTimeTo.HasValue)
            {
                sql += " AND L.EVENT_TIME <= @EventTimeTo";
                parameters.Add("EventTimeTo", query.EventTimeTo.Value);
            }

            // 只查詢異常事件 (EVENT_ID='1')
            sql += " AND L.EVENT_ID = '1'";

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*)
                FROM LoginLog L
                WHERE 1=1";

            var countParameters = new DynamicParameters();

            if (query.EventIds != null && query.EventIds.Any())
            {
                countSql += " AND L.EVENT_ID IN @EventIds";
                countParameters.Add("EventIds", query.EventIds);
            }

            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                countSql += " AND L.USER_ID LIKE @UserId";
                countParameters.Add("UserId", $"%{query.UserId}%");
            }

            if (query.EventTimeFrom.HasValue)
            {
                countSql += " AND L.EVENT_TIME >= @EventTimeFrom";
                countParameters.Add("EventTimeFrom", query.EventTimeFrom.Value);
            }

            if (query.EventTimeTo.HasValue)
            {
                countSql += " AND L.EVENT_TIME <= @EventTimeTo";
                countParameters.Add("EventTimeTo", query.EventTimeTo.Value);
            }

            countSql += " AND L.EVENT_ID = '1'";

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            // 排序
            var orderBy = BuildOrderBy(query);
            sql += $" ORDER BY {orderBy}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<LoginLog>(sql, parameters);

            return new PagedResult<LoginLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢異常登入記錄失敗", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(List<long> tKeys)
    {
        try
        {
            if (tKeys == null || !tKeys.Any())
                return 0;

            const string sql = "DELETE FROM LoginLog WHERE T_KEY IN @TKeys";
            var parameters = new { TKeys = tKeys };

            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除異常登入記錄失敗", ex);
            throw;
        }
    }

    public async Task<List<EventTypeOption>> GetEventTypesAsync()
    {
        try
        {
            const string sql = @"
                SELECT TAG AS Tag, CONTENT AS Content
                FROM Parameters
                WHERE TITLE = 'MNG_ERR_TYPE'
                AND STATUS = '1'
                ORDER BY TAG";

            var result = await QueryAsync<EventTypeOption>(sql);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢異常事件代碼選項失敗", ex);
            throw;
        }
    }

    private string BuildOrderBy(LoginLogQuery query)
    {
        var orders = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.SortBy1))
        {
            orders.Add($"L.{query.SortBy1} {query.SortOrder1 ?? "ASC"}");
        }

        if (!string.IsNullOrWhiteSpace(query.SortBy2))
        {
            orders.Add($"L.{query.SortBy2} {query.SortOrder2 ?? "ASC"}");
        }

        if (!string.IsNullOrWhiteSpace(query.SortBy3))
        {
            orders.Add($"L.{query.SortBy3} {query.SortOrder3 ?? "ASC"}");
        }

        // 預設排序
        if (!orders.Any())
        {
            orders.Add("L.EVENT_TIME DESC");
            orders.Add("L.USER_ID ASC");
        }

        return string.Join(", ", orders);
    }
}
