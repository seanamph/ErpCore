using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 會員 Repository 實作 (SYS5210-SYS52A0 - 會員管理)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Std5000MemberRepository : BaseRepository, IStd5000MemberRepository
{
    public Std5000MemberRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Std5000Member?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000Members 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Std5000Member>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Std5000Member?> GetByMemberIdAsync(string memberId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000Members 
                WHERE MemberId = @MemberId";

            return await QueryFirstOrDefaultAsync<Std5000Member>(sql, new { MemberId = memberId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Std5000Member>> QueryAsync(Std5000MemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Std5000Members
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MemberId))
            {
                sql += " AND MemberId LIKE @MemberId";
                parameters.Add("MemberId", $"%{query.MemberId}%");
            }

            if (!string.IsNullOrEmpty(query.MemberName))
            {
                sql += " AND MemberName LIKE @MemberName";
                parameters.Add("MemberName", $"%{query.MemberName}%");
            }

            if (!string.IsNullOrEmpty(query.MemberType))
            {
                sql += " AND MemberType = @MemberType";
                parameters.Add("MemberType", query.MemberType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (MemberId LIKE @Keyword OR MemberName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY MemberId ASC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Std5000Member>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000會員列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Std5000MemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Std5000Members
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MemberId))
            {
                sql += " AND MemberId LIKE @MemberId";
                parameters.Add("MemberId", $"%{query.MemberId}%");
            }

            if (!string.IsNullOrEmpty(query.MemberName))
            {
                sql += " AND MemberName LIKE @MemberName";
                parameters.Add("MemberName", $"%{query.MemberName}%");
            }

            if (!string.IsNullOrEmpty(query.MemberType))
            {
                sql += " AND MemberType = @MemberType";
                parameters.Add("MemberType", query.MemberType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (MemberId LIKE @Keyword OR MemberName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000會員總數失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Std5000Member entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Std5000Members 
                (MemberId, MemberName, MemberType, IdCard, Phone, Email, Address, BirthDate, JoinDate, Points, Status, ShopId, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@MemberId, @MemberName, @MemberType, @IdCard, @Phone, @Email, @Address, @BirthDate, @JoinDate, @Points, @Status, @ShopId, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("MemberId", entity.MemberId);
            parameters.Add("MemberName", entity.MemberName);
            parameters.Add("MemberType", entity.MemberType);
            parameters.Add("IdCard", entity.IdCard);
            parameters.Add("Phone", entity.Phone);
            parameters.Add("Email", entity.Email);
            parameters.Add("Address", entity.Address);
            parameters.Add("BirthDate", entity.BirthDate);
            parameters.Add("JoinDate", entity.JoinDate);
            parameters.Add("Points", entity.Points);
            parameters.Add("Status", entity.Status);
            parameters.Add("ShopId", entity.ShopId);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("CreatedAt", entity.CreatedAt);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            return await QuerySingleAsync<long>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000會員失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Std5000Member entity)
    {
        try
        {
            const string sql = @"
                UPDATE Std5000Members 
                SET MemberName = @MemberName, 
                    MemberType = @MemberType,
                    IdCard = @IdCard,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address,
                    BirthDate = @BirthDate,
                    Points = @Points,
                    Status = @Status,
                    ShopId = @ShopId,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var parameters = new DynamicParameters();
            parameters.Add("TKey", entity.TKey);
            parameters.Add("MemberName", entity.MemberName);
            parameters.Add("MemberType", entity.MemberType);
            parameters.Add("IdCard", entity.IdCard);
            parameters.Add("Phone", entity.Phone);
            parameters.Add("Email", entity.Email);
            parameters.Add("Address", entity.Address);
            parameters.Add("BirthDate", entity.BirthDate);
            parameters.Add("Points", entity.Points);
            parameters.Add("Status", entity.Status);
            parameters.Add("ShopId", entity.ShopId);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改STD5000會員失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Std5000Members 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000會員失敗: {tKey}", ex);
            throw;
        }
    }
}

