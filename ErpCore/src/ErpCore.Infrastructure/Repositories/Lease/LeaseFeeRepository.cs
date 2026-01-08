using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 費用主檔 Repository 實作 (SYSE310-SYSE430)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseFeeRepository : BaseRepository, ILeaseFeeRepository
{
    public LeaseFeeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LeaseFee?> GetByIdAsync(string feeId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseFees 
                WHERE FeeId = @FeeId";

            return await QueryFirstOrDefaultAsync<LeaseFee>(sql, new { FeeId = feeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢費用失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseFee>> GetByLeaseIdAsync(string leaseId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseFees 
                WHERE LeaseId = @LeaseId
                ORDER BY FeeDate DESC, FeeId";

            return await QueryAsync<LeaseFee>(sql, new { LeaseId = leaseId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號查詢費用失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseFee>> QueryAsync(LeaseFeeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LeaseFees 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FeeId))
            {
                sql += " AND FeeId LIKE @FeeId";
                parameters.Add("FeeId", $"%{query.FeeId}%");
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId LIKE @LeaseId";
                parameters.Add("LeaseId", $"%{query.LeaseId}%");
            }

            if (!string.IsNullOrEmpty(query.FeeType))
            {
                sql += " AND FeeType = @FeeType";
                parameters.Add("FeeType", query.FeeType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.FeeDateFrom.HasValue)
            {
                sql += " AND FeeDate >= @FeeDateFrom";
                parameters.Add("FeeDateFrom", query.FeeDateFrom);
            }

            if (query.FeeDateTo.HasValue)
            {
                sql += " AND FeeDate <= @FeeDateTo";
                parameters.Add("FeeDateTo", query.FeeDateTo);
            }

            sql += " ORDER BY FeeDate DESC, FeeId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LeaseFee>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢費用列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseFeeQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LeaseFees 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FeeId))
            {
                sql += " AND FeeId LIKE @FeeId";
                parameters.Add("FeeId", $"%{query.FeeId}%");
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId LIKE @LeaseId";
                parameters.Add("LeaseId", $"%{query.LeaseId}%");
            }

            if (!string.IsNullOrEmpty(query.FeeType))
            {
                sql += " AND FeeType = @FeeType";
                parameters.Add("FeeType", query.FeeType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.FeeDateFrom.HasValue)
            {
                sql += " AND FeeDate >= @FeeDateFrom";
                parameters.Add("FeeDateFrom", query.FeeDateFrom);
            }

            if (query.FeeDateTo.HasValue)
            {
                sql += " AND FeeDate <= @FeeDateTo";
                parameters.Add("FeeDateTo", query.FeeDateTo);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢費用數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string feeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM LeaseFees 
                WHERE FeeId = @FeeId";

            var count = await ExecuteScalarAsync<int>(sql, new { FeeId = feeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查費用是否存在失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task<LeaseFee> CreateAsync(LeaseFee fee)
    {
        try
        {
            const string sql = @"
                INSERT INTO LeaseFees (
                    FeeId, LeaseId, FeeType, FeeItemId, FeeItemName, FeeAmount, FeeDate, DueDate,
                    PaidDate, PaidAmount, Status, CurrencyId, ExchangeRate, TaxRate, TaxAmount, TotalAmount,
                    Memo, SiteId, OrgId,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @FeeId, @LeaseId, @FeeType, @FeeItemId, @FeeItemName, @FeeAmount, @FeeDate, @DueDate,
                    @PaidDate, @PaidAmount, @Status, @CurrencyId, @ExchangeRate, @TaxRate, @TaxAmount, @TotalAmount,
                    @Memo, @SiteId, @OrgId,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                fee.FeeId,
                fee.LeaseId,
                fee.FeeType,
                fee.FeeItemId,
                fee.FeeItemName,
                fee.FeeAmount,
                fee.FeeDate,
                fee.DueDate,
                fee.PaidDate,
                fee.PaidAmount,
                fee.Status,
                fee.CurrencyId,
                fee.ExchangeRate,
                fee.TaxRate,
                fee.TaxAmount,
                fee.TotalAmount,
                fee.Memo,
                fee.SiteId,
                fee.OrgId,
                fee.CreatedBy,
                fee.CreatedAt,
                fee.UpdatedBy,
                fee.UpdatedAt
            });

            fee.TKey = tKey;
            return fee;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增費用失敗: {fee.FeeId}", ex);
            throw;
        }
    }

    public async Task<LeaseFee> UpdateAsync(LeaseFee fee)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseFees SET
                    FeeType = @FeeType,
                    FeeItemId = @FeeItemId,
                    FeeItemName = @FeeItemName,
                    FeeAmount = @FeeAmount,
                    FeeDate = @FeeDate,
                    DueDate = @DueDate,
                    PaidDate = @PaidDate,
                    PaidAmount = @PaidAmount,
                    Status = @Status,
                    CurrencyId = @CurrencyId,
                    ExchangeRate = @ExchangeRate,
                    TaxRate = @TaxRate,
                    TaxAmount = @TaxAmount,
                    TotalAmount = @TotalAmount,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE FeeId = @FeeId";

            await ExecuteAsync(sql, new
            {
                fee.FeeId,
                fee.FeeType,
                fee.FeeItemId,
                fee.FeeItemName,
                fee.FeeAmount,
                fee.FeeDate,
                fee.DueDate,
                fee.PaidDate,
                fee.PaidAmount,
                fee.Status,
                fee.CurrencyId,
                fee.ExchangeRate,
                fee.TaxRate,
                fee.TaxAmount,
                fee.TotalAmount,
                fee.Memo,
                fee.UpdatedBy,
                fee.UpdatedAt
            });

            return fee;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改費用失敗: {fee.FeeId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string feeId)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseFees 
                WHERE FeeId = @FeeId";

            await ExecuteAsync(sql, new { FeeId = feeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除費用失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string feeId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseFees SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE FeeId = @FeeId";

            await ExecuteAsync(sql, new { FeeId = feeId, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新費用狀態失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task UpdatePaidAmountAsync(string feeId, decimal paidAmount, DateTime? paidDate)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseFees SET
                    PaidAmount = @PaidAmount,
                    PaidDate = @PaidDate,
                    Status = CASE 
                        WHEN @PaidAmount >= FeeAmount THEN 'F'
                        WHEN @PaidAmount > 0 THEN 'P'
                        ELSE 'P'
                    END,
                    UpdatedAt = GETDATE()
                WHERE FeeId = @FeeId";

            await ExecuteAsync(sql, new { FeeId = feeId, PaidAmount = paidAmount, PaidDate = paidDate });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新已繳金額失敗: {feeId}", ex);
            throw;
        }
    }
}

