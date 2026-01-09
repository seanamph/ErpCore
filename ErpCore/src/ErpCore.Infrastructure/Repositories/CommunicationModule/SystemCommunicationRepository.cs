using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CommunicationModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CommunicationModule;

/// <summary>
/// 系統通訊設定 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SystemCommunicationRepository : BaseRepository, ISystemCommunicationRepository
{
    public SystemCommunicationRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SystemCommunication?> GetByIdAsync(long communicationId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SystemCommunications 
                WHERE CommunicationId = @CommunicationId";

            return await QueryFirstOrDefaultAsync<SystemCommunication>(sql, new { CommunicationId = communicationId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統通訊設定失敗: {communicationId}", ex);
            throw;
        }
    }

    public async Task<SystemCommunication?> GetBySystemCodeAsync(string systemCode)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SystemCommunications 
                WHERE SystemCode = @SystemCode";

            return await QueryFirstOrDefaultAsync<SystemCommunication>(sql, new { SystemCode = systemCode });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統通訊設定失敗: {systemCode}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<SystemCommunication>> QueryAsync(SystemCommunicationQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM SystemCommunications 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SystemCode))
            {
                sql += " AND SystemCode LIKE @SystemCode";
                parameters.Add("SystemCode", $"%{query.SystemCode}%");
            }

            if (!string.IsNullOrEmpty(query.SystemName))
            {
                sql += " AND SystemName LIKE @SystemName";
                parameters.Add("SystemName", $"%{query.SystemName}%");
            }

            if (!string.IsNullOrEmpty(query.CommunicationType))
            {
                sql += " AND CommunicationType = @CommunicationType";
                parameters.Add("CommunicationType", query.CommunicationType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var sortField = string.IsNullOrEmpty(query.SortField) ? "CreatedAt" : query.SortField;
            var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<SystemCommunication>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統通訊設定列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(SystemCommunicationQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM SystemCommunications 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SystemCode))
            {
                sql += " AND SystemCode LIKE @SystemCode";
                parameters.Add("SystemCode", $"%{query.SystemCode}%");
            }

            if (!string.IsNullOrEmpty(query.SystemName))
            {
                sql += " AND SystemName LIKE @SystemName";
                parameters.Add("SystemName", $"%{query.SystemName}%");
            }

            if (!string.IsNullOrEmpty(query.CommunicationType))
            {
                sql += " AND CommunicationType = @CommunicationType";
                parameters.Add("CommunicationType", query.CommunicationType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統通訊設定數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(SystemCommunication entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO SystemCommunications 
                (SystemCode, SystemName, CommunicationType, EndpointUrl, ApiKey, ApiSecret, ConfigData, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@SystemCode, @SystemName, @CommunicationType, @EndpointUrl, @ApiKey, @ApiSecret, @ConfigData, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.QueryFirstOrDefaultAsync<long>(sql, entity);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增系統通訊設定失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(SystemCommunication entity)
    {
        try
        {
            const string sql = @"
                UPDATE SystemCommunications 
                SET SystemName = @SystemName, 
                    CommunicationType = @CommunicationType, 
                    EndpointUrl = @EndpointUrl, 
                    ApiKey = @ApiKey, 
                    ApiSecret = @ApiSecret, 
                    ConfigData = @ConfigData, 
                    Status = @Status, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE CommunicationId = @CommunicationId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改系統通訊設定失敗: {entity.CommunicationId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long communicationId)
    {
        try
        {
            const string sql = @"
                DELETE FROM SystemCommunications 
                WHERE CommunicationId = @CommunicationId";

            await ExecuteAsync(sql, new { CommunicationId = communicationId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除系統通訊設定失敗: {communicationId}", ex);
            throw;
        }
    }
}

