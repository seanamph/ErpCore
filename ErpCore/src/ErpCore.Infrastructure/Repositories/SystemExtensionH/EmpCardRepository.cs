using System.Data;
using Dapper;
using ErpCore.Domain.Entities.SystemExtensionH;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionH;

/// <summary>
/// 員工感應卡 Repository 實作 (SYSPH00 - 系統擴展PH)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EmpCardRepository : BaseRepository, IEmpCardRepository
{
    public EmpCardRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EmpCard?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EmpCard 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<EmpCard>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢感應卡失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EmpCard?> GetByCardNoAsync(string cardNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EmpCard 
                WHERE CardNo = @CardNo";

            return await QueryFirstOrDefaultAsync<EmpCard>(sql, new { CardNo = cardNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢感應卡失敗: {cardNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EmpCard>> QueryAsync(EmpCardQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EmpCard
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo LIKE @CardNo";
                parameters.Add("CardNo", $"%{query.CardNo}%");
            }

            if (!string.IsNullOrEmpty(query.EmpId))
            {
                sql += " AND EmpId = @EmpId";
                parameters.Add("EmpId", query.EmpId);
            }

            if (!string.IsNullOrEmpty(query.CardStatus))
            {
                sql += " AND CardStatus = @CardStatus";
                parameters.Add("CardStatus", query.CardStatus);
            }

            if (query.BeginDateFrom.HasValue)
            {
                sql += " AND BeginDate >= @BeginDateFrom";
                parameters.Add("BeginDateFrom", query.BeginDateFrom.Value);
            }

            if (query.BeginDateTo.HasValue)
            {
                sql += " AND BeginDate <= @BeginDateTo";
                parameters.Add("BeginDateTo", query.BeginDateTo.Value);
            }

            if (query.EndDateFrom.HasValue)
            {
                sql += " AND EndDate >= @EndDateFrom";
                parameters.Add("EndDateFrom", query.EndDateFrom.Value);
            }

            if (query.EndDateTo.HasValue)
            {
                sql += " AND EndDate <= @EndDateTo";
                parameters.Add("EndDateTo", query.EndDateTo.Value);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY BTime DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<EmpCard>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢感應卡列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(EmpCardQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM EmpCard
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo LIKE @CardNo";
                parameters.Add("CardNo", $"%{query.CardNo}%");
            }

            if (!string.IsNullOrEmpty(query.EmpId))
            {
                sql += " AND EmpId = @EmpId";
                parameters.Add("EmpId", query.EmpId);
            }

            if (!string.IsNullOrEmpty(query.CardStatus))
            {
                sql += " AND CardStatus = @CardStatus";
                parameters.Add("CardStatus", query.CardStatus);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢感應卡數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(EmpCard entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO EmpCard 
                (CardNo, EmpId, BeginDate, EndDate, CardStatus, Notes, BUser, BTime, CUser, CTime, CPriority, CGroup)
                VALUES 
                (@CardNo, @EmpId, @BeginDate, @EndDate, @CardStatus, @Notes, @BUser, @BTime, @CUser, @CTime, @CPriority, @CGroup);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.CardNo,
                entity.EmpId,
                entity.BeginDate,
                entity.EndDate,
                entity.CardStatus,
                entity.Notes,
                entity.BUser,
                entity.BTime,
                entity.CUser,
                entity.CTime,
                entity.CPriority,
                entity.CGroup
            });

            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增感應卡失敗: {entity.CardNo}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(EmpCard entity)
    {
        try
        {
            const string sql = @"
                UPDATE EmpCard 
                SET CardNo = @CardNo,
                    EmpId = @EmpId,
                    BeginDate = @BeginDate,
                    EndDate = @EndDate,
                    CardStatus = @CardStatus,
                    Notes = @Notes,
                    CUser = @CUser,
                    CTime = @CTime
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.CardNo,
                entity.EmpId,
                entity.BeginDate,
                entity.EndDate,
                entity.CardStatus,
                entity.Notes,
                entity.CUser,
                entity.CTime
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新感應卡失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM EmpCard 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除感應卡失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByCardNoAsync(string cardNo)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM EmpCard 
                WHERE CardNo = @CardNo";

            var count = await ExecuteScalarAsync<int>(sql, new { CardNo = cardNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查感應卡號是否存在失敗: {cardNo}", ex);
            throw;
        }
    }

    public async Task<int> CreateBatchAsync(IEnumerable<EmpCard> entities)
    {
        try
        {
            const string sql = @"
                INSERT INTO EmpCard 
                (CardNo, EmpId, BeginDate, EndDate, CardStatus, Notes, BUser, BTime, CUser, CTime, CPriority, CGroup)
                VALUES 
                (@CardNo, @EmpId, @BeginDate, @EndDate, @CardStatus, @Notes, @BUser, @BTime, @CUser, @CTime, @CPriority, @CGroup)";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteAsync(sql, entities);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError("批量新增感應卡失敗", ex);
            throw;
        }
    }
}

