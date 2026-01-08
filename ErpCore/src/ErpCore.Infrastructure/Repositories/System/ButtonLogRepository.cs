using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 按鈕操作記錄 Repository 實作 (SYS0790)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ButtonLogRepository : BaseRepository, IButtonLogRepository
{
    public ButtonLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ButtonLog>> QueryAsync(ButtonLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    bl.TKey,
                    bl.BUser,
                    bl.BTime,
                    bl.ProgId,
                    bl.ProgName,
                    bl.ButtonName,
                    bl.Url,
                    bl.FrameName
                FROM ButtonLog bl
                LEFT JOIN Users u ON bl.BUser = u.UserId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            // 使用者篩選
            if (query.UserIds != null && query.UserIds.Any())
            {
                sql += " AND bl.BUser IN @UserIds";
                parameters.Add("UserIds", query.UserIds);
            }

            // 作業代碼篩選
            if (!string.IsNullOrEmpty(query.ProgId))
            {
                sql += " AND bl.ProgId = @ProgId";
                parameters.Add("ProgId", query.ProgId);
            }

            // 日期時間範圍篩選
            if (query.StartDateTime.HasValue)
            {
                sql += " AND bl.BTime >= @StartDateTime";
                parameters.Add("StartDateTime", query.StartDateTime.Value);
            }

            if (query.EndDateTime.HasValue)
            {
                sql += " AND bl.BTime <= @EndDateTime";
                parameters.Add("EndDateTime", query.EndDateTime.Value);
            }

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*)
                FROM ButtonLog bl
                WHERE 1=1";

            var countParameters = new DynamicParameters();

            if (query.UserIds != null && query.UserIds.Any())
            {
                countSql += " AND bl.BUser IN @UserIds";
                countParameters.Add("UserIds", query.UserIds);
            }

            if (!string.IsNullOrEmpty(query.ProgId))
            {
                countSql += " AND bl.ProgId = @ProgId";
                countParameters.Add("ProgId", query.ProgId);
            }

            if (query.StartDateTime.HasValue)
            {
                countSql += " AND bl.BTime >= @StartDateTime";
                countParameters.Add("StartDateTime", query.StartDateTime.Value);
            }

            if (query.EndDateTime.HasValue)
            {
                countSql += " AND bl.BTime <= @EndDateTime";
                countParameters.Add("EndDateTime", query.EndDateTime.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "BTime" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY bl.{sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ButtonLog>(sql, parameters);

            return new PagedResult<ButtonLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢按鈕操作記錄失敗", ex);
            throw;
        }
    }

    public async Task<ButtonLog> CreateAsync(ButtonLog buttonLog)
    {
        try
        {
            const string sql = @"
                INSERT INTO ButtonLog (BUser, BTime, ProgId, ProgName, ButtonName, Url, FrameName)
                OUTPUT INSERTED.TKey, INSERTED.BUser, INSERTED.BTime, INSERTED.ProgId, INSERTED.ProgName, INSERTED.ButtonName, INSERTED.Url, INSERTED.FrameName
                VALUES (@BUser, @BTime, @ProgId, @ProgName, @ButtonName, @Url, @FrameName)";

            var parameters = new DynamicParameters();
            parameters.Add("BUser", buttonLog.BUser);
            parameters.Add("BTime", buttonLog.BTime);
            parameters.Add("ProgId", buttonLog.ProgId);
            parameters.Add("ProgName", buttonLog.ProgName);
            parameters.Add("ButtonName", buttonLog.ButtonName);
            parameters.Add("Url", buttonLog.Url);
            parameters.Add("FrameName", buttonLog.FrameName);

            var result = await QueryFirstOrDefaultAsync<ButtonLog>(sql, parameters);
            if (result == null)
            {
                throw new InvalidOperationException("新增按鈕操作記錄失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增按鈕操作記錄失敗", ex);
            throw;
        }
    }
}

