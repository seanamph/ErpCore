using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 文本文件處理記錄 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TextFileProcessLogRepository : BaseRepository, ITextFileProcessLogRepository
{
    public TextFileProcessLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TextFileProcessLog?> GetByIdAsync(Guid logId)
    {
        try
        {
            var sql = @"
                SELECT 
                    LogId, FileName, FileType, ShopId, TotalRecords, SuccessRecords, FailedRecords,
                    ProcessStatus, ProcessStartTime, ProcessEndTime, ErrorMessage,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM TextFileProcessLog
                WHERE LogId = @LogId";

            return await QueryFirstOrDefaultAsync<TextFileProcessLog>(sql, new { LogId = logId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢處理記錄失敗: LogId={logId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TextFileProcessLog>> GetPagedAsync(TextFileProcessLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    LogId, FileName, FileType, ShopId, TotalRecords, SuccessRecords, FailedRecords,
                    ProcessStatus, ProcessStartTime, ProcessEndTime, ErrorMessage,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM TextFileProcessLog
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FileName))
            {
                sql += " AND FileName LIKE @FileName";
                parameters.Add("FileName", $"%{query.FileName}%");
            }

            if (!string.IsNullOrEmpty(query.FileType))
            {
                sql += " AND FileType = @FileType";
                parameters.Add("FileType", query.FileType);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                sql += " AND ProcessStatus = @ProcessStatus";
                parameters.Add("ProcessStatus", query.ProcessStatus);
            }

            // 排序
            var sortField = query.SortField ?? "CreatedAt";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<TextFileProcessLog>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM TextFileProcessLog
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.FileName))
            {
                countSql += " AND FileName LIKE @FileName";
                countParameters.Add("FileName", $"%{query.FileName}%");
            }
            if (!string.IsNullOrEmpty(query.FileType))
            {
                countSql += " AND FileType = @FileType";
                countParameters.Add("FileType", query.FileType);
            }
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                countSql += " AND ShopId = @ShopId";
                countParameters.Add("ShopId", query.ShopId);
            }
            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                countSql += " AND ProcessStatus = @ProcessStatus";
                countParameters.Add("ProcessStatus", query.ProcessStatus);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<TextFileProcessLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢處理記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<TextFileProcessLog> CreateAsync(TextFileProcessLog log)
    {
        try
        {
            var sql = @"
                INSERT INTO TextFileProcessLog 
                    (LogId, FileName, FileType, ShopId, TotalRecords, SuccessRecords, FailedRecords,
                     ProcessStatus, ProcessStartTime, ProcessEndTime, ErrorMessage,
                     CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                    (@LogId, @FileName, @FileType, @ShopId, @TotalRecords, @SuccessRecords, @FailedRecords,
                     @ProcessStatus, @ProcessStartTime, @ProcessEndTime, @ErrorMessage,
                     @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await ExecuteAsync(sql, log);
            return log;
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立處理記錄失敗: FileName={log.FileName}", ex);
            throw;
        }
    }

    public async Task<TextFileProcessLog> UpdateAsync(TextFileProcessLog log)
    {
        try
        {
            log.UpdatedAt = DateTime.Now;
            var sql = @"
                UPDATE TextFileProcessLog SET
                    FileName = @FileName,
                    FileType = @FileType,
                    ShopId = @ShopId,
                    TotalRecords = @TotalRecords,
                    SuccessRecords = @SuccessRecords,
                    FailedRecords = @FailedRecords,
                    ProcessStatus = @ProcessStatus,
                    ProcessStartTime = @ProcessStartTime,
                    ProcessEndTime = @ProcessEndTime,
                    ErrorMessage = @ErrorMessage,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE LogId = @LogId";

            await ExecuteAsync(sql, log);
            return log;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新處理記錄失敗: LogId={log.LogId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(Guid logId)
    {
        try
        {
            var sql = "DELETE FROM TextFileProcessLog WHERE LogId = @LogId";
            await ExecuteAsync(sql, new { LogId = logId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除處理記錄失敗: LogId={logId}", ex);
            throw;
        }
    }
}

