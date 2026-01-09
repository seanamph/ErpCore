using System.Data;
using Dapper;
using ErpCore.Domain.Entities.SystemExtensionH;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionH;

/// <summary>
/// 人事匯入記錄 Repository 實作 (SYSH3D0_FMI - 人事批量新增)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PersonnelImportLogRepository : BaseRepository, IPersonnelImportLogRepository
{
    public PersonnelImportLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PersonnelImportLog?> GetByIdAsync(string importId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PersonnelImportLogs 
                WHERE ImportId = @ImportId";

            return await QueryFirstOrDefaultAsync<PersonnelImportLog>(sql, new { ImportId = importId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢人事匯入記錄失敗: {importId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PersonnelImportLog>> QueryAsync(PersonnelImportLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PersonnelImportLogs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ImportId))
            {
                sql += " AND ImportId LIKE @ImportId";
                parameters.Add("ImportId", $"%{query.ImportId}%");
            }

            if (!string.IsNullOrEmpty(query.ImportStatus))
            {
                sql += " AND ImportStatus = @ImportStatus";
                parameters.Add("ImportStatus", query.ImportStatus);
            }

            if (query.ImportDateFrom.HasValue)
            {
                sql += " AND ImportDate >= @ImportDateFrom";
                parameters.Add("ImportDateFrom", query.ImportDateFrom.Value);
            }

            if (query.ImportDateTo.HasValue)
            {
                sql += " AND ImportDate <= @ImportDateTo";
                parameters.Add("ImportDateTo", query.ImportDateTo.Value);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY ImportDate DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<PersonnelImportLog>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢人事匯入記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PersonnelImportLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM PersonnelImportLogs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ImportId))
            {
                sql += " AND ImportId LIKE @ImportId";
                parameters.Add("ImportId", $"%{query.ImportId}%");
            }

            if (!string.IsNullOrEmpty(query.ImportStatus))
            {
                sql += " AND ImportStatus = @ImportStatus";
                parameters.Add("ImportStatus", query.ImportStatus);
            }

            if (query.ImportDateFrom.HasValue)
            {
                sql += " AND ImportDate >= @ImportDateFrom";
                parameters.Add("ImportDateFrom", query.ImportDateFrom.Value);
            }

            if (query.ImportDateTo.HasValue)
            {
                sql += " AND ImportDate <= @ImportDateTo";
                parameters.Add("ImportDateTo", query.ImportDateTo.Value);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢人事匯入記錄數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(PersonnelImportLog entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO PersonnelImportLogs 
                (ImportId, FileName, TotalCount, SuccessCount, FailCount, ImportStatus, ImportDate, CreatedBy, CreatedAt)
                VALUES 
                (@ImportId, @FileName, @TotalCount, @SuccessCount, @FailCount, @ImportStatus, @ImportDate, @CreatedBy, @CreatedAt)";

            await ExecuteAsync(sql, new
            {
                entity.ImportId,
                entity.FileName,
                entity.TotalCount,
                entity.SuccessCount,
                entity.FailCount,
                entity.ImportStatus,
                entity.ImportDate,
                entity.CreatedBy,
                entity.CreatedAt
            });

            return entity.ImportId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增人事匯入記錄失敗: {entity.ImportId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(PersonnelImportLog entity)
    {
        try
        {
            const string sql = @"
                UPDATE PersonnelImportLogs 
                SET FileName = @FileName,
                    TotalCount = @TotalCount,
                    SuccessCount = @SuccessCount,
                    FailCount = @FailCount,
                    ImportStatus = @ImportStatus,
                    ImportDate = @ImportDate
                WHERE ImportId = @ImportId";

            await ExecuteAsync(sql, new
            {
                entity.ImportId,
                entity.FileName,
                entity.TotalCount,
                entity.SuccessCount,
                entity.FailCount,
                entity.ImportStatus,
                entity.ImportDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新人事匯入記錄失敗: {entity.ImportId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string importId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE PersonnelImportLogs 
                SET ImportStatus = @Status
                WHERE ImportId = @ImportId";

            await ExecuteAsync(sql, new { ImportId = importId, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新人事匯入記錄狀態失敗: {importId}", ex);
            throw;
        }
    }

    public async Task<string> GenerateImportIdAsync()
    {
        try
        {
            const string sql = @"
                SELECT TOP 1 ImportId 
                FROM PersonnelImportLogs 
                WHERE ImportId LIKE 'IMP%'
                ORDER BY ImportId DESC";

            var lastId = await QueryFirstOrDefaultAsync<string>(sql);

            if (string.IsNullOrEmpty(lastId))
            {
                return "IMP001";
            }

            var number = int.Parse(lastId.Substring(3)) + 1;
            return $"IMP{number:D3}";
        }
        catch (Exception ex)
        {
            _logger.LogError("產生匯入批次編號失敗", ex);
            throw;
        }
    }
}

