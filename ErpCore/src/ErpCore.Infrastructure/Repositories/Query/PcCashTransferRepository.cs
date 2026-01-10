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
/// 零用金拋轉檔 Repository 實作 (SYSQ230)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PcCashTransferRepository : BaseRepository, IPcCashTransferRepository
{
    public PcCashTransferRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PcCashTransfer?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcCashTransfer 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<PcCashTransfer>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金拋轉檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcCashTransfer?> GetByTransferIdAsync(string transferId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcCashTransfer 
                WHERE TransferId = @TransferId";

            return await QueryFirstOrDefaultAsync<PcCashTransfer>(sql, new { TransferId = transferId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金拋轉檔失敗: {transferId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PcCashTransferDto>> QueryAsync(PcCashTransferQueryDto query)
    {
        try
        {
            var sql = @"
                SELECT 
                    pct.*,
                    s.SiteName
                FROM PcCashTransfer pct
                LEFT JOIN Sites s ON pct.SiteId = s.SiteId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND pct.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (query.TransferDateFrom.HasValue)
            {
                sql += " AND pct.TransferDate >= @TransferDateFrom";
                parameters.Add("TransferDateFrom", query.TransferDateFrom.Value);
            }

            if (query.TransferDateTo.HasValue)
            {
                sql += " AND pct.TransferDate <= @TransferDateTo";
                parameters.Add("TransferDateTo", query.TransferDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.TransferStatus))
            {
                sql += " AND pct.TransferStatus = @TransferStatus";
                parameters.Add("TransferStatus", query.TransferStatus);
            }

            if (!string.IsNullOrEmpty(query.VoucherId))
            {
                sql += " AND pct.VoucherId = @VoucherId";
                parameters.Add("VoucherId", query.VoucherId);
            }

            // 排序
            sql += " ORDER BY pct.TransferDate DESC, pct.TransferId DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PcCashTransferDto>(sql, parameters);

            // 查詢總數
            var countSql = @"SELECT COUNT(*) FROM PcCashTransfer pct WHERE 1=1";
            var countParameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND pct.SiteId = @SiteId";
                countParameters.Add("SiteId", query.SiteId);
            }

            if (query.TransferDateFrom.HasValue)
            {
                countSql += " AND pct.TransferDate >= @TransferDateFrom";
                countParameters.Add("TransferDateFrom", query.TransferDateFrom.Value);
            }

            if (query.TransferDateTo.HasValue)
            {
                countSql += " AND pct.TransferDate <= @TransferDateTo";
                countParameters.Add("TransferDateTo", query.TransferDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.TransferStatus))
            {
                countSql += " AND pct.TransferStatus = @TransferStatus";
                countParameters.Add("TransferStatus", query.TransferStatus);
            }

            if (!string.IsNullOrEmpty(query.VoucherId))
            {
                countSql += " AND pct.VoucherId = @VoucherId";
                countParameters.Add("VoucherId", query.VoucherId);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<PcCashTransferDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金拋轉檔列表失敗", ex);
            throw;
        }
    }

    public async Task<PcCashTransfer> CreateAsync(PcCashTransfer entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO PcCashTransfer (TransferId, SiteId, TransferDate, VoucherId, VoucherKind, VoucherDate, TransferAmount, TransferStatus, Notes, BUser, BTime, CUser, CTime)
                VALUES (@TransferId, @SiteId, @TransferDate, @VoucherId, @VoucherKind, @VoucherDate, @TransferAmount, @TransferStatus, @Notes, @BUser, @BTime, @CUser, @CTime);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.TransferId,
                entity.SiteId,
                entity.TransferDate,
                entity.VoucherId,
                entity.VoucherKind,
                entity.VoucherDate,
                entity.TransferAmount,
                entity.TransferStatus,
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
            _logger.LogError("新增零用金拋轉檔失敗", ex);
            throw;
        }
    }

    public async Task<PcCashTransfer> UpdateAsync(PcCashTransfer entity)
    {
        try
        {
            const string sql = @"
                UPDATE PcCashTransfer 
                SET TransferDate = @TransferDate,
                    VoucherId = @VoucherId,
                    VoucherKind = @VoucherKind,
                    VoucherDate = @VoucherDate,
                    TransferAmount = @TransferAmount,
                    TransferStatus = @TransferStatus,
                    Notes = @Notes,
                    CUser = @CUser,
                    CTime = @CTime
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.TransferDate,
                entity.VoucherId,
                entity.VoucherKind,
                entity.VoucherDate,
                entity.TransferAmount,
                entity.TransferStatus,
                entity.Notes,
                entity.CUser,
                entity.CTime
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金拋轉檔失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM PcCashTransfer WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金拋轉檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<string> GenerateTransferIdAsync(string? siteId)
    {
        try
        {
            // 生成拋轉單號格式: PT{YYYYMMDD}{流水號}
            var prefix = $"PT{DateTime.Now:yyyyMMdd}";
            var sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(TransferId, LEN(@Prefix) + 1, LEN(TransferId)) AS INT)), 0) + 1
                FROM PcCashTransfer
                WHERE TransferId LIKE @Prefix + '%'";

            var parameters = new DynamicParameters();
            parameters.Add("Prefix", prefix);

            var sequence = await ExecuteScalarAsync<int>(sql, parameters);
            return $"{prefix}{sequence:D4}";
        }
        catch (Exception ex)
        {
            _logger.LogError("生成拋轉單號失敗", ex);
            throw;
        }
    }
}

