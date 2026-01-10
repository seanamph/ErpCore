using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using ErpCore.Application.DTOs.Query;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 零用金請款檔 Repository 實作 (SYSQ220)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PcCashRequestRepository : BaseRepository, IPcCashRequestRepository
{
    public PcCashRequestRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PcCashRequest?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcCashRequest 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<PcCashRequest>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金請款檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcCashRequest?> GetByRequestIdAsync(string requestId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcCashRequest 
                WHERE RequestId = @RequestId";

            return await QueryFirstOrDefaultAsync<PcCashRequest>(sql, new { RequestId = requestId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金請款檔失敗: {requestId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PcCashRequestDto>> QueryAsync(PcCashRequestQueryDto query)
    {
        try
        {
            var sql = @"
                SELECT 
                    pcr.*,
                    s.SiteName
                FROM PcCashRequest pcr
                LEFT JOIN Sites s ON pcr.SiteId = s.SiteId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND pcr.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (query.RequestDateFrom.HasValue)
            {
                sql += " AND pcr.RequestDate >= @RequestDateFrom";
                parameters.Add("RequestDateFrom", query.RequestDateFrom.Value);
            }

            if (query.RequestDateTo.HasValue)
            {
                sql += " AND pcr.RequestDate <= @RequestDateTo";
                parameters.Add("RequestDateTo", query.RequestDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.RequestStatus))
            {
                sql += " AND pcr.RequestStatus = @RequestStatus";
                parameters.Add("RequestStatus", query.RequestStatus);
            }

            // 排序
            sql += " ORDER BY pcr.RequestDate DESC, pcr.RequestId DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PcCashRequestDto>(sql, parameters);

            // 處理 CashIds JSON 轉換為 List
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.CashIds))
                {
                    try
                    {
                        item.CashIdList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(item.CashIds);
                    }
                    catch
                    {
                        item.CashIdList = new List<string>();
                    }
                }
            }

            // 查詢總數
            var countSql = @"SELECT COUNT(*) FROM PcCashRequest pcr WHERE 1=1";
            var countParameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND pcr.SiteId = @SiteId";
                countParameters.Add("SiteId", query.SiteId);
            }

            if (query.RequestDateFrom.HasValue)
            {
                countSql += " AND pcr.RequestDate >= @RequestDateFrom";
                countParameters.Add("RequestDateFrom", query.RequestDateFrom.Value);
            }

            if (query.RequestDateTo.HasValue)
            {
                countSql += " AND pcr.RequestDate <= @RequestDateTo";
                countParameters.Add("RequestDateTo", query.RequestDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.RequestStatus))
            {
                countSql += " AND pcr.RequestStatus = @RequestStatus";
                countParameters.Add("RequestStatus", query.RequestStatus);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<PcCashRequestDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金請款檔列表失敗", ex);
            throw;
        }
    }

    public async Task<PcCashRequest> CreateAsync(PcCashRequest entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO PcCashRequest (RequestId, SiteId, RequestDate, CashIds, RequestAmount, RequestStatus, Notes, BUser, BTime, CUser, CTime)
                VALUES (@RequestId, @SiteId, @RequestDate, @CashIds, @RequestAmount, @RequestStatus, @Notes, @BUser, @BTime, @CUser, @CTime);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.RequestId,
                entity.SiteId,
                entity.RequestDate,
                entity.CashIds,
                entity.RequestAmount,
                entity.RequestStatus,
                entity.Notes,
                entity.BUser,
                entity.BTime,
                entity.CUser,
                entity.CTime
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增零用金請款檔失敗", ex);
            throw;
        }
    }

    public async Task<PcCashRequest> UpdateAsync(PcCashRequest entity)
    {
        try
        {
            const string sql = @"
                UPDATE PcCashRequest 
                SET RequestDate = @RequestDate,
                    CashIds = @CashIds,
                    RequestAmount = @RequestAmount,
                    RequestStatus = @RequestStatus,
                    Notes = @Notes,
                    CUser = @CUser,
                    CTime = @CTime
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.RequestDate,
                entity.CashIds,
                entity.RequestAmount,
                entity.RequestStatus,
                entity.Notes,
                entity.CUser,
                entity.CTime
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金請款檔失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM PcCashRequest WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金請款檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<string> GenerateRequestIdAsync(string? siteId)
    {
        try
        {
            // 生成請款單號格式: PR{YYYYMMDD}{流水號}
            var prefix = $"PR{DateTime.Now:yyyyMMdd}";
            var sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(RequestId, LEN(@Prefix) + 1, LEN(RequestId)) AS INT)), 0) + 1
                FROM PcCashRequest
                WHERE RequestId LIKE @Prefix + '%'";

            var parameters = new DynamicParameters();
            parameters.Add("Prefix", prefix);

            var sequence = await ExecuteScalarAsync<int>(sql, parameters);
            return $"{prefix}{sequence:D4}";
        }
        catch (Exception ex)
        {
            _logger.LogError("生成請款單號失敗", ex);
            throw;
        }
    }
}

