using Dapper;
using ErpCore.Domain.Entities.Tools;
using ErpCore.Infrastructure.Repositories;

namespace ErpCore.Infrastructure.Repositories.Tools;

/// <summary>
/// PDF轉換記錄儲存庫實作
/// </summary>
public class PdfConversionLogRepository : BaseRepository, IPdfConversionLogRepository
{
    public PdfConversionLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PdfConversionLog> CreateAsync(PdfConversionLog entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[PdfConversionLogs] 
                ([LogId], [SourceHtml], [PdfFilePath], [FileName], [FileSize], [ConversionStatus], 
                 [ErrorMessage], [CreatedBy], [CreatedAt], [CompletedAt])
                VALUES 
                (@LogId, @SourceHtml, @PdfFilePath, @FileName, @FileSize, @ConversionStatus, 
                 @ErrorMessage, @CreatedBy, @CreatedAt, @CompletedAt)";

            await ExecuteAsync(sql, entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立PDF轉換記錄失敗", ex);
            throw;
        }
    }

    public async Task<PdfConversionLog?> GetByIdAsync(Guid id)
    {
        try
        {
            var sql = @"
                SELECT [LogId], [SourceHtml], [PdfFilePath], [FileName], [FileSize], [ConversionStatus],
                       [ErrorMessage], [CreatedBy], [CreatedAt], [CompletedAt]
                FROM [dbo].[PdfConversionLogs]
                WHERE [LogId] = @LogId";

            return await QueryFirstOrDefaultAsync<PdfConversionLog>(sql, new { LogId = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢PDF轉換記錄失敗: {id}", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(PdfConversionLog entity)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[PdfConversionLogs]
                SET [PdfFilePath] = @PdfFilePath,
                    [FileName] = @FileName,
                    [FileSize] = @FileSize,
                    [ConversionStatus] = @ConversionStatus,
                    [ErrorMessage] = @ErrorMessage,
                    [CompletedAt] = @CompletedAt
                WHERE [LogId] = @LogId";

            var rowsAffected = await ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新PDF轉換記錄失敗: {entity.LogId}", ex);
            throw;
        }
    }
}

