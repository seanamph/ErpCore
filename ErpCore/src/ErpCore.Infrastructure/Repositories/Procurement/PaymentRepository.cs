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
public class PaymentRepository : BaseRepository, IPaymentRepository
{
    public PaymentRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Payment?> GetByIdAsync(string paymentId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payments 
                WHERE PaymentId = @PaymentId";

            return await QueryFirstOrDefaultAsync<Payment>(sql, new { PaymentId = paymentId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢付款單失敗: {paymentId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Payment>> QueryAsync(PaymentQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Payments 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PaymentId))
            {
                sql += " AND PaymentId LIKE @PaymentId";
                parameters.Add("PaymentId", $"%{query.PaymentId}%");
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
                sql += " AND SupplierId = @SupplierId";
                parameters.Add("SupplierId", query.SupplierId);
            }

            if (!string.IsNullOrEmpty(query.PaymentType))
            {
                sql += " AND PaymentType = @PaymentType";
                parameters.Add("PaymentType", query.PaymentType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY PaymentDate DESC, PaymentId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Payment>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢付款單列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PaymentQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Payments 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PaymentId))
            {
                sql += " AND PaymentId LIKE @PaymentId";
                parameters.Add("PaymentId", $"%{query.PaymentId}%");
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
                sql += " AND SupplierId = @SupplierId";
                parameters.Add("SupplierId", query.SupplierId);
            }

            if (!string.IsNullOrEmpty(query.PaymentType))
            {
                sql += " AND PaymentType = @PaymentType";
                parameters.Add("PaymentType", query.PaymentType);
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

    public async Task<bool> ExistsAsync(string paymentId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Payments 
                WHERE PaymentId = @PaymentId";

            var count = await ExecuteScalarAsync<int>(sql, new { PaymentId = paymentId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查付款單是否存在失敗: {paymentId}", ex);
            throw;
        }
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        try
        {
            const string sql = @"
                INSERT INTO Payments (
                    PaymentId, PaymentDate, SupplierId, PaymentType, Amount,
                    CurrencyId, ExchangeRate, BankAccountId, CheckNumber, Status, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @PaymentId, @PaymentDate, @SupplierId, @PaymentType, @Amount,
                    @CurrencyId, @ExchangeRate, @BankAccountId, @CheckNumber, @Status, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                payment.PaymentId,
                payment.PaymentDate,
                payment.SupplierId,
                payment.PaymentType,
                payment.Amount,
                payment.CurrencyId,
                payment.ExchangeRate,
                payment.BankAccountId,
                payment.CheckNumber,
                payment.Status,
                payment.Memo,
                payment.CreatedBy,
                payment.CreatedAt,
                payment.UpdatedBy,
                payment.UpdatedAt
            });

            payment.TKey = tKey;
            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增付款單失敗: {payment.PaymentId}", ex);
            throw;
        }
    }

    public async Task<Payment> UpdateAsync(Payment payment)
    {
        try
        {
            const string sql = @"
                UPDATE Payments SET
                    PaymentDate = @PaymentDate,
                    SupplierId = @SupplierId,
                    PaymentType = @PaymentType,
                    Amount = @Amount,
                    CurrencyId = @CurrencyId,
                    ExchangeRate = @ExchangeRate,
                    BankAccountId = @BankAccountId,
                    CheckNumber = @CheckNumber,
                    Status = @Status,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PaymentId = @PaymentId";

            await ExecuteAsync(sql, new
            {
                payment.PaymentId,
                payment.PaymentDate,
                payment.SupplierId,
                payment.PaymentType,
                payment.Amount,
                payment.CurrencyId,
                payment.ExchangeRate,
                payment.BankAccountId,
                payment.CheckNumber,
                payment.Status,
                payment.Memo,
                payment.UpdatedBy,
                payment.UpdatedAt
            });

            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改付款單失敗: {payment.PaymentId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string paymentId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Payments 
                WHERE PaymentId = @PaymentId";

            await ExecuteAsync(sql, new { PaymentId = paymentId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除付款單失敗: {paymentId}", ex);
            throw;
        }
    }
}

