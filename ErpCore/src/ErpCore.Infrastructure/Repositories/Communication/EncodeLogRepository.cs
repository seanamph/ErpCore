using Dapper;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 編碼記錄儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EncodeLogRepository : BaseRepository, IEncodeLogRepository
{
    public EncodeLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EncodeLog> CreateAsync(EncodeLog entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[EncodeLogs] 
                ([EncodeType], [OriginalData], [EncodedData], [KeyKind], [CreatedBy], [CreatedAt], [Purpose])
                VALUES 
                (@EncodeType, @OriginalData, @EncodedData, @KeyKind, @CreatedBy, @CreatedAt, @Purpose);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.Id = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立編碼記錄失敗", ex);
            throw;
        }
    }

    public async Task<EncodeLog?> GetByIdAsync(long id)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[EncodeLogs]
                WHERE [Id] = @Id";

            return await QueryFirstOrDefaultAsync<EncodeLog>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢編碼記錄失敗: {id}", ex);
            throw;
        }
    }
}

