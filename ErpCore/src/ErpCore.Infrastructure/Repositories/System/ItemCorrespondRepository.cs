using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 項目對應 Repository 實作 (SYS0360)
/// </summary>
public class ItemCorrespondRepository : BaseRepository, IItemCorrespondRepository
{
    public ItemCorrespondRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ItemCorrespond>> QueryAsync(ItemCorrespondQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ItemCorresponds
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ItemId))
            {
                sql += " AND ItemId LIKE @ItemId";
                parameters.Add("ItemId", $"%{query.ItemId}%");
            }

            if (!string.IsNullOrEmpty(query.ItemName))
            {
                sql += " AND ItemName LIKE @ItemName";
                parameters.Add("ItemName", $"%{query.ItemName}%");
            }

            if (!string.IsNullOrEmpty(query.ItemType))
            {
                sql += " AND ItemType = @ItemType";
                parameters.Add("ItemType", query.ItemType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ItemId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ItemCorrespond>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ItemCorresponds
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ItemId))
            {
                countSql += " AND ItemId LIKE @ItemId";
                countParameters.Add("ItemId", $"%{query.ItemId}%");
            }
            if (!string.IsNullOrEmpty(query.ItemName))
            {
                countSql += " AND ItemName LIKE @ItemName";
                countParameters.Add("ItemName", $"%{query.ItemName}%");
            }
            if (!string.IsNullOrEmpty(query.ItemType))
            {
                countSql += " AND ItemType = @ItemType";
                countParameters.Add("ItemType", query.ItemType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ItemCorrespond>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢項目對應列表失敗", ex);
            throw;
        }
    }

    public async Task<ItemCorrespond?> GetByIdAsync(string itemId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ItemCorresponds 
                WHERE ItemId = @ItemId";

            return await QueryFirstOrDefaultAsync<ItemCorrespond>(sql, new { ItemId = itemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目對應失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<ItemCorrespond> CreateAsync(ItemCorrespond itemCorrespond)
    {
        try
        {
            const string sql = @"
                INSERT INTO ItemCorresponds (
                    ItemId, ItemName, ItemType, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ItemId, @ItemName, @ItemType, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<ItemCorrespond>(sql, itemCorrespond);
            if (result == null)
            {
                throw new InvalidOperationException("新增項目對應失敗");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增項目對應失敗: {itemCorrespond.ItemId}", ex);
            throw;
        }
    }

    public async Task<ItemCorrespond> UpdateAsync(ItemCorrespond itemCorrespond)
    {
        try
        {
            const string sql = @"
                UPDATE ItemCorresponds SET
                    ItemName = @ItemName,
                    ItemType = @ItemType,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE ItemId = @ItemId";

            var result = await QueryFirstOrDefaultAsync<ItemCorrespond>(sql, itemCorrespond);
            if (result == null)
            {
                throw new InvalidOperationException($"項目對應不存在: {itemCorrespond.ItemId}");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改項目對應失敗: {itemCorrespond.ItemId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string itemId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ItemCorresponds
                WHERE ItemId = @ItemId";

            await ExecuteAsync(sql, new { ItemId = itemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除項目對應失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string itemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ItemCorresponds
                WHERE ItemId = @ItemId";

            var count = await QuerySingleAsync<int>(sql, new { ItemId = itemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查項目對應是否存在失敗: {itemId}", ex);
            throw;
        }
    }
}

