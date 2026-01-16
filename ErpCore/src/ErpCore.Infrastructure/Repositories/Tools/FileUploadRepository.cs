using Dapper;
using ErpCore.Domain.Entities.Tools;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Tools;

/// <summary>
/// 檔案上傳儲存庫實作
/// </summary>
public class FileUploadRepository : BaseRepository, IFileUploadRepository
{
    public FileUploadRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<FileUpload> CreateAsync(FileUpload entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[FileUploads] 
                ([FileName], [OriginalFileName], [FilePath], [FileSize], [FileType], [FileExtension], 
                 [UploadPath], [UploadedBy], [UploadedAt], [Status], [RelatedTable], [RelatedId], [Description])
                VALUES 
                (@FileName, @OriginalFileName, @FilePath, @FileSize, @FileType, @FileExtension, 
                 @UploadPath, @UploadedBy, @UploadedAt, @Status, @RelatedTable, @RelatedId, @Description);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.Id = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立檔案上傳記錄失敗", ex);
            throw;
        }
    }

    public async Task<FileUpload?> GetByIdAsync(long id)
    {
        try
        {
            var sql = @"
                SELECT [Id], [FileName], [OriginalFileName], [FilePath], [FileSize], [FileType], [FileExtension],
                       [UploadPath], [UploadedBy], [UploadedAt], [Status], [RelatedTable], [RelatedId], [Description]
                FROM [dbo].[FileUploads]
                WHERE [Id] = @Id";

            return await QueryFirstOrDefaultAsync<FileUpload>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢檔案上傳記錄失敗: {id}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<FileUpload>> GetListAsync(string? uploadedBy = null, string? relatedTable = null, string? relatedId = null, string? status = "1")
    {
        try
        {
            var sql = @"
                SELECT [Id], [FileName], [OriginalFileName], [FilePath], [FileSize], [FileType], [FileExtension],
                       [UploadPath], [UploadedBy], [UploadedAt], [Status], [RelatedTable], [RelatedId], [Description]
                FROM [dbo].[FileUploads]
                WHERE 1=1";

            var parameters = new DynamicParameters();
            
            if (!string.IsNullOrEmpty(uploadedBy))
            {
                sql += " AND [UploadedBy] = @UploadedBy";
                parameters.Add("UploadedBy", uploadedBy);
            }

            if (!string.IsNullOrEmpty(relatedTable))
            {
                sql += " AND [RelatedTable] = @RelatedTable";
                parameters.Add("RelatedTable", relatedTable);
            }

            if (!string.IsNullOrEmpty(relatedId))
            {
                sql += " AND [RelatedId] = @RelatedId";
                parameters.Add("RelatedId", relatedId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND [Status] = @Status";
                parameters.Add("Status", status);
            }

            sql += " ORDER BY [UploadedAt] DESC";

            return await QueryAsync<FileUpload>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢檔案上傳記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(FileUpload entity)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[FileUploads]
                SET [FileName] = @FileName,
                    [OriginalFileName] = @OriginalFileName,
                    [FilePath] = @FilePath,
                    [FileSize] = @FileSize,
                    [FileType] = @FileType,
                    [FileExtension] = @FileExtension,
                    [UploadPath] = @UploadPath,
                    [Status] = @Status,
                    [RelatedTable] = @RelatedTable,
                    [RelatedId] = @RelatedId,
                    [Description] = @Description
                WHERE [Id] = @Id";

            var rowsAffected = await ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新檔案上傳記錄失敗: {entity.Id}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[FileUploads]
                SET [Status] = '0'
                WHERE [Id] = @Id";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除檔案上傳記錄失敗: {id}", ex);
            throw;
        }
    }
}

