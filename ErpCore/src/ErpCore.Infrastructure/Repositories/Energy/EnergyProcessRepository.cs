using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Energy;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Energy;

/// <summary>
/// 能源處理 Repository 實作 (SYSO310 - 能源處理功能)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EnergyProcessRepository : BaseRepository, IEnergyProcessRepository
{
    public EnergyProcessRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EnergyProcess?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EnergyProcesses 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<EnergyProcess>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源處理資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EnergyProcess?> GetByProcessIdAsync(string processId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EnergyProcesses 
                WHERE ProcessId = @ProcessId";

            return await QueryFirstOrDefaultAsync<EnergyProcess>(sql, new { ProcessId = processId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源處理資料失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EnergyProcess>> QueryAsync(EnergyProcessQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EnergyProcesses
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProcessId))
            {
                sql += " AND ProcessId LIKE @ProcessId";
                parameters.Add("ProcessId", $"%{query.ProcessId}%");
            }

            if (!string.IsNullOrEmpty(query.EnergyId))
            {
                sql += " AND EnergyId = @EnergyId";
                parameters.Add("EnergyId", query.EnergyId);
            }

            if (query.ProcessDateFrom.HasValue)
            {
                sql += " AND ProcessDate >= @ProcessDateFrom";
                parameters.Add("ProcessDateFrom", query.ProcessDateFrom.Value);
            }

            if (query.ProcessDateTo.HasValue)
            {
                sql += " AND ProcessDate <= @ProcessDateTo";
                parameters.Add("ProcessDateTo", query.ProcessDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.ProcessType))
            {
                sql += " AND ProcessType = @ProcessType";
                parameters.Add("ProcessType", query.ProcessType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY ProcessDate DESC, ProcessId ASC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<EnergyProcess>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢能源處理資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(EnergyProcessQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM EnergyProcesses
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProcessId))
            {
                sql += " AND ProcessId LIKE @ProcessId";
                parameters.Add("ProcessId", $"%{query.ProcessId}%");
            }

            if (!string.IsNullOrEmpty(query.EnergyId))
            {
                sql += " AND EnergyId = @EnergyId";
                parameters.Add("EnergyId", query.EnergyId);
            }

            if (query.ProcessDateFrom.HasValue)
            {
                sql += " AND ProcessDate >= @ProcessDateFrom";
                parameters.Add("ProcessDateFrom", query.ProcessDateFrom.Value);
            }

            if (query.ProcessDateTo.HasValue)
            {
                sql += " AND ProcessDate <= @ProcessDateTo";
                parameters.Add("ProcessDateTo", query.ProcessDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.ProcessType))
            {
                sql += " AND ProcessType = @ProcessType";
                parameters.Add("ProcessType", query.ProcessType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢能源處理資料數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(EnergyProcess entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO EnergyProcesses 
                (ProcessId, EnergyId, ProcessDate, ProcessType, Amount, Cost, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ProcessId, @EnergyId, @ProcessDate, @ProcessType, @Amount, @Cost, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增能源處理資料失敗: {entity.ProcessId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(EnergyProcess entity)
    {
        try
        {
            const string sql = @"
                UPDATE EnergyProcesses 
                SET EnergyId = @EnergyId, 
                    ProcessDate = @ProcessDate, 
                    ProcessType = @ProcessType, 
                    Amount = @Amount, 
                    Cost = @Cost, 
                    Status = @Status, 
                    Notes = @Notes, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改能源處理資料失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM EnergyProcesses 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除能源處理資料失敗: {tKey}", ex);
            throw;
        }
    }
}

