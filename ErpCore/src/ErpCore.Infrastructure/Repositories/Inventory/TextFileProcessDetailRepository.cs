using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 文本文件處理明細 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TextFileProcessDetailRepository : BaseRepository, ITextFileProcessDetailRepository
{
    public TextFileProcessDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<TextFileProcessDetail>> GetPagedByLogIdAsync(Guid logId, TextFileProcessDetailQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    DetailId, LogId, LineNumber, RawData, ProcessStatus, ErrorMessage, ProcessedData, CreatedAt
                FROM TextFileProcessDetail
                WHERE LogId = @LogId";

            var parameters = new DynamicParameters();
            parameters.Add("LogId", logId);

            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                sql += " AND ProcessStatus = @ProcessStatus";
                parameters.Add("ProcessStatus", query.ProcessStatus);
            }

            sql += " ORDER BY LineNumber";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<TextFileProcessDetail>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM TextFileProcessDetail
                WHERE LogId = @LogId";

            var countParameters = new DynamicParameters();
            countParameters.Add("LogId", logId);

            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                countSql += " AND ProcessStatus = @ProcessStatus";
                countParameters.Add("ProcessStatus", query.ProcessStatus);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<TextFileProcessDetail>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢處理明細列表失敗: LogId={logId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(IEnumerable<TextFileProcessDetail> details)
    {
        try
        {
            var sql = @"
                INSERT INTO TextFileProcessDetail 
                    (DetailId, LogId, LineNumber, RawData, ProcessStatus, ErrorMessage, ProcessedData, CreatedAt)
                VALUES 
                    (@DetailId, @LogId, @LineNumber, @RawData, @ProcessStatus, @ErrorMessage, @ProcessedData, @CreatedAt)";

            await ExecuteAsync(sql, details);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次建立處理明細失敗", ex);
            throw;
        }
    }

    public async Task UpdateBatchAsync(IEnumerable<TextFileProcessDetail> details)
    {
        try
        {
            var sql = @"
                UPDATE TextFileProcessDetail SET
                    ProcessStatus = @ProcessStatus,
                    ErrorMessage = @ErrorMessage,
                    ProcessedData = @ProcessedData
                WHERE DetailId = @DetailId";

            await ExecuteAsync(sql, details);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次更新處理明細失敗", ex);
            throw;
        }
    }

    public async Task DeleteByLogIdAsync(Guid logId)
    {
        try
        {
            var sql = "DELETE FROM TextFileProcessDetail WHERE LogId = @LogId";
            await ExecuteAsync(sql, new { LogId = logId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除處理明細失敗: LogId={logId}", ex);
            throw;
        }
    }
}

