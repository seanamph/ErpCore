using Dapper;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 耗材列印 Repository 實作 (SYSA254)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ConsumablePrintRepository : BaseRepository, IConsumablePrintRepository
{
    public ConsumablePrintRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<Consumable>> GetConsumablesForPrintAsync(ConsumablePrintQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    c.ConsumableId, c.ConsumableName, c.CategoryId, c.Unit, c.Specification,
                    c.Brand, c.Model, c.BarCode, c.Status, c.AssetStatus, c.SiteId,
                    c.Location, c.Quantity, c.MinQuantity, c.MaxQuantity, c.Price,
                    c.SupplierId, c.Notes, c.CreatedBy, c.CreatedAt, c.UpdatedBy, c.UpdatedAt
                FROM Consumables c
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND c.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND c.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.AssetStatus))
            {
                sql += " AND c.AssetStatus = @AssetStatus";
                parameters.Add("AssetStatus", query.AssetStatus);
            }

            if (query.ConsumableIds != null && query.ConsumableIds.Any())
            {
                sql += " AND c.ConsumableId IN @ConsumableIds";
                parameters.Add("ConsumableIds", query.ConsumableIds);
            }

            sql += " ORDER BY c.ConsumableId";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<Consumable>(sql, parameters);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材列表失敗", ex);
            throw;
        }
    }

    public async Task<ConsumablePrintLog> CreateLogAsync(ConsumablePrintLog log)
    {
        try
        {
            const string sql = @"
                INSERT INTO ConsumablePrintLogs (
                    LogId, ConsumableId, PrintType, PrintCount, PrintDate, PrintedBy, SiteId
                ) VALUES (
                    @LogId, @ConsumableId, @PrintType, @PrintCount, @PrintDate, @PrintedBy, @SiteId
                )";

            await ExecuteAsync(sql, log);
            return log;
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立列印記錄失敗: {log.ConsumableId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ConsumablePrintLog>> GetLogsAsync(ConsumablePrintLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ConsumablePrintLogs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ConsumableId))
            {
                sql += " AND ConsumableId LIKE @ConsumableId";
                parameters.Add("ConsumableId", $"%{query.ConsumableId}%");
            }

            if (!string.IsNullOrEmpty(query.PrintType))
            {
                sql += " AND PrintType = @PrintType";
                parameters.Add("PrintType", query.PrintType);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.PrintedBy))
            {
                sql += " AND PrintedBy = @PrintedBy";
                parameters.Add("PrintedBy", query.PrintedBy);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND PrintDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND PrintDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            // 排序
            sql += " ORDER BY PrintDate DESC";

            // 查詢總數
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ConsumablePrintLog>(sql, parameters);

            return new PagedResult<ConsumablePrintLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢列印記錄列表失敗", ex);
            throw;
        }
    }
}
