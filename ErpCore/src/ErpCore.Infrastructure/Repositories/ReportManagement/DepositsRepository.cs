using System.Data;
using System.Linq;
using Dapper;
using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.ReportManagement;

/// <summary>
/// 保證金 Repository 實作 (SYSR510-SYSR570)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class DepositsRepository : BaseRepository, IDepositsRepository
{
    public DepositsRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Deposits?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Deposits 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Deposits>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢保證金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Deposits?> GetByDepositNoAsync(string depositNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Deposits 
                WHERE DepositNo = @DepositNo";

            return await QueryFirstOrDefaultAsync<Deposits>(sql, new { DepositNo = depositNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢保證金失敗: DepositNo={depositNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Deposits>> GetAllAsync()
    {
        try
        {
            const string sql = @"SELECT * FROM Deposits ORDER BY DepositDate DESC, TKey DESC";

            return await QueryAsync<Deposits>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢保證金列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<Deposits>> QueryAsync(DepositsQuery query)
    {
        try
        {
            var sql = @"SELECT * FROM Deposits WHERE 1=1";
            var countSql = @"SELECT COUNT(1) FROM Deposits WHERE 1=1";
            var parameters = new DynamicParameters();

            // 查詢條件
            if (!string.IsNullOrEmpty(query.DepositNo))
            {
                sql += " AND DepositNo LIKE @DepositNo";
                countSql += " AND DepositNo LIKE @DepositNo";
                parameters.Add("DepositNo", $"%{query.DepositNo}%");
            }

            if (!string.IsNullOrEmpty(query.ObjectId))
            {
                sql += " AND ObjectId = @ObjectId";
                countSql += " AND ObjectId = @ObjectId";
                parameters.Add("ObjectId", query.ObjectId);
            }

            if (query.DepositDateFrom.HasValue)
            {
                sql += " AND DepositDate >= @DepositDateFrom";
                countSql += " AND DepositDate >= @DepositDateFrom";
                parameters.Add("DepositDateFrom", query.DepositDateFrom.Value);
            }

            if (query.DepositDateTo.HasValue)
            {
                sql += " AND DepositDate <= @DepositDateTo";
                countSql += " AND DepositDate <= @DepositDateTo";
                parameters.Add("DepositDateTo", query.DepositDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.DepositStatus))
            {
                sql += " AND DepositStatus = @DepositStatus";
                countSql += " AND DepositStatus = @DepositStatus";
                parameters.Add("DepositStatus", query.DepositStatus);
            }

            if (!string.IsNullOrEmpty(query.DepositType))
            {
                sql += " AND DepositType = @DepositType";
                countSql += " AND DepositType = @DepositType";
                parameters.Add("DepositType", query.DepositType);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                countSql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                countSql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            // 排序
            var sortBy = query.SortBy ?? "DepositDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY {sortBy} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Deposits>(sql, parameters);
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            return new PagedResult<Deposits>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢保證金列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Deposits>> GetByObjectIdAsync(string objectId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Deposits 
                WHERE ObjectId = @ObjectId 
                ORDER BY DepositDate DESC";

            return await QueryAsync<Deposits>(sql, new { ObjectId = objectId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢保證金列表失敗: ObjectId={objectId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Deposits>> GetByDepositStatusAsync(string depositStatus)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Deposits 
                WHERE DepositStatus = @DepositStatus 
                ORDER BY DepositDate DESC";

            return await QueryAsync<Deposits>(sql, new { DepositStatus = depositStatus });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢保證金列表失敗: DepositStatus={depositStatus}", ex);
            throw;
        }
    }

    public async Task<Deposits> CreateAsync(Deposits entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Deposits (
                    DepositNo, DepositDate, ObjectId, DepositAmount, DepositType, DepositStatus,
                    ReturnDate, ReturnAmount, VoucherNo, VoucherM_TKey, VoucherStatus, CheckDueDate,
                    ShopId, SiteId, OrgId, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @DepositNo, @DepositDate, @ObjectId, @DepositAmount, @DepositType, @DepositStatus,
                    @ReturnDate, @ReturnAmount, @VoucherNo, @VoucherM_TKey, @VoucherStatus, @CheckDueDate,
                    @ShopId, @SiteId, @OrgId, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.DepositNo,
                entity.DepositDate,
                entity.ObjectId,
                entity.DepositAmount,
                entity.DepositType,
                entity.DepositStatus,
                entity.ReturnDate,
                entity.ReturnAmount,
                entity.VoucherNo,
                entity.VoucherM_TKey,
                entity.VoucherStatus,
                entity.CheckDueDate,
                entity.ShopId,
                entity.SiteId,
                entity.OrgId,
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
            _logger.LogError("新增保證金失敗", ex);
            throw;
        }
    }

    public async Task<Deposits> UpdateAsync(Deposits entity)
    {
        try
        {
            const string sql = @"
                UPDATE Deposits 
                SET DepositNo = @DepositNo,
                    DepositDate = @DepositDate,
                    ObjectId = @ObjectId,
                    DepositAmount = @DepositAmount,
                    DepositType = @DepositType,
                    DepositStatus = @DepositStatus,
                    ReturnDate = @ReturnDate,
                    ReturnAmount = @ReturnAmount,
                    VoucherNo = @VoucherNo,
                    VoucherM_TKey = @VoucherM_TKey,
                    VoucherStatus = @VoucherStatus,
                    CheckDueDate = @CheckDueDate,
                    ShopId = @ShopId,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.DepositNo,
                entity.DepositDate,
                entity.ObjectId,
                entity.DepositAmount,
                entity.DepositType,
                entity.DepositStatus,
                entity.ReturnDate,
                entity.ReturnAmount,
                entity.VoucherNo,
                entity.VoucherM_TKey,
                entity.VoucherStatus,
                entity.CheckDueDate,
                entity.ShopId,
                entity.SiteId,
                entity.OrgId,
                entity.Notes,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新保證金失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM Deposits WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除保證金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string depositNo)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) FROM Deposits 
                WHERE DepositNo = @DepositNo";

            var count = await ExecuteScalarAsync<int>(sql, new { DepositNo = depositNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查保證金是否存在失敗: DepositNo={depositNo}", ex);
            throw;
        }
    }
}

