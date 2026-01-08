using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 現金流量小計設定 Repository 實作 (SYST134)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CashFlowSubTotalRepository : BaseRepository, ICashFlowSubTotalRepository
{
    public CashFlowSubTotalRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CashFlowSubTotal?> GetByIdAsync(string cashLTypeId, string cashSubId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CashFlowSubTotals 
                WHERE CashLTypeId = @CashLTypeId AND CashSubId = @CashSubId";

            return await QueryFirstOrDefaultAsync<CashFlowSubTotal>(sql, new 
            { 
                CashLTypeId = cashLTypeId,
                CashSubId = cashSubId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現金流量小計設定失敗: {cashLTypeId}/{cashSubId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CashFlowSubTotal>> QueryAsync(CashFlowSubTotalQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM CashFlowSubTotals
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CashLTypeId))
            {
                sql += " AND CashLTypeId LIKE @CashLTypeId";
                parameters.Add("CashLTypeId", $"%{query.CashLTypeId}%");
            }

            if (!string.IsNullOrEmpty(query.CashSubId))
            {
                sql += " AND CashSubId LIKE @CashSubId";
                parameters.Add("CashSubId", $"%{query.CashSubId}%");
            }

            // 排序
            sql += " ORDER BY CashLTypeId, CashSubId";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<CashFlowSubTotal>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM CashFlowSubTotals
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.CashLTypeId))
            {
                countSql += " AND CashLTypeId LIKE @CashLTypeId";
                countParameters.Add("CashLTypeId", $"%{query.CashLTypeId}%");
            }
            if (!string.IsNullOrEmpty(query.CashSubId))
            {
                countSql += " AND CashSubId LIKE @CashSubId";
                countParameters.Add("CashSubId", $"%{query.CashSubId}%");
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<CashFlowSubTotal>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現金流量小計設定列表失敗", ex);
            throw;
        }
    }

    public async Task<CashFlowSubTotal> CreateAsync(CashFlowSubTotal entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO CashFlowSubTotals (
                    CashLTypeId, CashSubId, CashSubName, CashMTypeIdB, CashMTypeIdE, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @CashLTypeId, @CashSubId, @CashSubName, @CashMTypeIdB, @CashMTypeIdE, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, new
            {
                entity.CashLTypeId,
                entity.CashSubId,
                entity.CashSubName,
                entity.CashMTypeIdB,
                entity.CashMTypeIdE,
                entity.CreatedBy,
                entity.CreatedAt,
                entity.UpdatedBy,
                entity.UpdatedAt,
                entity.CreatedPriority,
                entity.CreatedGroup
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增現金流量小計設定失敗: {entity.CashLTypeId}/{entity.CashSubId}", ex);
            throw;
        }
    }

    public async Task<CashFlowSubTotal> UpdateAsync(CashFlowSubTotal entity)
    {
        try
        {
            const string sql = @"
                UPDATE CashFlowSubTotals SET
                    CashSubName = @CashSubName,
                    CashMTypeIdB = @CashMTypeIdB,
                    CashMTypeIdE = @CashMTypeIdE,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE CashLTypeId = @CashLTypeId AND CashSubId = @CashSubId";

            await ExecuteAsync(sql, new
            {
                entity.CashLTypeId,
                entity.CashSubId,
                entity.CashSubName,
                entity.CashMTypeIdB,
                entity.CashMTypeIdE,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現金流量小計設定失敗: {entity.CashLTypeId}/{entity.CashSubId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string cashLTypeId, string cashSubId)
    {
        try
        {
            const string sql = @"
                DELETE FROM CashFlowSubTotals
                WHERE CashLTypeId = @CashLTypeId AND CashSubId = @CashSubId";

            await ExecuteAsync(sql, new 
            { 
                CashLTypeId = cashLTypeId,
                CashSubId = cashSubId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現金流量小計設定失敗: {cashLTypeId}/{cashSubId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cashLTypeId, string cashSubId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CashFlowSubTotals
                WHERE CashLTypeId = @CashLTypeId AND CashSubId = @CashSubId";

            var count = await QuerySingleAsync<int>(sql, new 
            { 
                CashLTypeId = cashLTypeId,
                CashSubId = cashSubId
            });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量小計設定是否存在失敗: {cashLTypeId}/{cashSubId}", ex);
            throw;
        }
    }
}

