using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 付款單 Repository 實作 (SYSP271-SYSP2B0)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PaymentVoucherRepository : BaseRepository, IPaymentVoucherRepository
{
    public PaymentVoucherRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PaymentVoucher?> GetByIdAsync(string paymentNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PaymentVouchers 
                WHERE PaymentNo = @PaymentNo";

            return await QueryFirstOrDefaultAsync<PaymentVoucher>(sql, new { PaymentNo = paymentNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢付款單失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PaymentVoucher>> QueryAsync(PaymentVoucherQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PaymentVouchers 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PaymentNo))
            {
                sql += " AND PaymentNo LIKE @PaymentNo";
                parameters.Add("PaymentNo", $"%{query.PaymentNo}%");
            }

            if (query.PaymentDateFrom.HasValue)
            {
                sql += " AND PaymentDate >= @PaymentDateFrom";
                parameters.Add("PaymentDateFrom", query.PaymentDateFrom.Value);
            }

            if (query.PaymentDateTo.HasValue)
            {
                sql += " AND PaymentDate <= @PaymentDateTo";
                parameters.Add("PaymentDateTo", query.PaymentDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                sql += " AND SupplierId LIKE @SupplierId";
                parameters.Add("SupplierId", $"%{query.SupplierId}%");
            }

            if (!string.IsNullOrEmpty(query.PaymentMethod))
            {
                sql += " AND PaymentMethod = @PaymentMethod";
                parameters.Add("PaymentMethod", query.PaymentMethod);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY PaymentDate DESC, PaymentNo";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<PaymentVoucher>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢付款單列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PaymentVoucherQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM PaymentVouchers 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PaymentNo))
            {
                sql += " AND PaymentNo LIKE @PaymentNo";
                parameters.Add("PaymentNo", $"%{query.PaymentNo}%");
            }

            if (query.PaymentDateFrom.HasValue)
            {
                sql += " AND PaymentDate >= @PaymentDateFrom";
                parameters.Add("PaymentDateFrom", query.PaymentDateFrom.Value);
            }

            if (query.PaymentDateTo.HasValue)
            {
                sql += " AND PaymentDate <= @PaymentDateTo";
                parameters.Add("PaymentDateTo", query.PaymentDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                sql += " AND SupplierId LIKE @SupplierId";
                parameters.Add("SupplierId", $"%{query.SupplierId}%");
            }

            if (!string.IsNullOrEmpty(query.PaymentMethod))
            {
                sql += " AND PaymentMethod = @PaymentMethod";
                parameters.Add("PaymentMethod", query.PaymentMethod);
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
            _logger.LogError("查詢付款單數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string paymentNo)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PaymentVouchers 
                WHERE PaymentNo = @PaymentNo";

            var count = await ExecuteScalarAsync<int>(sql, new { PaymentNo = paymentNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查付款單是否存在失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task<PaymentVoucher> CreateAsync(PaymentVoucher paymentVoucher)
    {
        try
        {
            const string sql = @"
                INSERT INTO PaymentVouchers (
                    PaymentNo, PaymentDate, SupplierId, PaymentAmount, PaymentMethod,
                    BankAccount, Status, Verifier, VerifyDate, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @PaymentNo, @PaymentDate, @SupplierId, @PaymentAmount, @PaymentMethod,
                    @BankAccount, @Status, @Verifier, @VerifyDate, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                paymentVoucher.PaymentNo,
                paymentVoucher.PaymentDate,
                paymentVoucher.SupplierId,
                paymentVoucher.PaymentAmount,
                paymentVoucher.PaymentMethod,
                paymentVoucher.BankAccount,
                paymentVoucher.Status,
                paymentVoucher.Verifier,
                paymentVoucher.VerifyDate,
                paymentVoucher.Notes,
                paymentVoucher.CreatedBy,
                paymentVoucher.CreatedAt,
                paymentVoucher.UpdatedBy,
                paymentVoucher.UpdatedAt
            });

            paymentVoucher.TKey = tKey;
            return paymentVoucher;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增付款單失敗: {paymentVoucher.PaymentNo}", ex);
            throw;
        }
    }

    public async Task<PaymentVoucher> UpdateAsync(PaymentVoucher paymentVoucher)
    {
        try
        {
            const string sql = @"
                UPDATE PaymentVouchers SET
                    PaymentDate = @PaymentDate,
                    SupplierId = @SupplierId,
                    PaymentAmount = @PaymentAmount,
                    PaymentMethod = @PaymentMethod,
                    BankAccount = @BankAccount,
                    Status = @Status,
                    Verifier = @Verifier,
                    VerifyDate = @VerifyDate,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PaymentNo = @PaymentNo";

            await ExecuteAsync(sql, new
            {
                paymentVoucher.PaymentNo,
                paymentVoucher.PaymentDate,
                paymentVoucher.SupplierId,
                paymentVoucher.PaymentAmount,
                paymentVoucher.PaymentMethod,
                paymentVoucher.BankAccount,
                paymentVoucher.Status,
                paymentVoucher.Verifier,
                paymentVoucher.VerifyDate,
                paymentVoucher.Notes,
                paymentVoucher.UpdatedBy,
                paymentVoucher.UpdatedAt
            });

            return paymentVoucher;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改付款單失敗: {paymentVoucher.PaymentNo}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string paymentNo)
    {
        try
        {
            const string sql = @"
                DELETE FROM PaymentVouchers 
                WHERE PaymentNo = @PaymentNo";

            await ExecuteAsync(sql, new { PaymentNo = paymentNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除付款單失敗: {paymentNo}", ex);
            throw;
        }
    }
}
