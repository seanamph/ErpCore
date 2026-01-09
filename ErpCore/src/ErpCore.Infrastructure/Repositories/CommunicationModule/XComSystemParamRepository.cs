using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CommunicationModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CommunicationModule;

/// <summary>
/// XCOM系統參數 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class XComSystemParamRepository : BaseRepository, IXComSystemParamRepository
{
    public XComSystemParamRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<XComSystemParam?> GetByIdAsync(string paramCode)
    {
        try
        {
            const string sql = @"
                SELECT * FROM XComSystemParams 
                WHERE ParamCode = @ParamCode";

            return await QueryFirstOrDefaultAsync<XComSystemParam>(sql, new { ParamCode = paramCode });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢XCOM系統參數失敗: {paramCode}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<XComSystemParam>> QueryAsync(XComSystemParamQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM XComSystemParams 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ParamCode))
            {
                sql += " AND ParamCode LIKE @ParamCode";
                parameters.Add("ParamCode", $"%{query.ParamCode}%");
            }

            if (!string.IsNullOrEmpty(query.ParamName))
            {
                sql += " AND ParamName LIKE @ParamName";
                parameters.Add("ParamName", $"%{query.ParamName}%");
            }

            if (!string.IsNullOrEmpty(query.ParamType))
            {
                sql += " AND ParamType = @ParamType";
                parameters.Add("ParamType", query.ParamType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            var sortField = string.IsNullOrEmpty(query.SortField) ? "ParamCode" : query.SortField;
            var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<XComSystemParam>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢XCOM系統參數列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(XComSystemParamQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM XComSystemParams 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ParamCode))
            {
                sql += " AND ParamCode LIKE @ParamCode";
                parameters.Add("ParamCode", $"%{query.ParamCode}%");
            }

            if (!string.IsNullOrEmpty(query.ParamName))
            {
                sql += " AND ParamName LIKE @ParamName";
                parameters.Add("ParamName", $"%{query.ParamName}%");
            }

            if (!string.IsNullOrEmpty(query.ParamType))
            {
                sql += " AND ParamType = @ParamType";
                parameters.Add("ParamType", query.ParamType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢XCOM系統參數數量失敗", ex);
            throw;
        }
    }

    public async Task CreateAsync(XComSystemParam entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO XComSystemParams 
                (ParamCode, ParamName, ParamValue, ParamType, Description, Status, SystemId, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup)
                VALUES 
                (@ParamCode, @ParamName, @ParamValue, @ParamType, @Description, @Status, @SystemId, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup)";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增XCOM系統參數失敗: {entity.ParamCode}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(XComSystemParam entity)
    {
        try
        {
            const string sql = @"
                UPDATE XComSystemParams 
                SET ParamName = @ParamName, 
                    ParamValue = @ParamValue, 
                    ParamType = @ParamType, 
                    Description = @Description, 
                    Status = @Status, 
                    SystemId = @SystemId, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE ParamCode = @ParamCode";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改XCOM系統參數失敗: {entity.ParamCode}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string paramCode)
    {
        try
        {
            const string sql = @"
                DELETE FROM XComSystemParams 
                WHERE ParamCode = @ParamCode";

            await ExecuteAsync(sql, new { ParamCode = paramCode });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除XCOM系統參數失敗: {paramCode}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string paramCode)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM XComSystemParams 
                WHERE ParamCode = @ParamCode";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { ParamCode = paramCode });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查XCOM系統參數是否存在失敗: {paramCode}", ex);
            throw;
        }
    }
}

