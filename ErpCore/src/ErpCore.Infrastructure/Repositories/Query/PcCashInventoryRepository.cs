using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Query;

/// <summary>
/// 零用金盤點檔 Repository 實作 (SYSQ241, SYSQ242)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PcCashInventoryRepository : BaseRepository, IPcCashInventoryRepository
{
    public PcCashInventoryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PcCashInventory?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcCashInventory 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<PcCashInventory>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金盤點檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcCashInventory?> GetByInventoryIdAsync(string inventoryId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PcCashInventory 
                WHERE InventoryId = @InventoryId";

            return await QueryFirstOrDefaultAsync<PcCashInventory>(sql, new { InventoryId = inventoryId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金盤點檔失敗: {inventoryId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PcCashInventory>> QueryAsync(PcCashInventoryQueryParams query)
    {
        try
        {
            var sql = @"
                SELECT 
                    pci.*,
                    s.SiteName,
                    e.EmpName AS KeepEmpName
                FROM PcCashInventory pci
                LEFT JOIN Sites s ON pci.SiteId = s.SiteId
                LEFT JOIN V_EMP_USER e ON pci.KeepEmpId = e.EmpId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND pci.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (query.InventoryDateFrom.HasValue)
            {
                sql += " AND pci.InventoryDate >= @InventoryDateFrom";
                parameters.Add("InventoryDateFrom", query.InventoryDateFrom.Value);
            }

            if (query.InventoryDateTo.HasValue)
            {
                sql += " AND pci.InventoryDate <= @InventoryDateTo";
                parameters.Add("InventoryDateTo", query.InventoryDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.KeepEmpId))
            {
                sql += " AND pci.KeepEmpId = @KeepEmpId";
                parameters.Add("KeepEmpId", query.KeepEmpId);
            }

            if (!string.IsNullOrEmpty(query.InventoryStatus))
            {
                sql += " AND pci.InventoryStatus = @InventoryStatus";
                parameters.Add("InventoryStatus", query.InventoryStatus);
            }

            // 排序
            sql += " ORDER BY pci.InventoryDate DESC, pci.InventoryId DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PcCashInventory>(sql, parameters);

            // 計算差異金額
            foreach (var item in items)
            {
                if (item.ActualAmount.HasValue)
                {
                    item.DifferenceAmount = item.ActualAmount.Value - item.InventoryAmount;
                }
            }

            // 查詢總數
            var countSql = @"SELECT COUNT(*) FROM PcCashInventory pci WHERE 1=1";
            var countParameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND pci.SiteId = @SiteId";
                countParameters.Add("SiteId", query.SiteId);
            }

            if (query.InventoryDateFrom.HasValue)
            {
                countSql += " AND pci.InventoryDate >= @InventoryDateFrom";
                countParameters.Add("InventoryDateFrom", query.InventoryDateFrom.Value);
            }

            if (query.InventoryDateTo.HasValue)
            {
                countSql += " AND pci.InventoryDate <= @InventoryDateTo";
                countParameters.Add("InventoryDateTo", query.InventoryDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.KeepEmpId))
            {
                countSql += " AND pci.KeepEmpId = @KeepEmpId";
                countParameters.Add("KeepEmpId", query.KeepEmpId);
            }

            if (!string.IsNullOrEmpty(query.InventoryStatus))
            {
                countSql += " AND pci.InventoryStatus = @InventoryStatus";
                countParameters.Add("InventoryStatus", query.InventoryStatus);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<PcCashInventory>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金盤點檔列表失敗", ex);
            throw;
        }
    }

    public async Task<PcCashInventory> CreateAsync(PcCashInventory entity)
    {
        try
        {
            // 計算差異金額
            if (entity.ActualAmount.HasValue)
            {
                entity.DifferenceAmount = entity.ActualAmount.Value - entity.InventoryAmount;
            }

            const string sql = @"
                INSERT INTO PcCashInventory (InventoryId, SiteId, InventoryDate, KeepEmpId, InventoryAmount, ActualAmount, DifferenceAmount, InventoryStatus, Notes, BUser, BTime, CUser, CTime)
                VALUES (@InventoryId, @SiteId, @InventoryDate, @KeepEmpId, @InventoryAmount, @ActualAmount, @DifferenceAmount, @InventoryStatus, @Notes, @BUser, @BTime, @CUser, @CTime);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.InventoryId,
                entity.SiteId,
                entity.InventoryDate,
                entity.KeepEmpId,
                entity.InventoryAmount,
                entity.ActualAmount,
                entity.DifferenceAmount,
                entity.InventoryStatus,
                entity.Notes,
                entity.BUser,
                entity.BTime,
                entity.CUser,
                entity.CTime
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增零用金盤點檔失敗", ex);
            throw;
        }
    }

    public async Task<PcCashInventory> UpdateAsync(PcCashInventory entity)
    {
        try
        {
            // 計算差異金額
            if (entity.ActualAmount.HasValue)
            {
                entity.DifferenceAmount = entity.ActualAmount.Value - entity.InventoryAmount;
            }

            const string sql = @"
                UPDATE PcCashInventory 
                SET ActualAmount = @ActualAmount,
                    DifferenceAmount = @DifferenceAmount,
                    InventoryStatus = @InventoryStatus,
                    Notes = @Notes,
                    CUser = @CUser,
                    CTime = @CTime
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.ActualAmount,
                entity.DifferenceAmount,
                entity.InventoryStatus,
                entity.Notes,
                entity.CUser,
                entity.CTime
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金盤點檔失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM PcCashInventory WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金盤點檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<string> GenerateInventoryIdAsync(string? siteId)
    {
        try
        {
            // 生成盤點單號格式: PI{YYYYMMDD}{流水號}
            var prefix = $"PI{DateTime.Now:yyyyMMdd}";
            var sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(InventoryId, LEN(@Prefix) + 1, LEN(InventoryId)) AS INT)), 0) + 1
                FROM PcCashInventory
                WHERE InventoryId LIKE @Prefix + '%'";

            var parameters = new DynamicParameters();
            parameters.Add("Prefix", prefix);

            var sequence = await ExecuteScalarAsync<int>(sql, parameters);
            return $"{prefix}{sequence:D4}";
        }
        catch (Exception ex)
        {
            _logger.LogError("生成盤點單號失敗", ex);
            throw;
        }
    }
}

