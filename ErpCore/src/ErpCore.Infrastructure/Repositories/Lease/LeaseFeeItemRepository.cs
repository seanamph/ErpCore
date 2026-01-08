using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 費用項目主檔 Repository 實作 (SYSE310-SYSE430)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseFeeItemRepository : BaseRepository, ILeaseFeeItemRepository
{
    public LeaseFeeItemRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LeaseFeeItem?> GetByIdAsync(string feeItemId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseFeeItems 
                WHERE FeeItemId = @FeeItemId";

            return await QueryFirstOrDefaultAsync<LeaseFeeItem>(sql, new { FeeItemId = feeItemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢費用項目失敗: {feeItemId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseFeeItem>> QueryAsync(LeaseFeeItemQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LeaseFeeItems 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FeeItemId))
            {
                sql += " AND FeeItemId LIKE @FeeItemId";
                parameters.Add("FeeItemId", $"%{query.FeeItemId}%");
            }

            if (!string.IsNullOrEmpty(query.FeeItemName))
            {
                sql += " AND FeeItemName LIKE @FeeItemName";
                parameters.Add("FeeItemName", $"%{query.FeeItemName}%");
            }

            if (!string.IsNullOrEmpty(query.FeeType))
            {
                sql += " AND FeeType = @FeeType";
                parameters.Add("FeeType", query.FeeType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY FeeItemId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LeaseFeeItem>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢費用項目列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseFeeItemQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LeaseFeeItems 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FeeItemId))
            {
                sql += " AND FeeItemId LIKE @FeeItemId";
                parameters.Add("FeeItemId", $"%{query.FeeItemId}%");
            }

            if (!string.IsNullOrEmpty(query.FeeItemName))
            {
                sql += " AND FeeItemName LIKE @FeeItemName";
                parameters.Add("FeeItemName", $"%{query.FeeItemName}%");
            }

            if (!string.IsNullOrEmpty(query.FeeType))
            {
                sql += " AND FeeType = @FeeType";
                parameters.Add("FeeType", query.FeeType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢費用項目數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string feeItemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM LeaseFeeItems 
                WHERE FeeItemId = @FeeItemId";

            var count = await ExecuteScalarAsync<int>(sql, new { FeeItemId = feeItemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查費用項目是否存在失敗: {feeItemId}", ex);
            throw;
        }
    }

    public async Task<LeaseFeeItem> CreateAsync(LeaseFeeItem feeItem)
    {
        try
        {
            const string sql = @"
                INSERT INTO LeaseFeeItems (
                    FeeItemId, FeeItemName, FeeType, DefaultAmount, Status, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @FeeItemId, @FeeItemName, @FeeType, @DefaultAmount, @Status, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                feeItem.FeeItemId,
                feeItem.FeeItemName,
                feeItem.FeeType,
                feeItem.DefaultAmount,
                feeItem.Status,
                feeItem.Memo,
                feeItem.CreatedBy,
                feeItem.CreatedAt,
                feeItem.UpdatedBy,
                feeItem.UpdatedAt
            });

            feeItem.TKey = tKey;
            return feeItem;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增費用項目失敗: {feeItem.FeeItemId}", ex);
            throw;
        }
    }

    public async Task<LeaseFeeItem> UpdateAsync(LeaseFeeItem feeItem)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseFeeItems SET
                    FeeItemName = @FeeItemName,
                    FeeType = @FeeType,
                    DefaultAmount = @DefaultAmount,
                    Status = @Status,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE FeeItemId = @FeeItemId";

            await ExecuteAsync(sql, new
            {
                feeItem.FeeItemId,
                feeItem.FeeItemName,
                feeItem.FeeType,
                feeItem.DefaultAmount,
                feeItem.Status,
                feeItem.Memo,
                feeItem.UpdatedBy,
                feeItem.UpdatedAt
            });

            return feeItem;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改費用項目失敗: {feeItem.FeeItemId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string feeItemId)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseFeeItems 
                WHERE FeeItemId = @FeeItemId";

            await ExecuteAsync(sql, new { FeeItemId = feeItemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除費用項目失敗: {feeItemId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string feeItemId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseFeeItems SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE FeeItemId = @FeeItemId";

            await ExecuteAsync(sql, new { FeeItemId = feeItemId, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新費用項目狀態失敗: {feeItemId}", ex);
            throw;
        }
    }
}

