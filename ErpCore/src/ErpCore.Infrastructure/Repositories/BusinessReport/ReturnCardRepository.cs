using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 銷退卡 Repository 實作 (SYSL310)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ReturnCardRepository : BaseRepository, IReturnCardRepository
{
    public ReturnCardRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ReturnCard?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT 
                    rc.TKey,
                    rc.Uuid,
                    rc.SiteId,
                    s.SiteName,
                    rc.OrgId,
                    o.OrgName,
                    rc.CardYear,
                    rc.CardMonth,
                    rc.CardType,
                    rc.Status,
                    rc.Notes,
                    rc.CreatedBy,
                    rc.CreatedAt,
                    rc.UpdatedBy,
                    rc.UpdatedAt
                FROM ReturnCards rc
                LEFT JOIN Sites s ON rc.SiteId = s.SiteId
                LEFT JOIN Organizations o ON rc.OrgId = o.OrgId
                WHERE rc.TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ReturnCard>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷退卡失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ReturnCard?> GetByUuidAsync(Guid uuid)
    {
        try
        {
            const string sql = @"
                SELECT 
                    rc.TKey,
                    rc.Uuid,
                    rc.SiteId,
                    s.SiteName,
                    rc.OrgId,
                    o.OrgName,
                    rc.CardYear,
                    rc.CardMonth,
                    rc.CardType,
                    rc.Status,
                    rc.Notes,
                    rc.CreatedBy,
                    rc.CreatedAt,
                    rc.UpdatedBy,
                    rc.UpdatedAt
                FROM ReturnCards rc
                LEFT JOIN Sites s ON rc.SiteId = s.SiteId
                LEFT JOIN Organizations o ON rc.OrgId = o.OrgId
                WHERE rc.Uuid = @Uuid";

            return await QueryFirstOrDefaultAsync<ReturnCard>(sql, new { Uuid = uuid });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷退卡失敗: {uuid}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ReturnCard>> QueryAsync(ReturnCardQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    rc.TKey,
                    rc.Uuid,
                    rc.SiteId,
                    s.SiteName,
                    rc.OrgId,
                    o.OrgName,
                    rc.CardYear,
                    rc.CardMonth,
                    rc.CardType,
                    rc.Status,
                    rc.Notes,
                    rc.CreatedBy,
                    rc.CreatedAt,
                    rc.UpdatedBy,
                    rc.UpdatedAt
                FROM ReturnCards rc
                LEFT JOIN Sites s ON rc.SiteId = s.SiteId
                LEFT JOIN Organizations o ON rc.OrgId = o.OrgId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND rc.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND rc.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (query.CardYear.HasValue)
            {
                sql += " AND rc.CardYear = @CardYear";
                parameters.Add("CardYear", query.CardYear.Value);
            }

            if (query.CardMonth.HasValue)
            {
                sql += " AND rc.CardMonth = @CardMonth";
                parameters.Add("CardMonth", query.CardMonth.Value);
            }

            // 計算總筆數
            var countSql = "SELECT COUNT(*) FROM ReturnCards rc WHERE 1=1";
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND rc.SiteId = @SiteId";
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND rc.OrgId = @OrgId";
            }
            if (query.CardYear.HasValue)
            {
                countSql += " AND rc.CardYear = @CardYear";
            }
            if (query.CardMonth.HasValue)
            {
                countSql += " AND rc.CardMonth = @CardMonth";
            }

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 排序
            var sortField = query.SortField ?? "CardYear";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY rc.{sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = (await QueryAsync<ReturnCard>(sql, parameters)).ToList();

            return new PagedResult<ReturnCard>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷退卡列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(ReturnCard returnCard)
    {
        try
        {
            const string sql = @"
                INSERT INTO ReturnCards 
                (Uuid, SiteId, OrgId, CardYear, CardMonth, CardType, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@Uuid, @SiteId, @OrgId, @CardYear, @CardMonth, @CardType, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, returnCard);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增銷退卡失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(ReturnCard returnCard)
    {
        try
        {
            const string sql = @"
                UPDATE ReturnCards SET
                    OrgId = @OrgId,
                    CardYear = @CardYear,
                    CardMonth = @CardMonth,
                    CardType = @CardType,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, returnCard);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷退卡失敗: {returnCard.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = "DELETE FROM ReturnCards WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷退卡失敗: {tKey}", ex);
            throw;
        }
    }
}

