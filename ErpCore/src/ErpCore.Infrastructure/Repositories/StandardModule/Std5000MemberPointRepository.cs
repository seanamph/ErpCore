using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 會員積分 Repository 實作 (SYS5210-SYS52A0 - 會員積分管理)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Std5000MemberPointRepository : BaseRepository, IStd5000MemberPointRepository
{
    public Std5000MemberPointRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Std5000MemberPoint?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000MemberPoints 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Std5000MemberPoint>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000會員積分失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Std5000MemberPoint>> GetByMemberIdAsync(string memberId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000MemberPoints 
                WHERE MemberId = @MemberId
                ORDER BY TransDate DESC";

            return await QueryAsync<Std5000MemberPoint>(sql, new { MemberId = memberId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000會員積分失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Std5000MemberPoint>> QueryAsync(Std5000MemberPointQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Std5000MemberPoints
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MemberId))
            {
                sql += " AND MemberId = @MemberId";
                parameters.Add("MemberId", query.MemberId);
            }

            if (!string.IsNullOrEmpty(query.TransType))
            {
                sql += " AND TransType = @TransType";
                parameters.Add("TransType", query.TransType);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND TransDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND TransDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY TransDate DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Std5000MemberPoint>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000會員積分列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Std5000MemberPointQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Std5000MemberPoints
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MemberId))
            {
                sql += " AND MemberId = @MemberId";
                parameters.Add("MemberId", query.MemberId);
            }

            if (!string.IsNullOrEmpty(query.TransType))
            {
                sql += " AND TransType = @TransType";
                parameters.Add("TransType", query.TransType);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND TransDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND TransDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000會員積分總數失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Std5000MemberPoint entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Std5000MemberPoints 
                (MemberId, TransDate, TransType, Points, TransId, Memo, CreatedBy, CreatedAt)
                VALUES 
                (@MemberId, @TransDate, @TransType, @Points, @TransId, @Memo, @CreatedBy, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("MemberId", entity.MemberId);
            parameters.Add("TransDate", entity.TransDate);
            parameters.Add("TransType", entity.TransType);
            parameters.Add("Points", entity.Points);
            parameters.Add("TransId", entity.TransId);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("CreatedAt", entity.CreatedAt);

            return await QuerySingleAsync<long>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000會員積分失敗", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Std5000MemberPoints 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000會員積分失敗: {tKey}", ex);
            throw;
        }
    }
}

