using ErpCore.Domain.Entities.Certificate;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Certificate;

/// <summary>
/// 憑證明細 Repository 實作 (SYSK110-SYSK150)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class VoucherDetailRepository : BaseRepository, IVoucherDetailRepository
{
    public VoucherDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<IEnumerable<VoucherDetail>> GetByVoucherIdAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM VoucherDetails 
                WHERE VoucherId = @VoucherId
                ORDER BY LineNum";

            return await QueryAsync<VoucherDetail>(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢憑證明細失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<VoucherDetail?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM VoucherDetails 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<VoucherDetail>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢憑證明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<VoucherDetail> CreateAsync(VoucherDetail entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO VoucherDetails (VoucherId, LineNum, AccountId, DebitAmount, CreditAmount, Description, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@VoucherId, @LineNum, @AccountId, @DebitAmount, @CreditAmount, @Description, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.VoucherId,
                entity.LineNum,
                entity.AccountId,
                entity.DebitAmount,
                entity.CreditAmount,
                entity.Description,
                entity.Memo,
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
            _logger.LogError("新增憑證明細失敗", ex);
            throw;
        }
    }

    public async Task<VoucherDetail> UpdateAsync(VoucherDetail entity)
    {
        try
        {
            const string sql = @"
                UPDATE VoucherDetails 
                SET LineNum = @LineNum,
                    AccountId = @AccountId,
                    DebitAmount = @DebitAmount,
                    CreditAmount = @CreditAmount,
                    Description = @Description,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.LineNum,
                entity.AccountId,
                entity.DebitAmount,
                entity.CreditAmount,
                entity.Description,
                entity.Memo,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新憑證明細失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM VoucherDetails WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除憑證明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteByVoucherIdAsync(string voucherId)
    {
        try
        {
            const string sql = @"DELETE FROM VoucherDetails WHERE VoucherId = @VoucherId";

            await ExecuteAsync(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除憑證明細失敗: {voucherId}", ex);
            throw;
        }
    }
}

