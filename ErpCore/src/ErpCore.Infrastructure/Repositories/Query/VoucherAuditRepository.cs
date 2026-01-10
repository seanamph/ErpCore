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
/// 傳票審核傳送檔 Repository 實作 (SYSQ250)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class VoucherAuditRepository : BaseRepository, IVoucherAuditRepository
{
    public VoucherAuditRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<VoucherAudit?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM VoucherAudit 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<VoucherAudit>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票審核傳送檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<VoucherAudit?> GetByVoucherIdAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM VoucherAudit 
                WHERE VoucherId = @VoucherId";

            return await QueryFirstOrDefaultAsync<VoucherAudit>(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票審核傳送檔失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<VoucherAuditDto>> QueryAsync(VoucherAuditQueryDto query)
    {
        try
        {
            var sql = @"
                SELECT 
                    va.*,
                    u1.UserName AS AuditUserName
                FROM VoucherAudit va
                LEFT JOIN Users u1 ON va.AuditUser = u1.UserId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.VoucherDateFrom.HasValue)
            {
                sql += " AND va.VoucherDate >= @VoucherDateFrom";
                parameters.Add("VoucherDateFrom", query.VoucherDateFrom.Value);
            }

            if (query.VoucherDateTo.HasValue)
            {
                sql += " AND va.VoucherDate <= @VoucherDateTo";
                parameters.Add("VoucherDateTo", query.VoucherDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.AuditStatus))
            {
                sql += " AND va.AuditStatus = @AuditStatus";
                parameters.Add("AuditStatus", query.AuditStatus);
            }

            if (!string.IsNullOrEmpty(query.SendStatus))
            {
                sql += " AND va.SendStatus = @SendStatus";
                parameters.Add("SendStatus", query.SendStatus);
            }

            // 排序
            sql += " ORDER BY va.VoucherDate DESC, va.TKey DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<VoucherAuditDto>(sql, parameters);

            // 查詢總數
            var countSql = @"SELECT COUNT(*) FROM VoucherAudit va WHERE 1=1";
            var countParameters = new DynamicParameters();

            if (query.VoucherDateFrom.HasValue)
            {
                countSql += " AND va.VoucherDate >= @VoucherDateFrom";
                countParameters.Add("VoucherDateFrom", query.VoucherDateFrom.Value);
            }

            if (query.VoucherDateTo.HasValue)
            {
                countSql += " AND va.VoucherDate <= @VoucherDateTo";
                countParameters.Add("VoucherDateTo", query.VoucherDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.AuditStatus))
            {
                countSql += " AND va.AuditStatus = @AuditStatus";
                countParameters.Add("AuditStatus", query.AuditStatus);
            }

            if (!string.IsNullOrEmpty(query.SendStatus))
            {
                countSql += " AND va.SendStatus = @SendStatus";
                countParameters.Add("SendStatus", query.SendStatus);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<VoucherAuditDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢傳票審核傳送檔列表失敗", ex);
            throw;
        }
    }

    public async Task<VoucherAudit> CreateAsync(VoucherAudit entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO VoucherAudit (VoucherId, VoucherKind, VoucherDate, AuditStatus, AuditUser, AuditTime, AuditNotes, SendStatus, SendTime, Notes, BUser, BTime, CUser, CTime)
                VALUES (@VoucherId, @VoucherKind, @VoucherDate, @AuditStatus, @AuditUser, @AuditTime, @AuditNotes, @SendStatus, @SendTime, @Notes, @BUser, @BTime, @CUser, @CTime);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.VoucherId,
                entity.VoucherKind,
                entity.VoucherDate,
                entity.AuditStatus,
                entity.AuditUser,
                entity.AuditTime,
                entity.AuditNotes,
                entity.SendStatus,
                entity.SendTime,
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
            _logger.LogError("新增傳票審核傳送檔失敗", ex);
            throw;
        }
    }

    public async Task<VoucherAudit> UpdateAsync(VoucherAudit entity)
    {
        try
        {
            const string sql = @"
                UPDATE VoucherAudit 
                SET AuditStatus = @AuditStatus,
                    AuditUser = @AuditUser,
                    AuditTime = @AuditTime,
                    AuditNotes = @AuditNotes,
                    SendStatus = @SendStatus,
                    SendTime = @SendTime,
                    Notes = @Notes,
                    CUser = @CUser,
                    CTime = @CTime
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.AuditStatus,
                entity.AuditUser,
                entity.AuditTime,
                entity.AuditNotes,
                entity.SendStatus,
                entity.SendTime,
                entity.Notes,
                entity.CUser,
                entity.CTime
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改傳票審核傳送檔失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM VoucherAudit WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除傳票審核傳送檔失敗: {tKey}", ex);
            throw;
        }
    }
}

