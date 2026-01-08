using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 異動記錄 Repository 實作 (SYS0610)
/// </summary>
public class ChangeLogRepository : BaseRepository, IChangeLogRepository
{
    public ChangeLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ChangeLog>> GetUserChangeLogsAsync(
        string programId,
        string? changeUserId,
        string? targetUserId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize)
    {
        try
        {
            var sql = @"
                SELECT 
                    cl.LogId,
                    cl.ProgramId,
                    cl.ChangeUserId,
                    cl.ChangeDate,
                    cl.ChangeStatus,
                    cl.ChangeField,
                    cl.OldValue,
                    cl.NewValue,
                    cl.CreatedBy,
                    cl.CreatedAt
                FROM ChangeLogs cl
                WHERE cl.ProgramId = @ProgramId
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@TargetUserId IS NULL OR cl.OldValue LIKE '%' + @TargetUserId + '%' OR cl.NewValue LIKE '%' + @TargetUserId + '%')
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var parameters = new DynamicParameters();
            parameters.Add("ProgramId", programId);
            parameters.Add("ChangeUserId", changeUserId);
            parameters.Add("TargetUserId", targetUserId);
            parameters.Add("BeginDate", beginDate);
            parameters.Add("EndDate", endDate);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*)
                FROM ChangeLogs cl
                WHERE cl.ProgramId = @ProgramId
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@TargetUserId IS NULL OR cl.OldValue LIKE '%' + @TargetUserId + '%' OR cl.NewValue LIKE '%' + @TargetUserId + '%')
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 排序和分頁
            sql += " ORDER BY cl.ChangeDate DESC, cl.ChangeUserId";
            var offset = (pageIndex - 1) * pageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var items = await QueryAsync<ChangeLog>(sql, parameters);

            return new PagedResult<ChangeLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<ChangeLog?> GetChangeLogByIdAsync(long logId)
    {
        try
        {
            var sql = @"
                SELECT 
                    cl.LogId,
                    cl.ProgramId,
                    cl.ChangeUserId,
                    cl.ChangeDate,
                    cl.ChangeStatus,
                    cl.ChangeField,
                    cl.OldValue,
                    cl.NewValue,
                    cl.CreatedBy,
                    cl.CreatedAt
                FROM ChangeLogs cl
                WHERE cl.LogId = @LogId
            ";

            var parameters = new DynamicParameters();
            parameters.Add("LogId", logId);

            return await QueryFirstOrDefaultAsync<ChangeLog>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢異動記錄失敗: {logId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLog>> GetUserRoleChangeLogsAsync(
        string? changeUserId,
        string? searchUserId,
        string? searchRoleId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize)
    {
        try
        {
            var sql = @"
                SELECT 
                    cl.LogId,
                    cl.ProgramId,
                    cl.ChangeUserId,
                    cl.ChangeDate,
                    cl.ChangeStatus,
                    cl.ChangeField,
                    cl.OldValue,
                    cl.NewValue,
                    cl.CreatedBy,
                    cl.CreatedAt
                FROM ChangeLogs cl
                WHERE cl.ProgramId IN ('SYS0220', 'SYS0230')
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@SearchUserId IS NULL OR cl.OldValue LIKE '%' + @SearchUserId + '%' OR cl.NewValue LIKE '%' + @SearchUserId + '%')
                    AND (@SearchRoleId IS NULL OR cl.OldValue LIKE '%' + @SearchRoleId + '%' OR cl.NewValue LIKE '%' + @SearchRoleId + '%')
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var parameters = new DynamicParameters();
            parameters.Add("ChangeUserId", changeUserId);
            parameters.Add("SearchUserId", searchUserId);
            parameters.Add("SearchRoleId", searchRoleId);
            parameters.Add("BeginDate", beginDate);
            parameters.Add("EndDate", endDate);

            var countSql = @"
                SELECT COUNT(*)
                FROM ChangeLogs cl
                WHERE cl.ProgramId IN ('SYS0220', 'SYS0230')
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@SearchUserId IS NULL OR cl.OldValue LIKE '%' + @SearchUserId + '%' OR cl.NewValue LIKE '%' + @SearchUserId + '%')
                    AND (@SearchRoleId IS NULL OR cl.OldValue LIKE '%' + @SearchRoleId + '%' OR cl.NewValue LIKE '%' + @SearchRoleId + '%')
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            sql += " ORDER BY cl.ChangeDate DESC, cl.ChangeUserId";
            var offset = (pageIndex - 1) * pageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var items = await QueryAsync<ChangeLog>(sql, parameters);

            return new PagedResult<ChangeLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者角色對應設定異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLog>> GetSystemPermissionChangeLogsAsync(
        string? changeUserId,
        string? searchUserId,
        string? searchRoleId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize)
    {
        try
        {
            var sql = @"
                SELECT 
                    cl.LogId,
                    cl.ProgramId,
                    cl.ChangeUserId,
                    cl.ChangeDate,
                    cl.ChangeStatus,
                    cl.ChangeField,
                    cl.OldValue,
                    cl.NewValue,
                    cl.CreatedBy,
                    cl.CreatedAt
                FROM ChangeLogs cl
                WHERE cl.ProgramId IN ('SYS0310', 'SYS0320')
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@SearchUserId IS NULL OR cl.OldValue LIKE '%' + @SearchUserId + '%' OR cl.NewValue LIKE '%' + @SearchUserId + '%')
                    AND (@SearchRoleId IS NULL OR cl.OldValue LIKE '%' + @SearchRoleId + '%' OR cl.NewValue LIKE '%' + @SearchRoleId + '%')
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var parameters = new DynamicParameters();
            parameters.Add("ChangeUserId", changeUserId);
            parameters.Add("SearchUserId", searchUserId);
            parameters.Add("SearchRoleId", searchRoleId);
            parameters.Add("BeginDate", beginDate);
            parameters.Add("EndDate", endDate);

            var countSql = @"
                SELECT COUNT(*)
                FROM ChangeLogs cl
                WHERE cl.ProgramId IN ('SYS0310', 'SYS0320')
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@SearchUserId IS NULL OR cl.OldValue LIKE '%' + @SearchUserId + '%' OR cl.NewValue LIKE '%' + @SearchUserId + '%')
                    AND (@SearchRoleId IS NULL OR cl.OldValue LIKE '%' + @SearchRoleId + '%' OR cl.NewValue LIKE '%' + @SearchRoleId + '%')
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            sql += " ORDER BY cl.ChangeDate DESC, cl.ChangeUserId";
            var offset = (pageIndex - 1) * pageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var items = await QueryAsync<ChangeLog>(sql, parameters);

            return new PagedResult<ChangeLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統權限異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLog>> GetControllableFieldChangeLogsAsync(
        string? changeUserId,
        string? searchUserId,
        string? fieldId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize)
    {
        try
        {
            var sql = @"
                SELECT 
                    cl.LogId,
                    cl.ProgramId,
                    cl.ChangeUserId,
                    cl.ChangeDate,
                    cl.ChangeStatus,
                    cl.ChangeField,
                    cl.OldValue,
                    cl.NewValue,
                    cl.CreatedBy,
                    cl.CreatedAt
                FROM ChangeLogs cl
                WHERE cl.ProgramId = 'SYS0510'
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@SearchUserId IS NULL OR cl.OldValue LIKE '%' + @SearchUserId + '%' OR cl.NewValue LIKE '%' + @SearchUserId + '%')
                    AND (@FieldId IS NULL OR cl.OldValue LIKE '%' + @FieldId + '%' OR cl.NewValue LIKE '%' + @FieldId + '%')
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var parameters = new DynamicParameters();
            parameters.Add("ChangeUserId", changeUserId);
            parameters.Add("SearchUserId", searchUserId);
            parameters.Add("FieldId", fieldId);
            parameters.Add("BeginDate", beginDate);
            parameters.Add("EndDate", endDate);

            var countSql = @"
                SELECT COUNT(*)
                FROM ChangeLogs cl
                WHERE cl.ProgramId = 'SYS0510'
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@SearchUserId IS NULL OR cl.OldValue LIKE '%' + @SearchUserId + '%' OR cl.NewValue LIKE '%' + @SearchUserId + '%')
                    AND (@FieldId IS NULL OR cl.OldValue LIKE '%' + @FieldId + '%' OR cl.NewValue LIKE '%' + @FieldId + '%')
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            sql += " ORDER BY cl.ChangeDate DESC, cl.ChangeUserId";
            var offset = (pageIndex - 1) * pageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var items = await QueryAsync<ChangeLog>(sql, parameters);

            return new PagedResult<ChangeLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢可管控欄位異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLog>> GetOtherChangeLogsAsync(
        string? changeUserId,
        string? programId,
        DateTime beginDate,
        DateTime endDate,
        int pageIndex,
        int pageSize)
    {
        try
        {
            var sql = @"
                SELECT 
                    cl.LogId,
                    cl.ProgramId,
                    cl.ChangeUserId,
                    cl.ChangeDate,
                    cl.ChangeStatus,
                    cl.ChangeField,
                    cl.OldValue,
                    cl.NewValue,
                    cl.CreatedBy,
                    cl.CreatedAt
                FROM ChangeLogs cl
                WHERE cl.ProgramId NOT IN ('SYS0110', 'SYS0210', 'SYS0220', 'SYS0230', 'SYS0310', 'SYS0320', 'SYS0510')
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@ProgramId IS NULL OR cl.ProgramId = @ProgramId)
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var parameters = new DynamicParameters();
            parameters.Add("ChangeUserId", changeUserId);
            parameters.Add("ProgramId", programId);
            parameters.Add("BeginDate", beginDate);
            parameters.Add("EndDate", endDate);

            var countSql = @"
                SELECT COUNT(*)
                FROM ChangeLogs cl
                WHERE cl.ProgramId NOT IN ('SYS0110', 'SYS0210', 'SYS0220', 'SYS0230', 'SYS0310', 'SYS0320', 'SYS0510')
                    AND (@ChangeUserId IS NULL OR cl.ChangeUserId = @ChangeUserId)
                    AND (@ProgramId IS NULL OR cl.ProgramId = @ProgramId)
                    AND cl.ChangeDate >= @BeginDate
                    AND cl.ChangeDate < DATEADD(DAY, 1, @EndDate)
            ";

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            sql += " ORDER BY cl.ChangeDate DESC, cl.ChangeUserId";
            var offset = (pageIndex - 1) * pageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var items = await QueryAsync<ChangeLog>(sql, parameters);

            return new PagedResult<ChangeLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢其他異動記錄失敗", ex);
            throw;
        }
    }
}

