using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表列印記錄 Repository 實作 (SYSL161)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class BusinessReportPrintLogRepository : BaseRepository, IBusinessReportPrintLogRepository
{
    public BusinessReportPrintLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<BusinessReportPrintLog>> QueryAsync(BusinessReportPrintLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    brpl.TKey,
                    brpl.ReportId,
                    brpl.ReportName,
                    brpl.ReportType,
                    brpl.PrintDate,
                    brpl.PrintUserId,
                    brpl.PrintUserName,
                    brpl.PrintParams,
                    brpl.PrintFormat,
                    brpl.Status,
                    brpl.ErrorMessage,
                    brpl.FilePath,
                    brpl.CreatedBy,
                    brpl.CreatedAt,
                    brpl.UpdatedBy,
                    brpl.UpdatedAt
                FROM BusinessReportPrintLog brpl
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportId))
            {
                sql += " AND brpl.ReportId = @ReportId";
                parameters.Add("ReportId", query.ReportId);
            }

            if (!string.IsNullOrEmpty(query.ReportName))
            {
                sql += " AND brpl.ReportName LIKE @ReportName";
                parameters.Add("ReportName", $"%{query.ReportName}%");
            }

            if (!string.IsNullOrEmpty(query.PrintUserId))
            {
                sql += " AND brpl.PrintUserId = @PrintUserId";
                parameters.Add("PrintUserId", query.PrintUserId);
            }

            if (query.PrintDateFrom.HasValue)
            {
                sql += " AND brpl.PrintDate >= @PrintDateFrom";
                parameters.Add("PrintDateFrom", query.PrintDateFrom.Value);
            }

            if (query.PrintDateTo.HasValue)
            {
                sql += " AND brpl.PrintDate <= @PrintDateTo";
                parameters.Add("PrintDateTo", query.PrintDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND brpl.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 計算總筆數
            var countSql = @"
                SELECT COUNT(*) 
                FROM BusinessReportPrintLog brpl
                WHERE 1=1";
            if (!string.IsNullOrEmpty(query.ReportId))
            {
                countSql += " AND brpl.ReportId = @ReportId";
            }
            if (!string.IsNullOrEmpty(query.ReportName))
            {
                countSql += " AND brpl.ReportName LIKE @ReportName";
            }
            if (!string.IsNullOrEmpty(query.PrintUserId))
            {
                countSql += " AND brpl.PrintUserId = @PrintUserId";
            }
            if (query.PrintDateFrom.HasValue)
            {
                countSql += " AND brpl.PrintDate >= @PrintDateFrom";
            }
            if (query.PrintDateTo.HasValue)
            {
                countSql += " AND brpl.PrintDate <= @PrintDateTo";
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND brpl.Status = @Status";
            }
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "PrintDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY brpl.{sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<BusinessReportPrintLog>(sql, parameters);

            return new PagedResult<BusinessReportPrintLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表列印記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintLog?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT 
                    TKey,
                    ReportId,
                    ReportName,
                    ReportType,
                    PrintDate,
                    PrintUserId,
                    PrintUserName,
                    PrintParams,
                    PrintFormat,
                    Status,
                    ErrorMessage,
                    FilePath,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM BusinessReportPrintLog
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<BusinessReportPrintLog>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<BusinessReportPrintLog>> GetByReportIdAsync(string reportId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    TKey,
                    ReportId,
                    ReportName,
                    ReportType,
                    PrintDate,
                    PrintUserId,
                    PrintUserName,
                    PrintParams,
                    PrintFormat,
                    Status,
                    ErrorMessage,
                    FilePath,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM BusinessReportPrintLog
                WHERE ReportId = @ReportId
                ORDER BY PrintDate DESC";

            var items = await QueryAsync<BusinessReportPrintLog>(sql, new { ReportId = reportId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印記錄失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(BusinessReportPrintLog entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO BusinessReportPrintLog 
                (ReportId, ReportName, ReportType, PrintDate, PrintUserId, PrintUserName, PrintParams, PrintFormat, Status, ErrorMessage, FilePath, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ReportId, @ReportName, @ReportType, @PrintDate, @PrintUserId, @PrintUserName, @PrintParams, @PrintFormat, @Status, @ErrorMessage, @FilePath, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增業務報表列印記錄失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(BusinessReportPrintLog entity)
    {
        try
        {
            const string sql = @"
                UPDATE BusinessReportPrintLog SET
                    ReportName = @ReportName,
                    ReportType = @ReportType,
                    PrintDate = @PrintDate,
                    PrintUserId = @PrintUserId,
                    PrintUserName = @PrintUserName,
                    PrintParams = @PrintParams,
                    PrintFormat = @PrintFormat,
                    Status = @Status,
                    ErrorMessage = @ErrorMessage,
                    FilePath = @FilePath,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var affectedRows = await ExecuteAsync(sql, entity);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表列印記錄失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = "DELETE FROM BusinessReportPrintLog WHERE TKey = @TKey";
            var affectedRows = await ExecuteAsync(sql, new { TKey = tKey });
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }
}

