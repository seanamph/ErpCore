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
/// 零用金主檔 Repository 實作 (SYSQ210)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PcCashRepository : BaseRepository, IPcCashRepository
{
    public PcCashRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PcCash?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcCash 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<PcCash>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcCash?> GetByCashIdAsync(string cashId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcCash 
                WHERE CashId = @CashId";

            return await QueryFirstOrDefaultAsync<PcCash>(sql, new { CashId = cashId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金失敗: {cashId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PcCashDto>> QueryAsync(PcCashQueryDto query)
    {
        try
        {
            var sql = @"
                SELECT 
                    pc.*,
                    s.SiteName,
                    e1.EmpName AS AppleNameDesc,
                    o.OrgName,
                    e2.EmpName AS KeepEmpName
                FROM PcCash pc
                LEFT JOIN Sites s ON pc.SiteId = s.SiteId
                LEFT JOIN V_EMP_USER e1 ON pc.AppleName = e1.EmpId
                LEFT JOIN Organizations o ON pc.OrgId = o.OrgId
                LEFT JOIN V_EMP_USER e2 ON pc.KeepEmpId = e2.EmpId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND pc.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (query.AppleDateFrom.HasValue)
            {
                sql += " AND pc.AppleDate >= @AppleDateFrom";
                parameters.Add("AppleDateFrom", query.AppleDateFrom.Value);
            }

            if (query.AppleDateTo.HasValue)
            {
                sql += " AND pc.AppleDate <= @AppleDateTo";
                parameters.Add("AppleDateTo", query.AppleDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.AppleName))
            {
                sql += " AND pc.AppleName LIKE @AppleName";
                parameters.Add("AppleName", $"%{query.AppleName}%");
            }

            if (!string.IsNullOrEmpty(query.KeepEmpId))
            {
                sql += " AND pc.KeepEmpId = @KeepEmpId";
                parameters.Add("KeepEmpId", query.KeepEmpId);
            }

            if (!string.IsNullOrEmpty(query.CashStatus))
            {
                sql += " AND pc.CashStatus = @CashStatus";
                parameters.Add("CashStatus", query.CashStatus);
            }

            // 排序
            var orderBy = !string.IsNullOrEmpty(query.OrderBy) ? query.OrderBy : "pc.AppleDate";
            var orderDirection = !string.IsNullOrEmpty(query.OrderDirection) ? query.OrderDirection : "DESC";
            sql += $" ORDER BY {orderBy} {orderDirection}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PcCashDto>(sql, parameters);

            // 查詢總數
            var countSql = @"SELECT COUNT(*) FROM PcCash pc WHERE 1=1";
            var countParameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND pc.SiteId = @SiteId";
                countParameters.Add("SiteId", query.SiteId);
            }

            if (query.AppleDateFrom.HasValue)
            {
                countSql += " AND pc.AppleDate >= @AppleDateFrom";
                countParameters.Add("AppleDateFrom", query.AppleDateFrom.Value);
            }

            if (query.AppleDateTo.HasValue)
            {
                countSql += " AND pc.AppleDate <= @AppleDateTo";
                countParameters.Add("AppleDateTo", query.AppleDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.AppleName))
            {
                countSql += " AND pc.AppleName LIKE @AppleName";
                countParameters.Add("AppleName", $"%{query.AppleName}%");
            }

            if (!string.IsNullOrEmpty(query.KeepEmpId))
            {
                countSql += " AND pc.KeepEmpId = @KeepEmpId";
                countParameters.Add("KeepEmpId", query.KeepEmpId);
            }

            if (!string.IsNullOrEmpty(query.CashStatus))
            {
                countSql += " AND pc.CashStatus = @CashStatus";
                countParameters.Add("CashStatus", query.CashStatus);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<PcCashDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金列表失敗", ex);
            throw;
        }
    }

    public async Task<PcCash> CreateAsync(PcCash entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO PcCash (CashId, SiteId, AppleDate, AppleName, OrgId, KeepEmpId, CashAmount, CashStatus, Notes, BUser, BTime, CUser, CTime, CPriority, CGroup)
                VALUES (@CashId, @SiteId, @AppleDate, @AppleName, @OrgId, @KeepEmpId, @CashAmount, @CashStatus, @Notes, @BUser, @BTime, @CUser, @CTime, @CPriority, @CGroup);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.CashId,
                entity.SiteId,
                entity.AppleDate,
                entity.AppleName,
                entity.OrgId,
                entity.KeepEmpId,
                entity.CashAmount,
                entity.CashStatus,
                entity.Notes,
                entity.BUser,
                entity.BTime,
                entity.CUser,
                entity.CTime,
                entity.CPriority,
                entity.CGroup
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增零用金失敗", ex);
            throw;
        }
    }

    public async Task<PcCash> UpdateAsync(PcCash entity)
    {
        try
        {
            const string sql = @"
                UPDATE PcCash 
                SET AppleDate = @AppleDate,
                    AppleName = @AppleName,
                    OrgId = @OrgId,
                    KeepEmpId = @KeepEmpId,
                    CashAmount = @CashAmount,
                    CashStatus = @CashStatus,
                    Notes = @Notes,
                    CUser = @CUser,
                    CTime = @CTime
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.AppleDate,
                entity.AppleName,
                entity.OrgId,
                entity.KeepEmpId,
                entity.CashAmount,
                entity.CashStatus,
                entity.Notes,
                entity.CUser,
                entity.CTime
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM PcCash WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<string> GenerateCashIdAsync(string? siteId)
    {
        try
        {
            // 生成零用金單號格式: PC{YYYYMMDD}{流水號}
            var prefix = $"PC{DateTime.Now:yyyyMMdd}";
            var sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(CashId, LEN(@Prefix) + 1, LEN(CashId)) AS INT)), 0) + 1
                FROM PcCash
                WHERE CashId LIKE @Prefix + '%'";

            var parameters = new DynamicParameters();
            parameters.Add("Prefix", prefix);

            var sequence = await ExecuteScalarAsync<int>(sql, parameters);
            return $"{prefix}{sequence:D4}";
        }
        catch (Exception ex)
        {
            _logger.LogError("生成零用金單號失敗", ex);
            throw;
        }
    }
}

