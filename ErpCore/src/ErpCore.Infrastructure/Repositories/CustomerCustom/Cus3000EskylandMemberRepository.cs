using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerCustom;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerCustom;

/// <summary>
/// CUS3000.ESKYLAND 會員 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Cus3000EskylandMemberRepository : BaseRepository, ICus3000EskylandMemberRepository
{
    public Cus3000EskylandMemberRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Cus3000EskylandMember?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000EskylandMembers 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Cus3000EskylandMember>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000.ESKYLAND會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Cus3000EskylandMember?> GetByMemberIdAsync(string memberId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000EskylandMembers 
                WHERE MemberId = @MemberId";

            return await QueryFirstOrDefaultAsync<Cus3000EskylandMember>(sql, new { MemberId = memberId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000.ESKYLAND會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<Cus3000EskylandMember?> GetByCardNoAsync(string cardNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000EskylandMembers 
                WHERE CardNo = @CardNo";

            return await QueryFirstOrDefaultAsync<Cus3000EskylandMember>(sql, new { CardNo = cardNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000.ESKYLAND會員失敗: {cardNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Cus3000EskylandMember>> QueryAsync(Cus3000EskylandMemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Cus3000EskylandMembers
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
                sql += " AND (MemberId LIKE @Keyword OR MemberName LIKE @Keyword OR CardNo LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            // 排序
            var sortField = !string.IsNullOrEmpty(query.SortField) ? query.SortField : "TKey";
            var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            if (query.PageSize > 0)
            {
                var offset = (query.PageIndex - 1) * query.PageSize;
                sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                parameters.Add("Offset", offset);
                parameters.Add("PageSize", query.PageSize);
            }

            return await QueryAsync<Cus3000EskylandMember>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000.ESKYLAND會員列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Cus3000EskylandMemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Cus3000EskylandMembers
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
                sql += " AND (MemberId LIKE @Keyword OR MemberName LIKE @Keyword OR CardNo LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000.ESKYLAND會員總數失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Cus3000EskylandMember>> GetAllAsync()
    {
        try
        {
            const string sql = "SELECT * FROM Cus3000EskylandMembers ORDER BY TKey";
            return await QueryAsync<Cus3000EskylandMember>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢所有CUS3000.ESKYLAND會員失敗", ex);
            throw;
        }
    }

    public async Task<Cus3000EskylandMember> AddAsync(Cus3000EskylandMember entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Cus3000EskylandMembers 
                (MemberId, MemberName, CardNo, EskylandSpecificField, Phone, Email, Address, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@MemberId, @MemberName, @CardNo, @EskylandSpecificField, @Phone, @Email, @Address, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("MemberId", entity.MemberId);
            parameters.Add("MemberName", entity.MemberName);
            parameters.Add("CardNo", entity.CardNo);
            parameters.Add("EskylandSpecificField", entity.EskylandSpecificField);
            parameters.Add("Phone", entity.Phone);
            parameters.Add("Email", entity.Email);
            parameters.Add("Address", entity.Address);
            parameters.Add("Status", entity.Status);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("CreatedAt", entity.CreatedAt);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            var tKey = await QuerySingleAsync<long>(sql, parameters);
            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增CUS3000.ESKYLAND會員失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Cus3000EskylandMember entity)
    {
        try
        {
            const string sql = @"
                UPDATE Cus3000EskylandMembers 
                SET MemberName = @MemberName,
                    CardNo = @CardNo,
                    EskylandSpecificField = @EskylandSpecificField,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var parameters = new DynamicParameters();
            parameters.Add("TKey", entity.TKey);
            parameters.Add("MemberName", entity.MemberName);
            parameters.Add("CardNo", entity.CardNo);
            parameters.Add("EskylandSpecificField", entity.EskylandSpecificField);
            parameters.Add("Phone", entity.Phone);
            parameters.Add("Email", entity.Email);
            parameters.Add("Address", entity.Address);
            parameters.Add("Status", entity.Status);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改CUS3000.ESKYLAND會員失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Cus3000EskylandMembers 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CUS3000.ESKYLAND會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Cus3000EskylandMember entity)
    {
        var result = await AddAsync(entity);
        return result.TKey;
    }
}

