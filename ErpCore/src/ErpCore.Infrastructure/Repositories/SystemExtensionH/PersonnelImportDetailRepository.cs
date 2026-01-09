using System.Data;
using Dapper;
using ErpCore.Domain.Entities.SystemExtensionH;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionH;

/// <summary>
/// 人事匯入明細 Repository 實作 (SYSH3D0_FMI - 人事批量新增)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PersonnelImportDetailRepository : BaseRepository, IPersonnelImportDetailRepository
{
    public PersonnelImportDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<IEnumerable<PersonnelImportDetail>> GetByImportIdAsync(string importId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PersonnelImportDetails 
                WHERE ImportId = @ImportId 
                ORDER BY RowNum";

            return await QueryAsync<PersonnelImportDetail>(sql, new { ImportId = importId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢人事匯入明細失敗: {importId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(IEnumerable<PersonnelImportDetail> details)
    {
        try
        {
            const string sql = @"
                INSERT INTO PersonnelImportDetails 
                (ImportId, RowNum, PersonnelId, PersonnelName, ImportStatus, ErrorMessage, CreatedAt)
                VALUES 
                (@ImportId, @RowNum, @PersonnelId, @PersonnelName, @ImportStatus, @ErrorMessage, @CreatedAt)";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, details);
        }
        catch (Exception ex)
        {
            _logger.LogError("批量新增人事匯入明細失敗", ex);
            throw;
        }
    }

    public async Task<int> GetSuccessCountAsync(string importId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PersonnelImportDetails 
                WHERE ImportId = @ImportId AND ImportStatus = 'SUCCESS'";

            return await ExecuteScalarAsync<int>(sql, new { ImportId = importId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢成功筆數失敗: {importId}", ex);
            throw;
        }
    }

    public async Task<int> GetFailCountAsync(string importId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PersonnelImportDetails 
                WHERE ImportId = @ImportId AND ImportStatus = 'FAILED'";

            return await ExecuteScalarAsync<int>(sql, new { ImportId = importId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢失敗筆數失敗: {importId}", ex);
            throw;
        }
    }
}

