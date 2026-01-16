using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 商品永久變價 Repository 實作
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

    public async Task<PriceChangeMaster> CreateAsync(PriceChangeMaster priceChange)
    {
        try
        {
            const string sql = @"
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

            await ExecuteAsync(sql, priceChange);
            return priceChange;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增變價單失敗: {priceChange.PriceChangeId}/{priceChange.PriceChangeType}", ex);
            throw;
        }
    }

    public async Task<PriceChangeDetail> CreateDetailAsync(PriceChangeDetail detail)
    {
        try
        {
            const string sql = @"
                INSERT INTO PriceChangeDetails (
                    PriceChangeId, PriceChangeType, LineNum, GoodsId, BeforePrice, AfterPrice,
                    ChangeQty, Notes, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @PriceChangeId, @PriceChangeType, @LineNum, @GoodsId, @BeforePrice, @AfterPrice,
                    @ChangeQty, @Notes, @CreatedBy, @CreatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            detail.CreatedAt = DateTime.Now;

            var id = await ExecuteScalarAsync<long>(sql, detail);
            detail.Id = id;
            return detail;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增變價單明細失敗: {detail.PriceChangeId}/{detail.PriceChangeType}", ex);
            throw;
        }
    }

    public async Task<PriceChangeMaster> UpdateAsync(PriceChangeMaster priceChange)
    {
        try
        {
            const string sql = @"
                UPDATE PriceChangeMasters SET
                    SupplierId = @SupplierId,
                    LogoId = @LogoId,
                    ApplyEmpId = @ApplyEmpId,
                    ApplyOrgId = @ApplyOrgId,
                    ApplyDate = @ApplyDate,
                    StartDate = @StartDate,
                    ApproveEmpId = @ApproveEmpId,
                    ApproveDate = @ApproveDate,
                    ConfirmEmpId = @ConfirmEmpId,
                    ConfirmDate = @ConfirmDate,
                    Status = @Status,
                    TotalAmount = @TotalAmount,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";

            priceChange.UpdatedAt = DateTime.Now;

            await ExecuteAsync(sql, priceChange);
            return priceChange;
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

    public async Task DeleteDetailsAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            const string sql = @"
                DELETE FROM PriceChangeDetails 
                WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";

            await ExecuteAsync(sql, new { PriceChangeId = priceChangeId, PriceChangeType = priceChangeType });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除變價單明細失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }
}
