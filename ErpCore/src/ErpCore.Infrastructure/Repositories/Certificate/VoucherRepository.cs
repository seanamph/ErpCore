using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Certificate;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Certificate;

/// <summary>
/// 憑證 Repository 實作 (SYSK110-SYSK150)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class VoucherRepository : BaseRepository, IVoucherRepository
{
    public VoucherRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Voucher?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Vouchers 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Voucher>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢憑證失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Voucher?> GetByVoucherIdAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Vouchers 
                WHERE VoucherId = @VoucherId";

            return await QueryFirstOrDefaultAsync<Voucher>(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Voucher>> GetAllAsync()
    {
        try
        {
            const string sql = @"SELECT * FROM Vouchers ORDER BY VoucherDate DESC, VoucherId";

            return await QueryAsync<Voucher>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢憑證列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Voucher>> GetPagedAsync(int pageIndex, int pageSize, string? voucherId = null, string? voucherType = null, string? shopId = null, string? status = null, DateTime? voucherDateFrom = null, DateTime? voucherDateTo = null)
    {
        try
        {
            var whereClauses = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(voucherId))
            {
                whereClauses.Add("VoucherId LIKE @VoucherId");
                parameters.Add("VoucherId", $"%{voucherId}%");
            }

            if (!string.IsNullOrEmpty(voucherType))
            {
                whereClauses.Add("VoucherType = @VoucherType");
                parameters.Add("VoucherType", voucherType);
            }

            if (!string.IsNullOrEmpty(shopId))
            {
                whereClauses.Add("ShopId = @ShopId");
                parameters.Add("ShopId", shopId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                whereClauses.Add("Status = @Status");
                parameters.Add("Status", status);
            }

            if (voucherDateFrom.HasValue)
            {
                whereClauses.Add("VoucherDate >= @VoucherDateFrom");
                parameters.Add("VoucherDateFrom", voucherDateFrom.Value);
            }

            if (voucherDateTo.HasValue)
            {
                whereClauses.Add("VoucherDate <= @VoucherDateTo");
                parameters.Add("VoucherDateTo", voucherDateTo.Value);
            }

            var whereClause = whereClauses.Any() ? "WHERE " + string.Join(" AND ", whereClauses) : "";
            var offset = (pageIndex - 1) * pageSize;

            var sql = $@"
                SELECT * FROM Vouchers 
                {whereClause}
                ORDER BY VoucherDate DESC, VoucherId
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            return await QueryAsync<Voucher>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢憑證分頁列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(string? voucherId = null, string? voucherType = null, string? shopId = null, string? status = null, DateTime? voucherDateFrom = null, DateTime? voucherDateTo = null)
    {
        try
        {
            var whereClauses = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(voucherId))
            {
                whereClauses.Add("VoucherId LIKE @VoucherId");
                parameters.Add("VoucherId", $"%{voucherId}%");
            }

            if (!string.IsNullOrEmpty(voucherType))
            {
                whereClauses.Add("VoucherType = @VoucherType");
                parameters.Add("VoucherType", voucherType);
            }

            if (!string.IsNullOrEmpty(shopId))
            {
                whereClauses.Add("ShopId = @ShopId");
                parameters.Add("ShopId", shopId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                whereClauses.Add("Status = @Status");
                parameters.Add("Status", status);
            }

            if (voucherDateFrom.HasValue)
            {
                whereClauses.Add("VoucherDate >= @VoucherDateFrom");
                parameters.Add("VoucherDateFrom", voucherDateFrom.Value);
            }

            if (voucherDateTo.HasValue)
            {
                whereClauses.Add("VoucherDate <= @VoucherDateTo");
                parameters.Add("VoucherDateTo", voucherDateTo.Value);
            }

            var whereClause = whereClauses.Any() ? "WHERE " + string.Join(" AND ", whereClauses) : "";

            var sql = $@"SELECT COUNT(1) FROM Vouchers {whereClause}";

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢憑證數量失敗", ex);
            throw;
        }
    }

    public async Task<Voucher> CreateAsync(Voucher entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Vouchers (VoucherId, VoucherDate, VoucherType, ShopId, Status, ApplyUserId, ApplyDate, ApproveUserId, ApproveDate, TotalAmount, TotalDebitAmount, TotalCreditAmount, Memo, SiteId, OrgId, CurrencyId, ExchangeRate, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@VoucherId, @VoucherDate, @VoucherType, @ShopId, @Status, @ApplyUserId, @ApplyDate, @ApproveUserId, @ApproveDate, @TotalAmount, @TotalDebitAmount, @TotalCreditAmount, @Memo, @SiteId, @OrgId, @CurrencyId, @ExchangeRate, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.VoucherId,
                entity.VoucherDate,
                entity.VoucherType,
                entity.ShopId,
                entity.Status,
                entity.ApplyUserId,
                entity.ApplyDate,
                entity.ApproveUserId,
                entity.ApproveDate,
                entity.TotalAmount,
                entity.TotalDebitAmount,
                entity.TotalCreditAmount,
                entity.Memo,
                entity.SiteId,
                entity.OrgId,
                entity.CurrencyId,
                entity.ExchangeRate,
                entity.CreatedBy,
                entity.CreatedAt,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增憑證失敗", ex);
            throw;
        }
    }

    public async Task<Voucher> UpdateAsync(Voucher entity)
    {
        try
        {
            const string sql = @"
                UPDATE Vouchers 
                SET VoucherDate = @VoucherDate,
                    VoucherType = @VoucherType,
                    ShopId = @ShopId,
                    Status = @Status,
                    ApplyUserId = @ApplyUserId,
                    ApplyDate = @ApplyDate,
                    ApproveUserId = @ApproveUserId,
                    ApproveDate = @ApproveDate,
                    TotalAmount = @TotalAmount,
                    TotalDebitAmount = @TotalDebitAmount,
                    TotalCreditAmount = @TotalCreditAmount,
                    Memo = @Memo,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    CurrencyId = @CurrencyId,
                    ExchangeRate = @ExchangeRate,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE VoucherId = @VoucherId";

            await ExecuteAsync(sql, new
            {
                entity.VoucherId,
                entity.VoucherDate,
                entity.VoucherType,
                entity.ShopId,
                entity.Status,
                entity.ApplyUserId,
                entity.ApplyDate,
                entity.ApproveUserId,
                entity.ApproveDate,
                entity.TotalAmount,
                entity.TotalDebitAmount,
                entity.TotalCreditAmount,
                entity.Memo,
                entity.SiteId,
                entity.OrgId,
                entity.CurrencyId,
                entity.ExchangeRate,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新憑證失敗: {entity.VoucherId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string voucherId)
    {
        try
        {
            const string sql = @"DELETE FROM Vouchers WHERE VoucherId = @VoucherId";

            await ExecuteAsync(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) FROM Vouchers 
                WHERE VoucherId = @VoucherId";

            var count = await ExecuteScalarAsync<int>(sql, new { VoucherId = voucherId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查憑證是否存在失敗: {voucherId}", ex);
            throw;
        }
    }
}

