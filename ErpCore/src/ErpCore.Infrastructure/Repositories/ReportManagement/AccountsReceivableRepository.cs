using System.Data;
using Dapper;
using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.ReportManagement;

/// <summary>
/// 應收帳款 Repository 實作 (SYSR210-SYSR240)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class AccountsReceivableRepository : BaseRepository, IAccountsReceivableRepository
{
    public AccountsReceivableRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<AccountsReceivable?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM AccountsReceivable 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<AccountsReceivable>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivable>> GetAllAsync()
    {
        try
        {
            const string sql = @"SELECT * FROM AccountsReceivable ORDER BY ReceiptDate DESC, TKey DESC";

            return await QueryAsync<AccountsReceivable>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢應收帳款列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivable>> GetByReceiptDateRangeAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var sql = @"SELECT * FROM AccountsReceivable WHERE 1=1";
            var parameters = new DynamicParameters();

            if (startDate.HasValue)
            {
                sql += " AND ReceiptDate >= @StartDate";
                parameters.Add("StartDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                sql += " AND ReceiptDate <= @EndDate";
                parameters.Add("EndDate", endDate.Value);
            }

            sql += " ORDER BY ReceiptDate DESC, TKey DESC";

            return await QueryAsync<AccountsReceivable>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款列表失敗: StartDate={startDate}, EndDate={endDate}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivable>> GetByVoucherNoAsync(string voucherNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM AccountsReceivable 
                WHERE VoucherNo = @VoucherNo 
                ORDER BY ReceiptDate DESC";

            return await QueryAsync<AccountsReceivable>(sql, new { VoucherNo = voucherNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款列表失敗: VoucherNo={voucherNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivable>> GetByReceiptNoAsync(string receiptNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM AccountsReceivable 
                WHERE ReceiptNo = @ReceiptNo 
                ORDER BY ReceiptDate DESC";

            return await QueryAsync<AccountsReceivable>(sql, new { ReceiptNo = receiptNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款列表失敗: ReceiptNo={receiptNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivable>> GetByObjectIdAsync(string objectId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM AccountsReceivable 
                WHERE ObjectId = @ObjectId 
                ORDER BY ReceiptDate DESC";

            return await QueryAsync<AccountsReceivable>(sql, new { ObjectId = objectId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款列表失敗: ObjectId={objectId}", ex);
            throw;
        }
    }

    public async Task<AccountsReceivable> CreateAsync(AccountsReceivable entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO AccountsReceivable (
                    VoucherM_TKey, ObjectId, AcctKey, ReceiptDate, ReceiptAmount, 
                    AritemId, ReceiptNo, VoucherNo, VoucherStatus, ShopId, SiteId, OrgId, 
                    CurrencyId, ExchangeRate, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @VoucherM_TKey, @ObjectId, @AcctKey, @ReceiptDate, @ReceiptAmount, 
                    @AritemId, @ReceiptNo, @VoucherNo, @VoucherStatus, @ShopId, @SiteId, @OrgId, 
                    @CurrencyId, @ExchangeRate, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.VoucherM_TKey,
                entity.ObjectId,
                entity.AcctKey,
                entity.ReceiptDate,
                entity.ReceiptAmount,
                entity.AritemId,
                entity.ReceiptNo,
                entity.VoucherNo,
                entity.VoucherStatus,
                entity.ShopId,
                entity.SiteId,
                entity.OrgId,
                entity.CurrencyId,
                entity.ExchangeRate,
                entity.Notes,
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
            _logger.LogError("新增應收帳款失敗", ex);
            throw;
        }
    }

    public async Task<AccountsReceivable> UpdateAsync(AccountsReceivable entity)
    {
        try
        {
            const string sql = @"
                UPDATE AccountsReceivable 
                SET VoucherM_TKey = @VoucherM_TKey,
                    ObjectId = @ObjectId,
                    AcctKey = @AcctKey,
                    ReceiptDate = @ReceiptDate,
                    ReceiptAmount = @ReceiptAmount,
                    AritemId = @AritemId,
                    ReceiptNo = @ReceiptNo,
                    VoucherNo = @VoucherNo,
                    VoucherStatus = @VoucherStatus,
                    ShopId = @ShopId,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    CurrencyId = @CurrencyId,
                    ExchangeRate = @ExchangeRate,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.VoucherM_TKey,
                entity.ObjectId,
                entity.AcctKey,
                entity.ReceiptDate,
                entity.ReceiptAmount,
                entity.AritemId,
                entity.ReceiptNo,
                entity.VoucherNo,
                entity.VoucherStatus,
                entity.ShopId,
                entity.SiteId,
                entity.OrgId,
                entity.CurrencyId,
                entity.ExchangeRate,
                entity.Notes,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新應收帳款失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM AccountsReceivable WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除應收帳款失敗: {tKey}", ex);
            throw;
        }
    }
}

