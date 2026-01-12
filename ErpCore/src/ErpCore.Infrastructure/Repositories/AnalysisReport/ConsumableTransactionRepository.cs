using Dapper;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 耗材異動記錄 Repository 實作 (SYSA255)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ConsumableTransactionRepository : BaseRepository, IConsumableTransactionRepository
{
    public ConsumableTransactionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ConsumableTransaction>> GetTransactionsAsync(ConsumableTransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    t.TransactionId, t.ConsumableId, t.TransactionType, t.TransactionDate,
                    t.Quantity, t.UnitPrice, t.Amount, t.SiteId, t.WarehouseId,
                    t.SourceId, t.Notes, t.CreatedBy, t.CreatedAt,
                    s.ShopName AS SiteName,
                    w.WarehouseName
                FROM ConsumableTransactions t
                LEFT JOIN Shops s ON t.SiteId = s.ShopId
                LEFT JOIN Warehouses w ON t.SiteId = w.SiteId AND t.WarehouseId = w.WarehouseId
                WHERE t.ConsumableId = @ConsumableId";

            var parameters = new DynamicParameters();
            parameters.Add("ConsumableId", query.ConsumableId);

            if (query.DateFrom.HasValue)
            {
                sql += " AND t.TransactionDate >= @DateFrom";
                parameters.Add("DateFrom", query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                sql += " AND t.TransactionDate <= @DateTo";
                parameters.Add("DateTo", query.DateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.TransactionType))
            {
                sql += " AND t.TransactionType = @TransactionType";
                parameters.Add("TransactionType", query.TransactionType);
            }

            // 排序
            var sortField = query.SortField ?? "TransactionDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 查詢總數
            var countSql = sql.Replace("SELECT t.TransactionId, t.ConsumableId", "SELECT COUNT(*)").Split("ORDER BY")[0];
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var items = await connection.QueryAsync<ConsumableTransaction>(sql, parameters);

            return new PagedResult<ConsumableTransaction>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材使用明細失敗", ex);
            throw;
        }
    }
}
