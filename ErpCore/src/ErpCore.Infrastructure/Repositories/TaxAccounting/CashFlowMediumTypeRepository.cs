using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 現金流量中分類 Repository 實作 (SYST132)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CashFlowMediumTypeRepository : BaseRepository, ICashFlowMediumTypeRepository
{
    public CashFlowMediumTypeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CashFlowMediumType?> GetByIdAsync(string cashLTypeId, string cashMTypeId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CashFlowMediumTypes 
                WHERE CashLTypeId = @CashLTypeId AND CashMTypeId = @CashMTypeId";

            return await QueryFirstOrDefaultAsync<CashFlowMediumType>(sql, new 
            { 
                CashLTypeId = cashLTypeId,
                CashMTypeId = cashMTypeId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現金流量中分類失敗: {cashLTypeId}/{cashMTypeId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CashFlowMediumType>> QueryAsync(CashFlowMediumTypeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM CashFlowMediumTypes
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CashLTypeId))
            {
                sql += " AND CashLTypeId LIKE @CashLTypeId";
                parameters.Add("CashLTypeId", $"%{query.CashLTypeId}%");
            }

            if (!string.IsNullOrEmpty(query.CashMTypeId))
            {
                sql += " AND CashMTypeId LIKE @CashMTypeId";
                parameters.Add("CashMTypeId", $"%{query.CashMTypeId}%");
            }

            if (!string.IsNullOrEmpty(query.CashMTypeName))
            {
                sql += " AND CashMTypeName LIKE @CashMTypeName";
                parameters.Add("CashMTypeName", $"%{query.CashMTypeName}%");
            }

            if (!string.IsNullOrEmpty(query.AbItem))
            {
                sql += " AND AbItem = @AbItem";
                parameters.Add("AbItem", query.AbItem);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CashLTypeId, CashMTypeId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<CashFlowMediumType>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM CashFlowMediumTypes
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.CashLTypeId))
            {
                countSql += " AND CashLTypeId LIKE @CashLTypeId";
                countParameters.Add("CashLTypeId", $"%{query.CashLTypeId}%");
            }
            if (!string.IsNullOrEmpty(query.CashMTypeId))
            {
                countSql += " AND CashMTypeId LIKE @CashMTypeId";
                countParameters.Add("CashMTypeId", $"%{query.CashMTypeId}%");
            }
            if (!string.IsNullOrEmpty(query.CashMTypeName))
            {
                countSql += " AND CashMTypeName LIKE @CashMTypeName";
                countParameters.Add("CashMTypeName", $"%{query.CashMTypeName}%");
            }
            if (!string.IsNullOrEmpty(query.AbItem))
            {
                countSql += " AND AbItem = @AbItem";
                countParameters.Add("AbItem", query.AbItem);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<CashFlowMediumType>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現金流量中分類列表失敗", ex);
            throw;
        }
    }

    public async Task<CashFlowMediumType> CreateAsync(CashFlowMediumType entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO CashFlowMediumTypes (
                    CashLTypeId, CashMTypeId, CashMTypeName, AbItem, Sn, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @CashLTypeId, @CashMTypeId, @CashMTypeName, @AbItem, @Sn, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, new
            {
                entity.CashLTypeId,
                entity.CashMTypeId,
                entity.CashMTypeName,
                entity.AbItem,
                entity.Sn,
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
            _logger.LogError($"新增現金流量中分類失敗: {entity.CashLTypeId}/{entity.CashMTypeId}", ex);
            throw;
        }
    }

    public async Task<CashFlowMediumType> UpdateAsync(CashFlowMediumType entity)
    {
        try
        {
            const string sql = @"
                UPDATE CashFlowMediumTypes SET
                    CashMTypeName = @CashMTypeName,
                    AbItem = @AbItem,
                    Sn = @Sn,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE CashLTypeId = @CashLTypeId AND CashMTypeId = @CashMTypeId";

            await ExecuteAsync(sql, new
            {
                entity.CashLTypeId,
                entity.CashMTypeId,
                entity.CashMTypeName,
                entity.AbItem,
                entity.Sn,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現金流量中分類失敗: {entity.CashLTypeId}/{entity.CashMTypeId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string cashLTypeId, string cashMTypeId)
    {
        try
        {
            const string sql = @"
                DELETE FROM CashFlowMediumTypes
                WHERE CashLTypeId = @CashLTypeId AND CashMTypeId = @CashMTypeId";

            await ExecuteAsync(sql, new 
            { 
                CashLTypeId = cashLTypeId,
                CashMTypeId = cashMTypeId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現金流量中分類失敗: {cashLTypeId}/{cashMTypeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cashLTypeId, string cashMTypeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CashFlowMediumTypes
                WHERE CashLTypeId = @CashLTypeId AND CashMTypeId = @CashMTypeId";

            var count = await QuerySingleAsync<int>(sql, new 
            { 
                CashLTypeId = cashLTypeId,
                CashMTypeId = cashMTypeId
            });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量中分類是否存在失敗: {cashLTypeId}/{cashMTypeId}", ex);
            throw;
        }
    }

    public async Task<bool> HasSubjectTypesAsync(string cashMTypeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CashFlowSubjectTypes
                WHERE CashMTypeId = @CashMTypeId";

            var count = await QuerySingleAsync<int>(sql, new { CashMTypeId = cashMTypeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量中分類是否有科目設定失敗: {cashMTypeId}", ex);
            throw;
        }
    }
}

