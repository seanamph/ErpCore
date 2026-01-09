using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StoreMember;

/// <summary>
/// 促銷活動 Repository 實作 (SYS3510-SYS3600 - 促銷活動維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PromotionRepository : BaseRepository, IPromotionRepository
{
    public PromotionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Promotion?> GetByIdAsync(string promotionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Promotions 
                WHERE PromotionId = @PromotionId";

            return await QueryFirstOrDefaultAsync<Promotion>(sql, new { PromotionId = promotionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢促銷活動失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Promotion>> QueryAsync(PromotionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Promotions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PromotionId))
            {
                sql += " AND PromotionId LIKE @PromotionId";
                parameters.Add("PromotionId", $"%{query.PromotionId}%");
            }

            if (!string.IsNullOrEmpty(query.PromotionName))
            {
                sql += " AND PromotionName LIKE @PromotionName";
                parameters.Add("PromotionName", $"%{query.PromotionName}%");
            }

            if (!string.IsNullOrEmpty(query.PromotionType))
            {
                sql += " AND PromotionType = @PromotionType";
                parameters.Add("PromotionType", query.PromotionType);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND StartDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND EndDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "PromotionId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Promotion>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<Promotion>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢促銷活動列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PromotionQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Promotions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PromotionId))
            {
                sql += " AND PromotionId LIKE @PromotionId";
                parameters.Add("PromotionId", $"%{query.PromotionId}%");
            }

            if (!string.IsNullOrEmpty(query.PromotionName))
            {
                sql += " AND PromotionName LIKE @PromotionName";
                parameters.Add("PromotionName", $"%{query.PromotionName}%");
            }

            if (!string.IsNullOrEmpty(query.PromotionType))
            {
                sql += " AND PromotionType = @PromotionType";
                parameters.Add("PromotionType", query.PromotionType);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND StartDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND EndDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
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
            _logger.LogError("查詢促銷活動總數失敗", ex);
            throw;
        }
    }

    public async Task<Promotion> CreateAsync(Promotion promotion)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            promotion.CreatedAt = DateTime.Now;
            promotion.UpdatedAt = DateTime.Now;

            const string sql = @"
                INSERT INTO Promotions (
                    PromotionId, PromotionName, PromotionType, StartDate, EndDate,
                    DiscountType, DiscountValue, MinPurchaseAmount, MaxDiscountAmount,
                    ApplicableShops, ApplicableProducts, ApplicableMemberLevels,
                    Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @PromotionId, @PromotionName, @PromotionType, @StartDate, @EndDate,
                    @DiscountType, @DiscountValue, @MinPurchaseAmount, @MaxDiscountAmount,
                    @ApplicableShops, @ApplicableProducts, @ApplicableMemberLevels,
                    @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await connection.QuerySingleAsync<long>(sql, promotion, transaction);
            promotion.TKey = tKey;

            transaction.Commit();
            _logger.LogInfo($"建立促銷活動成功: {promotion.PromotionId}");
            return promotion;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立促銷活動失敗: {promotion.PromotionId}", ex);
            throw;
        }
    }

    public async Task<Promotion> UpdateAsync(Promotion promotion)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            promotion.UpdatedAt = DateTime.Now;

            const string sql = @"
                UPDATE Promotions SET
                    PromotionName = @PromotionName,
                    PromotionType = @PromotionType,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    DiscountType = @DiscountType,
                    DiscountValue = @DiscountValue,
                    MinPurchaseAmount = @MinPurchaseAmount,
                    MaxDiscountAmount = @MaxDiscountAmount,
                    ApplicableShops = @ApplicableShops,
                    ApplicableProducts = @ApplicableProducts,
                    ApplicableMemberLevels = @ApplicableMemberLevels,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PromotionId = @PromotionId";

            await connection.ExecuteAsync(sql, promotion, transaction);

            transaction.Commit();
            _logger.LogInfo($"更新促銷活動成功: {promotion.PromotionId}");
            return promotion;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新促銷活動失敗: {promotion.PromotionId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string promotionId)
    {
        try
        {
            const string sql = @"
                UPDATE Promotions SET
                    Status = 'I',
                    UpdatedAt = GETDATE()
                WHERE PromotionId = @PromotionId";

            await ExecuteAsync(sql, new { PromotionId = promotionId });
            _logger.LogInfo($"刪除促銷活動成功: {promotionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除促銷活動失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string promotionId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Promotions
                WHERE PromotionId = @PromotionId";

            var count = await ExecuteScalarAsync<int>(sql, new { PromotionId = promotionId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查促銷活動編號是否存在失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string promotionId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE Promotions SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE PromotionId = @PromotionId";

            await ExecuteAsync(sql, new { PromotionId = promotionId, Status = status });
            _logger.LogInfo($"更新促銷活動狀態成功: {promotionId}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新促銷活動狀態失敗: {promotionId}", ex);
            throw;
        }
    }
}

