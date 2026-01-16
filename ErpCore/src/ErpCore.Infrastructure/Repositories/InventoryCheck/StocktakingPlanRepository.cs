using global::System.Data;
using Dapper;
using ErpCore.Domain.Entities.InventoryCheck;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InventoryCheck;

/// <summary>
/// 盤點計劃 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class StocktakingPlanRepository : BaseRepository, IStocktakingPlanRepository
{
    public StocktakingPlanRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<StocktakingPlan?> GetByIdAsync(string planId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM StocktakingPlans 
                WHERE PlanId = @PlanId";

            return await QueryFirstOrDefaultAsync<StocktakingPlan>(sql, new { PlanId = planId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢盤點計劃失敗: {planId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<StocktakingPlanShop>> GetShopsByPlanIdAsync(string planId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM StocktakingPlanShops 
                WHERE PlanId = @PlanId 
                ORDER BY ShopId";

            return await QueryAsync<StocktakingPlanShop>(sql, new { PlanId = planId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢盤點計劃店舖失敗: {planId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<StocktakingDetail>> GetDetailsByPlanIdAsync(string planId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM StocktakingDetails 
                WHERE PlanId = @PlanId 
                ORDER BY ShopId, GoodsId";

            return await QueryAsync<StocktakingDetail>(sql, new { PlanId = planId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢盤點明細失敗: {planId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<StocktakingTemp>> GetTempByPlanIdAsync(string planId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM StocktakingTemp 
                WHERE PlanId = @PlanId 
                ORDER BY ShopId, GoodsId";

            return await QueryAsync<StocktakingTemp>(sql, new { PlanId = planId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢盤點暫存資料失敗: {planId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<StocktakingPlan>> QueryAsync(StocktakingPlanQuery query)
    {
        try
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

            if (!string.IsNullOrEmpty(query.SakeType))
            {
                sql += " AND SakeType = @SakeType";
                parameters.Add("SakeType", query.SakeType);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND EXISTS (SELECT 1 FROM StocktakingPlanShops WHERE PlanId = StocktakingPlans.PlanId AND ShopId = @ShopId)";
                parameters.Add("ShopId", query.ShopId);
            }

            sql += " ORDER BY PlanDate DESC, PlanId DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<StocktakingPlan>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢盤點計劃列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(StocktakingPlanQuery query)
    {
        try
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

            if (!string.IsNullOrEmpty(query.SakeType))
            {
                sql += " AND SakeType = @SakeType";
                parameters.Add("SakeType", query.SakeType);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND EXISTS (SELECT 1 FROM StocktakingPlanShops WHERE PlanId = StocktakingPlans.PlanId AND ShopId = @ShopId)";
                parameters.Add("ShopId", query.ShopId);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢盤點計劃數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(StocktakingPlan entity, List<StocktakingPlanShop> shops)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string sqlPlan = @"
                    INSERT INTO StocktakingPlans (
                        PlanId, PlanDate, StartDate, EndDate, StartTime, EndTime,
                        SakeType, SakeDept, PlanStatus, SiteId,
                        CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                    ) VALUES (
                        @PlanId, @PlanDate, @StartDate, @EndDate, @StartTime, @EndTime,
                        @SakeType, @SakeDept, @PlanStatus, @SiteId,
                        @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                    )";

                await connection.ExecuteAsync(sqlPlan, entity, transaction);

                if (shops.Any())
                {
                    const string sqlShop = @"
                        INSERT INTO StocktakingPlanShops (
                            PlanId, ShopId, Status, InvStatus,
                            CreatedBy, CreatedAt
                        ) VALUES (
                            @PlanId, @ShopId, @Status, @InvStatus,
                            @CreatedBy, @CreatedAt
                        )";

                    await connection.ExecuteAsync(sqlShop, shops, transaction);
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
        catch (Exception ex)
        {
            _logger.LogError($"新增盤點計劃失敗: {entity.PlanId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(StocktakingPlan entity, List<StocktakingPlanShop> shops)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string sqlPlan = @"
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

                await connection.ExecuteAsync(sqlPlan, entity, transaction);

                // 刪除舊的店舖資料
                const string sqlDeleteShop = @"
                    DELETE FROM StocktakingPlanShops 
                    WHERE PlanId = @PlanId";

                await connection.ExecuteAsync(sqlDeleteShop, new { PlanId = entity.PlanId }, transaction);

                // 新增新的店舖資料
                if (shops.Any())
                {
                    const string sqlShop = @"
                        INSERT INTO StocktakingPlanShops (
                            PlanId, ShopId, Status, InvStatus,
                            CreatedBy, CreatedAt
                        ) VALUES (
                            @PlanId, @ShopId, @Status, @InvStatus,
                            @CreatedBy, @CreatedAt
                        )";

                    await connection.ExecuteAsync(sqlShop, shops, transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改盤點計劃失敗: {entity.PlanId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string planId)
    {
        try
        {
            const string sql = @"
                DELETE FROM StocktakingPlans 
                WHERE PlanId = @PlanId";

            await ExecuteAsync(sql, new { PlanId = planId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除盤點計劃失敗: {planId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string planId, string status, IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                UPDATE StocktakingPlans SET
                    PlanStatus = @Status,
                    UpdatedAt = GETDATE()
                WHERE PlanId = @PlanId";

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, new { PlanId = planId, Status = status }, transaction);
            }
            else
            {
                await ExecuteAsync(sql, new { PlanId = planId, Status = status });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新盤點計劃狀態失敗: {planId}", ex);
            throw;
        }
    }

    public async Task<string> GeneratePlanIdAsync()
    {
        try
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

            var number = int.Parse(lastId.Substring(12)) + 1;
            return $"PLAN{DateTime.Now:yyyyMMdd}{number:D3}";
        }
        catch (Exception ex)
        {
            _logger.LogError("產生盤點計劃單號失敗", ex);
            throw;
        }
    }

    public async Task BulkInsertTempAsync(List<StocktakingTemp> tempList, IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                INSERT INTO StocktakingTemp (
                    PlanId, SPlanId, ShopId, GoodsId, Kind, ShelfNo, SerialNo,
                    Qty, IQty, IsAdd, HtStatus, Status, BUser, BTime,
                    ApprvId, ApprvDate, InvDate, IsUpdate, NumNo, HtAuto,
                    IsSuccess, ErrMsg, IsHt, SiteId, CreatedBy, CreatedAt
                ) VALUES (
                    @PlanId, @SPlanId, @ShopId, @GoodsId, @Kind, @ShelfNo, @SerialNo,
                    @Qty, @IQty, @IsAdd, @HtStatus, @Status, @BUser, @BTime,
                    @ApprvId, @ApprvDate, @InvDate, @IsUpdate, @NumNo, @HtAuto,
                    @IsSuccess, @ErrMsg, @IsHt, @SiteId, @CreatedBy, @CreatedAt
                )";

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, tempList, transaction);
            }
            else
            {
                await ExecuteAsync(sql, tempList);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增盤點暫存資料失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<StocktakingDetail>> CalculateDiffAsync(string planId, string shopId)
    {
        try
        {
            // 計算差異：比較帳面數量與實盤數量
            const string sql = @"
                SELECT 
                    NEWID() AS DetailId,
                    @PlanId AS PlanId,
                    @ShopId AS ShopId,
                    t.GoodsId,
                    ISNULL(s.StockQty, 0) AS BookQty,
                    ISNULL(t.Qty, 0) + ISNULL(t.IQty, 0) AS PhysicalQty,
                    (ISNULL(t.Qty, 0) + ISNULL(t.IQty, 0)) - ISNULL(s.StockQty, 0) AS DiffQty,
                    p.UnitCost,
                    ((ISNULL(t.Qty, 0) + ISNULL(t.IQty, 0)) - ISNULL(s.StockQty, 0)) * ISNULL(p.UnitCost, 0) AS DiffAmount,
                    t.Kind,
                    t.ShelfNo,
                    t.SerialNo,
                    NULL AS Notes,
                    @CreatedBy AS CreatedBy,
                    GETDATE() AS CreatedAt
                FROM StocktakingTemp t
                LEFT JOIN Products p ON t.GoodsId = p.GoodsId
                LEFT JOIN (
                    SELECT ShopId, GoodsId, SUM(StockQty) AS StockQty
                    FROM Stocks
                    WHERE ShopId = @ShopId
                    GROUP BY ShopId, GoodsId
                ) s ON t.GoodsId = s.GoodsId
                WHERE t.PlanId = @PlanId 
                    AND t.ShopId = @ShopId
                    AND t.HtStatus = '1'";

            return await QueryAsync<StocktakingDetail>(sql, new { PlanId = planId, ShopId = shopId, CreatedBy = "SYSTEM" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"計算盤點差異失敗: {planId}, {shopId}", ex);
            throw;
        }
    }
}

