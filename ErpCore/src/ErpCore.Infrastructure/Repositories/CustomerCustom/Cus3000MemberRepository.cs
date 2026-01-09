using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerCustom;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerCustom;

/// <summary>
/// CUS3000 會員 Repository 實作 (SYS3130-SYS3160 - 會員管理)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Cus3000MemberRepository : BaseRepository, ICus3000MemberRepository
{
    public Cus3000MemberRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Cus3000Member?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000Members 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Cus3000Member>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Cus3000Member?> GetByMemberIdAsync(string memberId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000Members 
                WHERE MemberId = @MemberId";

            return await QueryFirstOrDefaultAsync<Cus3000Member>(sql, new { MemberId = memberId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<Cus3000Member?> GetByCardNoAsync(string cardNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000Members 
                WHERE CardNo = @CardNo";

            return await QueryFirstOrDefaultAsync<Cus3000Member>(sql, new { CardNo = cardNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000會員失敗: {cardNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Cus3000Member>> QueryAsync(Cus3000MemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Cus3000Members
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

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo LIKE @CardNo";
                parameters.Add("CardNo", $"%{query.CardNo}%");
            }

            if (!string.IsNullOrEmpty(query.Phone))
            {
                sql += " AND Phone LIKE @Phone";
                parameters.Add("Phone", $"%{query.Phone}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (MemberId LIKE @Keyword OR MemberName LIKE @Keyword OR CardNo LIKE @Keyword OR Phone LIKE @Keyword)";
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
                sql += " ORDER BY CreatedAt DESC";
            }

            // 分頁
            if (query.PageSize > 0)
            {
                var offset = (query.PageIndex - 1) * query.PageSize;
                sql += $" OFFSET {offset} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";
            }

            return await QueryAsync<Cus3000Member>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000會員列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Cus3000MemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Cus3000Members
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

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo LIKE @CardNo";
                parameters.Add("CardNo", $"%{query.CardNo}%");
            }

            if (!string.IsNullOrEmpty(query.Phone))
            {
                sql += " AND Phone LIKE @Phone";
                parameters.Add("Phone", $"%{query.Phone}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (MemberId LIKE @Keyword OR MemberName LIKE @Keyword OR CardNo LIKE @Keyword OR Phone LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000會員數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Cus3000Member entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Cus3000Members 
                (MemberId, MemberName, CardNo, Phone, Email, Address, BirthDate, Gender, PhotoPath, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@MemberId, @MemberName, @CardNo, @Phone, @Email, @Address, @BirthDate, @Gender, @PhotoPath, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            return await ExecuteScalarAsync<long>(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增CUS3000會員失敗: {entity.MemberId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Cus3000Member entity)
    {
        try
        {
            const string sql = @"
                UPDATE Cus3000Members 
                SET MemberName = @MemberName, CardNo = @CardNo, Phone = @Phone, Email = @Email, 
                    Address = @Address, BirthDate = @BirthDate, Gender = @Gender, PhotoPath = @PhotoPath, 
                    Status = @Status, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新CUS3000會員失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Cus3000Members 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CUS3000會員失敗: {tKey}", ex);
            throw;
        }
    }
}

