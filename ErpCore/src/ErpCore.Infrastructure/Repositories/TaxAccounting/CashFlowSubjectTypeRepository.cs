using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 現金流量科目設定 Repository 實作 (SYST133)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CashFlowSubjectTypeRepository : BaseRepository, ICashFlowSubjectTypeRepository
{
    public CashFlowSubjectTypeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CashFlowSubjectType?> GetByIdAsync(string cashMTypeId, string cashSTypeId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CashFlowSubjectTypes 
                WHERE CashMTypeId = @CashMTypeId AND CashSTypeId = @CashSTypeId";

            return await QueryFirstOrDefaultAsync<CashFlowSubjectType>(sql, new 
            { 
                CashMTypeId = cashMTypeId,
                CashSTypeId = cashSTypeId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現金流量科目設定失敗: {cashMTypeId}/{cashSTypeId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CashFlowSubjectType>> QueryAsync(CashFlowSubjectTypeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM CashFlowSubjectTypes
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CashMTypeId))
            {
                sql += " AND CashMTypeId LIKE @CashMTypeId";
                parameters.Add("CashMTypeId", $"%{query.CashMTypeId}%");
            }

            if (!string.IsNullOrEmpty(query.CashSTypeId))
            {
                sql += " AND CashSTypeId LIKE @CashSTypeId";
                parameters.Add("CashSTypeId", $"%{query.CashSTypeId}%");
            }

            // 排序
            sql += " ORDER BY CashMTypeId, CashSTypeId";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<CashFlowSubjectType>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM CashFlowSubjectTypes
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.CashMTypeId))
            {
                countSql += " AND CashMTypeId LIKE @CashMTypeId";
                countParameters.Add("CashMTypeId", $"%{query.CashMTypeId}%");
            }
            if (!string.IsNullOrEmpty(query.CashSTypeId))
            {
                countSql += " AND CashSTypeId LIKE @CashSTypeId";
                countParameters.Add("CashSTypeId", $"%{query.CashSTypeId}%");
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<CashFlowSubjectType>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現金流量科目設定列表失敗", ex);
            throw;
        }
    }

    public async Task<CashFlowSubjectType> CreateAsync(CashFlowSubjectType entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO CashFlowSubjectTypes (
                    CashMTypeId, CashSTypeId, AbItem, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @CashMTypeId, @CashSTypeId, @AbItem, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, new
            {
                entity.CashMTypeId,
                entity.CashSTypeId,
                entity.AbItem,
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
            _logger.LogError($"新增現金流量科目設定失敗: {entity.CashMTypeId}/{entity.CashSTypeId}", ex);
            throw;
        }
    }

    public async Task<CashFlowSubjectType> UpdateAsync(CashFlowSubjectType entity)
    {
        try
        {
            const string sql = @"
                UPDATE CashFlowSubjectTypes SET
                    AbItem = @AbItem,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE CashMTypeId = @CashMTypeId AND CashSTypeId = @CashSTypeId";

            await ExecuteAsync(sql, new
            {
                entity.CashMTypeId,
                entity.CashSTypeId,
                entity.AbItem,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現金流量科目設定失敗: {entity.CashMTypeId}/{entity.CashSTypeId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string cashMTypeId, string cashSTypeId)
    {
        try
        {
            const string sql = @"
                DELETE FROM CashFlowSubjectTypes
                WHERE CashMTypeId = @CashMTypeId AND CashSTypeId = @CashSTypeId";

            await ExecuteAsync(sql, new 
            { 
                CashMTypeId = cashMTypeId,
                CashSTypeId = cashSTypeId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現金流量科目設定失敗: {cashMTypeId}/{cashSTypeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cashMTypeId, string cashSTypeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CashFlowSubjectTypes
                WHERE CashMTypeId = @CashMTypeId AND CashSTypeId = @CashSTypeId";

            var count = await QuerySingleAsync<int>(sql, new 
            { 
                CashMTypeId = cashMTypeId,
                CashSTypeId = cashSTypeId
            });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量科目設定是否存在失敗: {cashMTypeId}/{cashSTypeId}", ex);
            throw;
        }
    }
}

