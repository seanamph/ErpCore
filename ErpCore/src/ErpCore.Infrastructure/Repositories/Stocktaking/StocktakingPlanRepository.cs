using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Stocktaking;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Stocktaking;

/// <summary>
/// 盤點計劃 Repository 實作
/// </summary>
public class StocktakingPlanRepository : BaseRepository, IStocktakingPlanRepository
{
    public StocktakingPlanRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<StocktakingPlan?> GetByIdAsync(string planId)
    {
        const string sql = @"
            SELECT * FROM StocktakingPlans 
            WHERE PlanId = @PlanId";

        return await QueryFirstOrDefaultAsync<StocktakingPlan>(sql, new { PlanId = planId });
    }

    public async Task<IEnumerable<StocktakingPlan>> QueryAsync(StocktakingPlanQuery query)
    {
        var sql = @"
            SELECT * FROM StocktakingPlans 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.PlanId))
        {
            sql += " AND PlanId LIKE @PlanId";
            parameters.Add("PlanId", $"%{query.PlanId}%");
        }

        if (query.PlanDateFrom.HasValue)
        {
            sql += " AND PlanDate >= @PlanDateFrom";
            parameters.Add("PlanDateFrom", query.PlanDateFrom);
        }

        if (query.PlanDateTo.HasValue)
        {
            sql += " AND PlanDate <= @PlanDateTo";
            parameters.Add("PlanDateTo", query.PlanDateTo);
        }

        if (!string.IsNullOrEmpty(query.PlanStatus))
        {
            sql += " AND PlanStatus = @PlanStatus";
            parameters.Add("PlanStatus", query.PlanStatus);
        }

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND SiteId = @SiteId";
            parameters.Add("SiteId", query.SiteId);
        }

        if (!string.IsNullOrEmpty(query.SakeType))
        {
            sql += " AND SakeType = @SakeType";
            parameters.Add("SakeType", query.SakeType);
        }

        // 分頁
        sql += " ORDER BY PlanDate DESC, PlanId DESC";
        sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        return await QueryAsync<StocktakingPlan>(sql, parameters);
    }

    public async Task<int> GetCountAsync(StocktakingPlanQuery query)
    {
        var sql = @"
            SELECT COUNT(*) FROM StocktakingPlans 
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.PlanId))
        {
            sql += " AND PlanId LIKE @PlanId";
            parameters.Add("PlanId", $"%{query.PlanId}%");
        }

        if (query.PlanDateFrom.HasValue)
        {
            sql += " AND PlanDate >= @PlanDateFrom";
            parameters.Add("PlanDateFrom", query.PlanDateFrom);
        }

        if (query.PlanDateTo.HasValue)
        {
            sql += " AND PlanDate <= @PlanDateTo";
            parameters.Add("PlanDateTo", query.PlanDateTo);
        }

        if (!string.IsNullOrEmpty(query.PlanStatus))
        {
            sql += " AND PlanStatus = @PlanStatus";
            parameters.Add("PlanStatus", query.PlanStatus);
        }

        if (!string.IsNullOrEmpty(query.SiteId))
        {
            sql += " AND SiteId = @SiteId";
            parameters.Add("SiteId", query.SiteId);
        }

        if (!string.IsNullOrEmpty(query.SakeType))
        {
            sql += " AND SakeType = @SakeType";
            parameters.Add("SakeType", query.SakeType);
        }

        return await QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<string> CreateAsync(StocktakingPlan entity, List<StocktakingPlanShop> shops)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 插入主檔
            const string insertPlanSql = @"
                INSERT INTO StocktakingPlans (
                    PlanId, PlanDate, StartDate, EndDate, StartTime, EndTime,
                    SakeType, SakeDept, PlanStatus, SiteId,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @PlanId, @PlanDate, @StartDate, @EndDate, @StartTime, @EndTime,
                    @SakeType, @SakeDept, @PlanStatus, @SiteId,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            await connection.ExecuteAsync(insertPlanSql, entity, transaction);

            // 插入店舖檔
            if (shops != null && shops.Any())
            {
                const string insertShopSql = @"
                    INSERT INTO StocktakingPlanShops (
                        PlanId, ShopId, Status, InvStatus,
                        CreatedBy, CreatedAt
                    ) VALUES (
                        @PlanId, @ShopId, @Status, @InvStatus,
                        @CreatedBy, @CreatedAt
                    )";

                foreach (var shop in shops)
                {
                    await connection.ExecuteAsync(insertShopSql, shop, transaction);
                }
            }

            transaction.Commit();
            return entity.PlanId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateAsync(StocktakingPlan entity, List<StocktakingPlanShop> shops)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 更新主檔
            const string updatePlanSql = @"
                UPDATE StocktakingPlans SET
                    PlanDate = @PlanDate,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    SakeType = @SakeType,
                    SakeDept = @SakeDept,
                    PlanStatus = @PlanStatus,
                    SiteId = @SiteId,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PlanId = @PlanId";

            await connection.ExecuteAsync(updatePlanSql, entity, transaction);

            // 刪除舊的店舖檔
            const string deleteShopSql = "DELETE FROM StocktakingPlanShops WHERE PlanId = @PlanId";
            await connection.ExecuteAsync(deleteShopSql, new { PlanId = entity.PlanId }, transaction);

            // 插入新的店舖檔
            if (shops != null && shops.Any())
            {
                const string insertShopSql = @"
                    INSERT INTO StocktakingPlanShops (
                        PlanId, ShopId, Status, InvStatus,
                        CreatedBy, CreatedAt
                    ) VALUES (
                        @PlanId, @ShopId, @Status, @InvStatus,
                        @CreatedBy, @CreatedAt
                    )";

                foreach (var shop in shops)
                {
                    await connection.ExecuteAsync(insertShopSql, shop, transaction);
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task DeleteAsync(string planId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 刪除店舖檔
            const string deleteShopSql = "DELETE FROM StocktakingPlanShops WHERE PlanId = @PlanId";
            await connection.ExecuteAsync(deleteShopSql, new { PlanId = planId }, transaction);

            // 刪除主檔
            const string deletePlanSql = "DELETE FROM StocktakingPlans WHERE PlanId = @PlanId";
            await connection.ExecuteAsync(deletePlanSql, new { PlanId = planId }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateStatusAsync(string planId, string status)
    {
        const string sql = @"
            UPDATE StocktakingPlans 
            SET PlanStatus = @Status, UpdatedAt = GETDATE()
            WHERE PlanId = @PlanId";

        await ExecuteAsync(sql, new { PlanId = planId, Status = status });
    }

    public async Task<string> GeneratePlanIdAsync()
    {
        const string sql = @"
            SELECT TOP 1 PlanId 
            FROM StocktakingPlans 
            WHERE PlanId LIKE 'PLAN' + FORMAT(GETDATE(), 'yyyyMMdd') + '%'
            ORDER BY PlanId DESC";

        var lastId = await QueryFirstOrDefaultAsync<string>(sql);

        if (string.IsNullOrEmpty(lastId))
        {
            return $"PLAN{DateTime.Now:yyyyMMdd}001";
        }

        var sequence = int.Parse(lastId.Substring(12)) + 1;
        return $"PLAN{DateTime.Now:yyyyMMdd}{sequence:D3}";
    }

    public async Task<IEnumerable<StocktakingPlanShop>> GetShopsByPlanIdAsync(string planId)
    {
        const string sql = @"
            SELECT * FROM StocktakingPlanShops 
            WHERE PlanId = @PlanId";

        return await QueryAsync<StocktakingPlanShop>(sql, new { PlanId = planId });
    }

    public async Task<IEnumerable<StocktakingDetail>> GetDetailsByPlanIdAsync(string planId)
    {
        const string sql = @"
            SELECT * FROM StocktakingDetails 
            WHERE PlanId = @PlanId 
            ORDER BY ShopId, GoodsId";

        return await QueryAsync<StocktakingDetail>(sql, new { PlanId = planId });
    }

    public async Task<IEnumerable<StocktakingTemp>> GetTempByPlanIdAsync(string planId)
    {
        const string sql = @"
            SELECT * FROM StocktakingTemp 
            WHERE PlanId = @PlanId 
            ORDER BY ShopId, GoodsId";

        return await QueryAsync<StocktakingTemp>(sql, new { PlanId = planId });
    }

    public async Task CreateDetailAsync(StocktakingDetail detail)
    {
        const string sql = @"
            INSERT INTO StocktakingDetails (
                DetailId, PlanId, ShopId, GoodsId,
                BookQty, PhysicalQty, DiffQty, UnitCost, DiffAmount,
                Kind, ShelfNo, SerialNo, Notes,
                CreatedBy, CreatedAt
            ) VALUES (
                @DetailId, @PlanId, @ShopId, @GoodsId,
                @BookQty, @PhysicalQty, @DiffQty, @UnitCost, @DiffAmount,
                @Kind, @ShelfNo, @SerialNo, @Notes,
                @CreatedBy, @CreatedAt
            )";

        await ExecuteAsync(sql, detail);
    }

    public async Task UpdateDetailAsync(StocktakingDetail detail)
    {
        const string sql = @"
            UPDATE StocktakingDetails SET
                BookQty = @BookQty,
                PhysicalQty = @PhysicalQty,
                DiffQty = @DiffQty,
                UnitCost = @UnitCost,
                DiffAmount = @DiffAmount,
                Kind = @Kind,
                ShelfNo = @ShelfNo,
                SerialNo = @SerialNo,
                Notes = @Notes
            WHERE DetailId = @DetailId";

        await ExecuteAsync(sql, detail);
    }

    public async Task DeleteDetailsByPlanIdAsync(string planId)
    {
        const string sql = "DELETE FROM StocktakingDetails WHERE PlanId = @PlanId";
        await ExecuteAsync(sql, new { PlanId = planId });
    }

    public async Task CreateTempAsync(StocktakingTemp temp)
    {
        const string sql = @"
            INSERT INTO StocktakingTemp (
                PlanId, SPlanId, ShopId, GoodsId,
                Kind, ShelfNo, SerialNo, Qty, IQty, IsAdd,
                HtStatus, Status, BUser, BTime,
                ApprvId, ApprvDate, InvDate, IsUpdate,
                NumNo, HtAuto, IsSuccess, ErrMsg, IsHt, SiteId,
                CreatedBy, CreatedAt
            ) VALUES (
                @PlanId, @SPlanId, @ShopId, @GoodsId,
                @Kind, @ShelfNo, @SerialNo, @Qty, @IQty, @IsAdd,
                @HtStatus, @Status, @BUser, @BTime,
                @ApprvId, @ApprvDate, @InvDate, @IsUpdate,
                @NumNo, @HtAuto, @IsSuccess, @ErrMsg, @IsHt, @SiteId,
                @CreatedBy, @CreatedAt
            )";

        await ExecuteAsync(sql, temp);
    }

    public async Task UpdateTempAsync(StocktakingTemp temp)
    {
        const string sql = @"
            UPDATE StocktakingTemp SET
                SPlanId = @SPlanId,
                Kind = @Kind,
                ShelfNo = @ShelfNo,
                SerialNo = @SerialNo,
                Qty = @Qty,
                IQty = @IQty,
                IsAdd = @IsAdd,
                HtStatus = @HtStatus,
                Status = @Status,
                BUser = @BUser,
                BTime = @BTime,
                ApprvId = @ApprvId,
                ApprvDate = @ApprvDate,
                InvDate = @InvDate,
                IsUpdate = @IsUpdate,
                NumNo = @NumNo,
                HtAuto = @HtAuto,
                IsSuccess = @IsSuccess,
                ErrMsg = @ErrMsg,
                IsHt = @IsHt,
                SiteId = @SiteId
            WHERE TKey = @TKey";

        await ExecuteAsync(sql, temp);
    }

    public async Task DeleteTempByPlanIdAsync(string planId)
    {
        const string sql = "DELETE FROM StocktakingTemp WHERE PlanId = @PlanId";
        await ExecuteAsync(sql, new { PlanId = planId });
    }
}
