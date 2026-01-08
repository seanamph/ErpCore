using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 現金流量大分類 Repository 實作 (SYST131)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CashFlowLargeTypeRepository : BaseRepository, ICashFlowLargeTypeRepository
{
    public CashFlowLargeTypeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CashFlowLargeType?> GetByIdAsync(string cashLTypeId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CashFlowLargeTypes 
                WHERE CashLTypeId = @CashLTypeId";

            return await QueryFirstOrDefaultAsync<CashFlowLargeType>(sql, new { CashLTypeId = cashLTypeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現金流量大分類失敗: {cashLTypeId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CashFlowLargeType>> QueryAsync(CashFlowLargeTypeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM CashFlowLargeTypes
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CashLTypeId))
            {
                sql += " AND CashLTypeId LIKE @CashLTypeId";
                parameters.Add("CashLTypeId", $"%{query.CashLTypeId}%");
            }

            if (!string.IsNullOrEmpty(query.CashLTypeName))
            {
                sql += " AND CashLTypeName LIKE @CashLTypeName";
                parameters.Add("CashLTypeName", $"%{query.CashLTypeName}%");
            }

            if (!string.IsNullOrEmpty(query.AbItem))
            {
                sql += " AND AbItem = @AbItem";
                parameters.Add("AbItem", query.AbItem);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CashLTypeId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<CashFlowLargeType>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM CashFlowLargeTypes
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.CashLTypeId))
            {
                countSql += " AND CashLTypeId LIKE @CashLTypeId";
                countParameters.Add("CashLTypeId", $"%{query.CashLTypeId}%");
            }
            if (!string.IsNullOrEmpty(query.CashLTypeName))
            {
                countSql += " AND CashLTypeName LIKE @CashLTypeName";
                countParameters.Add("CashLTypeName", $"%{query.CashLTypeName}%");
            }
            if (!string.IsNullOrEmpty(query.AbItem))
            {
                countSql += " AND AbItem = @AbItem";
                countParameters.Add("AbItem", query.AbItem);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<CashFlowLargeType>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現金流量大分類列表失敗", ex);
            throw;
        }
    }

    public async Task<CashFlowLargeType> CreateAsync(CashFlowLargeType entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO CashFlowLargeTypes (
                    CashLTypeId, CashLTypeName, AbItem, Sn, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @CashLTypeId, @CashLTypeName, @AbItem, @Sn, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, new
            {
                entity.CashLTypeId,
                entity.CashLTypeName,
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
            _logger.LogError($"新增現金流量大分類失敗: {entity.CashLTypeId}", ex);
            throw;
        }
    }

    public async Task<CashFlowLargeType> UpdateAsync(CashFlowLargeType entity)
    {
        try
        {
            const string sql = @"
                UPDATE CashFlowLargeTypes SET
                    CashLTypeName = @CashLTypeName,
                    AbItem = @AbItem,
                    Sn = @Sn,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE CashLTypeId = @CashLTypeId";

            await ExecuteAsync(sql, new
            {
                entity.CashLTypeId,
                entity.CashLTypeName,
                entity.AbItem,
                entity.Sn,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現金流量大分類失敗: {entity.CashLTypeId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string cashLTypeId)
    {
        try
        {
            const string sql = @"
                DELETE FROM CashFlowLargeTypes
                WHERE CashLTypeId = @CashLTypeId";

            await ExecuteAsync(sql, new { CashLTypeId = cashLTypeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現金流量大分類失敗: {cashLTypeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cashLTypeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CashFlowLargeTypes
                WHERE CashLTypeId = @CashLTypeId";

            var count = await QuerySingleAsync<int>(sql, new { CashLTypeId = cashLTypeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量大分類是否存在失敗: {cashLTypeId}", ex);
            throw;
        }
    }

    public async Task<bool> HasMediumTypesAsync(string cashLTypeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CashFlowMediumTypes
                WHERE CashLTypeId = @CashLTypeId";

            var count = await QuerySingleAsync<int>(sql, new { CashLTypeId = cashLTypeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量大分類是否有中分類失敗: {cashLTypeId}", ex);
            throw;
        }
    }
}

