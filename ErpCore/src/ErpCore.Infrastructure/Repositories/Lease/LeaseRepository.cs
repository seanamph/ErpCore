using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃 Repository 實作 (SYS8110-SYS8220)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseRepository : BaseRepository, ILeaseRepository
{
    public LeaseRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Lease?> GetByIdAsync(string leaseId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Leases 
                WHERE LeaseId = @LeaseId";

            return await QueryFirstOrDefaultAsync<Lease>(sql, new { LeaseId = leaseId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Lease>> QueryAsync(LeaseQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Leases 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId LIKE @LeaseId";
                parameters.Add("LeaseId", $"%{query.LeaseId}%");
            }

            if (!string.IsNullOrEmpty(query.TenantId))
            {
                sql += " AND TenantId LIKE @TenantId";
                parameters.Add("TenantId", $"%{query.TenantId}%");
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDateFrom.HasValue)
            {
                sql += " AND StartDate >= @StartDateFrom";
                parameters.Add("StartDateFrom", query.StartDateFrom);
            }

            if (query.StartDateTo.HasValue)
            {
                sql += " AND StartDate <= @StartDateTo";
                parameters.Add("StartDateTo", query.StartDateTo);
            }

            if (query.EndDateFrom.HasValue)
            {
                sql += " AND EndDate >= @EndDateFrom";
                parameters.Add("EndDateFrom", query.EndDateFrom);
            }

            if (query.EndDateTo.HasValue)
            {
                sql += " AND EndDate <= @EndDateTo";
                parameters.Add("EndDateTo", query.EndDateTo);
            }

            sql += " ORDER BY LeaseId DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Lease>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Leases 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId LIKE @LeaseId";
                parameters.Add("LeaseId", $"%{query.LeaseId}%");
            }

            if (!string.IsNullOrEmpty(query.TenantId))
            {
                sql += " AND TenantId LIKE @TenantId";
                parameters.Add("TenantId", $"%{query.TenantId}%");
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDateFrom.HasValue)
            {
                sql += " AND StartDate >= @StartDateFrom";
                parameters.Add("StartDateFrom", query.StartDateFrom);
            }

            if (query.StartDateTo.HasValue)
            {
                sql += " AND StartDate <= @StartDateTo";
                parameters.Add("StartDateTo", query.StartDateTo);
            }

            if (query.EndDateFrom.HasValue)
            {
                sql += " AND EndDate >= @EndDateFrom";
                parameters.Add("EndDateFrom", query.EndDateFrom);
            }

            if (query.EndDateTo.HasValue)
            {
                sql += " AND EndDate <= @EndDateTo";
                parameters.Add("EndDateTo", query.EndDateTo);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string leaseId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Leases 
                WHERE LeaseId = @LeaseId";

            var count = await ExecuteScalarAsync<int>(sql, new { LeaseId = leaseId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃是否存在失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<Lease> CreateAsync(Lease lease)
    {
        try
        {
            const string sql = @"
                INSERT INTO Leases (
                    LeaseId, TenantId, TenantName, ShopId, FloorId, LocationId, LeaseDate, StartDate, EndDate,
                    Status, MonthlyRent, TotalRent, Deposit, CurrencyId, PaymentMethod, PaymentDay, Memo,
                    SiteId, OrgId,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @LeaseId, @TenantId, @TenantName, @ShopId, @FloorId, @LocationId, @LeaseDate, @StartDate, @EndDate,
                    @Status, @MonthlyRent, @TotalRent, @Deposit, @CurrencyId, @PaymentMethod, @PaymentDay, @Memo,
                    @SiteId, @OrgId,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                lease.LeaseId,
                lease.TenantId,
                lease.TenantName,
                lease.ShopId,
                lease.FloorId,
                lease.LocationId,
                lease.LeaseDate,
                lease.StartDate,
                lease.EndDate,
                lease.Status,
                lease.MonthlyRent,
                lease.TotalRent,
                lease.Deposit,
                lease.CurrencyId,
                lease.PaymentMethod,
                lease.PaymentDay,
                lease.Memo,
                lease.SiteId,
                lease.OrgId,
                lease.CreatedBy,
                lease.CreatedAt,
                lease.UpdatedBy,
                lease.UpdatedAt
            });

            lease.TKey = tKey;
            return lease;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃失敗: {lease.LeaseId}", ex);
            throw;
        }
    }

    public async Task<Lease> UpdateAsync(Lease lease)
    {
        try
        {
            const string sql = @"
                UPDATE Leases SET
                    TenantId = @TenantId,
                    TenantName = @TenantName,
                    ShopId = @ShopId,
                    FloorId = @FloorId,
                    LocationId = @LocationId,
                    LeaseDate = @LeaseDate,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Status = @Status,
                    MonthlyRent = @MonthlyRent,
                    TotalRent = @TotalRent,
                    Deposit = @Deposit,
                    CurrencyId = @CurrencyId,
                    PaymentMethod = @PaymentMethod,
                    PaymentDay = @PaymentDay,
                    Memo = @Memo,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE LeaseId = @LeaseId";

            await ExecuteAsync(sql, new
            {
                lease.LeaseId,
                lease.TenantId,
                lease.TenantName,
                lease.ShopId,
                lease.FloorId,
                lease.LocationId,
                lease.LeaseDate,
                lease.StartDate,
                lease.EndDate,
                lease.Status,
                lease.MonthlyRent,
                lease.TotalRent,
                lease.Deposit,
                lease.CurrencyId,
                lease.PaymentMethod,
                lease.PaymentDay,
                lease.Memo,
                lease.SiteId,
                lease.OrgId,
                lease.UpdatedBy,
                lease.UpdatedAt
            });

            return lease;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃失敗: {lease.LeaseId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string leaseId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Leases 
                WHERE LeaseId = @LeaseId";

            await ExecuteAsync(sql, new { LeaseId = leaseId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string leaseId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE Leases SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE LeaseId = @LeaseId";

            await ExecuteAsync(sql, new { LeaseId = leaseId, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新租賃狀態失敗: {leaseId}, Status: {status}", ex);
            throw;
        }
    }
}

