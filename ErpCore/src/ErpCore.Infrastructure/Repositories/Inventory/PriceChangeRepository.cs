using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 變價單 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PriceChangeRepository : BaseRepository, IPriceChangeRepository
{
    public PriceChangeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PriceChangeMaster?> GetByIdAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PriceChangeMasters 
                WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";

            return await QueryFirstOrDefaultAsync<PriceChangeMaster>(sql, new { PriceChangeId = priceChangeId, PriceChangeType = priceChangeType });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PriceChangeMaster>> QueryAsync(PriceChangeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PriceChangeMasters
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PriceChangeId))
            {
                sql += " AND PriceChangeId LIKE @PriceChangeId";
                parameters.Add("PriceChangeId", $"%{query.PriceChangeId}%");
            }

            if (!string.IsNullOrEmpty(query.PriceChangeType))
            {
                sql += " AND PriceChangeType = @PriceChangeType";
                parameters.Add("PriceChangeType", query.PriceChangeType);
            }

            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                sql += " AND SupplierId = @SupplierId";
                parameters.Add("SupplierId", query.SupplierId);
            }

            if (!string.IsNullOrEmpty(query.LogoId))
            {
                sql += " AND LogoId = @LogoId";
                parameters.Add("LogoId", query.LogoId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.ApplyDateFrom.HasValue)
            {
                sql += " AND ApplyDate >= @ApplyDateFrom";
                parameters.Add("ApplyDateFrom", query.ApplyDateFrom.Value);
            }

            if (query.ApplyDateTo.HasValue)
            {
                sql += " AND ApplyDate <= @ApplyDateTo";
                parameters.Add("ApplyDateTo", query.ApplyDateTo.Value);
            }

            if (query.StartDateFrom.HasValue)
            {
                sql += " AND StartDate >= @StartDateFrom";
                parameters.Add("StartDateFrom", query.StartDateFrom.Value);
            }

            if (query.StartDateTo.HasValue)
            {
                sql += " AND StartDate <= @StartDateTo";
                parameters.Add("StartDateTo", query.StartDateTo.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ApplyDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PriceChangeMaster>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM PriceChangeMasters
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.PriceChangeId))
            {
                countSql += " AND PriceChangeId LIKE @PriceChangeId";
                countParameters.Add("PriceChangeId", $"%{query.PriceChangeId}%");
            }
            if (!string.IsNullOrEmpty(query.PriceChangeType))
            {
                countSql += " AND PriceChangeType = @PriceChangeType";
                countParameters.Add("PriceChangeType", query.PriceChangeType);
            }
            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                countSql += " AND SupplierId = @SupplierId";
                countParameters.Add("SupplierId", query.SupplierId);
            }
            if (!string.IsNullOrEmpty(query.LogoId))
            {
                countSql += " AND LogoId = @LogoId";
                countParameters.Add("LogoId", query.LogoId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.ApplyDateFrom.HasValue)
            {
                countSql += " AND ApplyDate >= @ApplyDateFrom";
                countParameters.Add("ApplyDateFrom", query.ApplyDateFrom.Value);
            }
            if (query.ApplyDateTo.HasValue)
            {
                countSql += " AND ApplyDate <= @ApplyDateTo";
                countParameters.Add("ApplyDateTo", query.ApplyDateTo.Value);
            }
            if (query.StartDateFrom.HasValue)
            {
                countSql += " AND StartDate >= @StartDateFrom";
                countParameters.Add("StartDateFrom", query.StartDateFrom.Value);
            }
            if (query.StartDateTo.HasValue)
            {
                countSql += " AND StartDate <= @StartDateTo";
                countParameters.Add("StartDateTo", query.StartDateTo.Value);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<PriceChangeMaster>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢變價單列表失敗", ex);
            throw;
        }
    }

    public async Task<List<PriceChangeDetail>> GetDetailsAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PriceChangeDetails 
                WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType
                ORDER BY LineNum";

            var items = await QueryAsync<PriceChangeDetail>(sql, new { PriceChangeId = priceChangeId, PriceChangeType = priceChangeType });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢變價單明細失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PriceChangeMasters 
                WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";

            var count = await ExecuteScalarAsync<int>(sql, new { PriceChangeId = priceChangeId, PriceChangeType = priceChangeType });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查變價單是否存在失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task<PriceChangeMaster> CreateAsync(PriceChangeMaster priceChange, List<PriceChangeDetail> details)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 新增主檔
                const string masterSql = @"
                    INSERT INTO PriceChangeMasters (
                        PriceChangeId, PriceChangeType, SupplierId, LogoId, ApplyEmpId, ApplyOrgId,
                        ApplyDate, StartDate, ApproveEmpId, ApproveDate, ConfirmEmpId, ConfirmDate,
                        Status, TotalAmount, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt,
                        CreatedPriority, CreatedGroup
                    ) VALUES (
                        @PriceChangeId, @PriceChangeType, @SupplierId, @LogoId, @ApplyEmpId, @ApplyOrgId,
                        @ApplyDate, @StartDate, @ApproveEmpId, @ApproveDate, @ConfirmEmpId, @ConfirmDate,
                        @Status, @TotalAmount, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt,
                        @CreatedPriority, @CreatedGroup
                    )";

                priceChange.CreatedAt = DateTime.Now;
                priceChange.UpdatedAt = DateTime.Now;

                await connection.ExecuteAsync(masterSql, priceChange, transaction);

                // 新增明細
                if (details != null && details.Count > 0)
                {
                    const string detailSql = @"
                        INSERT INTO PriceChangeDetails (
                            PriceChangeId, PriceChangeType, LineNum, GoodsId, BeforePrice, AfterPrice,
                            ChangeQty, Notes, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup
                        ) VALUES (
                            @PriceChangeId, @PriceChangeType, @LineNum, @GoodsId, @BeforePrice, @AfterPrice,
                            @ChangeQty, @Notes, @CreatedBy, @CreatedAt, @CreatedPriority, @CreatedGroup
                        )";

                    foreach (var detail in details)
                    {
                        detail.CreatedAt = DateTime.Now;
                        await connection.ExecuteAsync(detailSql, detail, transaction);
                    }
                }

                transaction.Commit();
                return priceChange;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增變價單失敗: {priceChange.PriceChangeId}/{priceChange.PriceChangeType}", ex);
            throw;
        }
    }

    public async Task<PriceChangeMaster> UpdateAsync(PriceChangeMaster priceChange, List<PriceChangeDetail> details)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 更新主檔
                const string masterSql = @"
                    UPDATE PriceChangeMasters SET
                        SupplierId = @SupplierId,
                        LogoId = @LogoId,
                        ApplyOrgId = @ApplyOrgId,
                        ApplyDate = @ApplyDate,
                        StartDate = @StartDate,
                        TotalAmount = @TotalAmount,
                        Notes = @Notes,
                        UpdatedBy = @UpdatedBy,
                        UpdatedAt = @UpdatedAt
                    WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";

                priceChange.UpdatedAt = DateTime.Now;

                await connection.ExecuteAsync(masterSql, priceChange, transaction);

                // 刪除舊明細
                const string deleteDetailSql = @"
                    DELETE FROM PriceChangeDetails 
                    WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";

                await connection.ExecuteAsync(deleteDetailSql, new { priceChange.PriceChangeId, priceChange.PriceChangeType }, transaction);

                // 新增新明細
                if (details != null && details.Count > 0)
                {
                    const string detailSql = @"
                        INSERT INTO PriceChangeDetails (
                            PriceChangeId, PriceChangeType, LineNum, GoodsId, BeforePrice, AfterPrice,
                            ChangeQty, Notes, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup
                        ) VALUES (
                            @PriceChangeId, @PriceChangeType, @LineNum, @GoodsId, @BeforePrice, @AfterPrice,
                            @ChangeQty, @Notes, @CreatedBy, @CreatedAt, @CreatedPriority, @CreatedGroup
                        )";

                    foreach (var detail in details)
                    {
                        detail.CreatedAt = DateTime.Now;
                        await connection.ExecuteAsync(detailSql, detail, transaction);
                    }
                }

                transaction.Commit();
                return priceChange;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改變價單失敗: {priceChange.PriceChangeId}/{priceChange.PriceChangeType}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            const string sql = @"
                DELETE FROM PriceChangeMasters 
                WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";

            await ExecuteAsync(sql, new { PriceChangeId = priceChangeId, PriceChangeType = priceChangeType });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string priceChangeId, string priceChangeType, string status, string? empId, DateTime? date)
    {
        try
        {
            var sql = "";
            var parameters = new DynamicParameters();

            if (status == "2") // 已審核
            {
                sql = @"
                    UPDATE PriceChangeMasters SET
                        Status = @Status,
                        ApproveEmpId = @EmpId,
                        ApproveDate = @Date,
                        UpdatedAt = @UpdatedAt
                    WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";
                parameters.Add("EmpId", empId);
                parameters.Add("Date", date);
            }
            else if (status == "10") // 已確認
            {
                sql = @"
                    UPDATE PriceChangeMasters SET
                        Status = @Status,
                        ConfirmEmpId = @EmpId,
                        ConfirmDate = @Date,
                        UpdatedAt = @UpdatedAt
                    WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";
                parameters.Add("EmpId", empId);
                parameters.Add("Date", date);
            }
            else if (status == "9") // 已作廢
            {
                sql = @"
                    UPDATE PriceChangeMasters SET
                        Status = @Status,
                        UpdatedAt = @UpdatedAt
                    WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";
            }

            parameters.Add("Status", status);
            parameters.Add("PriceChangeId", priceChangeId);
            parameters.Add("PriceChangeType", priceChangeType);
            parameters.Add("UpdatedAt", DateTime.Now);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新變價單狀態失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task UpdateProductPurchasePriceAsync(string goodsId, decimal price, string updatedBy)
    {
        try
        {
            // 更新商品進價（商品表名為 Products，進價欄位為 Lprc）
            const string sql = @"
                UPDATE Products SET
                    Lprc = @Price,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE GoodsId = @GoodsId";

            var parameters = new DynamicParameters();
            parameters.Add("Price", price);
            parameters.Add("GoodsId", goodsId);
            parameters.Add("UpdatedBy", updatedBy);
            parameters.Add("UpdatedAt", DateTime.Now);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商品進價失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task UpdateProductSalePriceAsync(string goodsId, decimal price, string updatedBy)
    {
        try
        {
            // 更新商品售價（商品表名為 Products，售價欄位為 Mprc）
            // 注意：根據系統設計，售價可能存儲在 Mprc（中價）欄位中
            const string sql = @"
                UPDATE Products SET
                    Mprc = @Price,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE GoodsId = @GoodsId";

            var parameters = new DynamicParameters();
            parameters.Add("Price", price);
            parameters.Add("GoodsId", goodsId);
            parameters.Add("UpdatedBy", updatedBy);
            parameters.Add("UpdatedAt", DateTime.Now);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商品售價失敗: {goodsId}", ex);
            throw;
        }
    }
}

