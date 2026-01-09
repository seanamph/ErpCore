using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Loyalty;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Loyalty;

/// <summary>
/// 忠誠度會員 Repository 實作 (LPS - 忠誠度系統維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LoyaltyMemberRepository : BaseRepository, ILoyaltyMemberRepository
{
    public LoyaltyMemberRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LoyaltyMember?> GetByCardNoAsync(string cardNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LoyaltyMembers 
                WHERE CardNo = @CardNo";

            return await QueryFirstOrDefaultAsync<LoyaltyMember>(sql, new { CardNo = cardNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢忠誠度會員失敗: {cardNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LoyaltyMember>> QueryAsync(LoyaltyMemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LoyaltyMembers
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo LIKE @CardNo";
                parameters.Add("CardNo", $"%{query.CardNo}%");
            }

            if (!string.IsNullOrEmpty(query.MemberName))
            {
                sql += " AND MemberName LIKE @MemberName";
                parameters.Add("MemberName", $"%{query.MemberName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY CardNo ASC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LoyaltyMember>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度會員列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LoyaltyMemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LoyaltyMembers
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo LIKE @CardNo";
                parameters.Add("CardNo", $"%{query.CardNo}%");
            }

            if (!string.IsNullOrEmpty(query.MemberName))
            {
                sql += " AND MemberName LIKE @MemberName";
                parameters.Add("MemberName", $"%{query.MemberName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度會員數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(LoyaltyMember entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO LoyaltyMembers 
                (CardNo, MemberName, Phone, Email, TotalPoints, AvailablePoints, ExpDate, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@CardNo, @MemberName, @Phone, @Email, @TotalPoints, @AvailablePoints, @ExpDate, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await ExecuteAsync(sql, entity);
            return entity.CardNo;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增忠誠度會員失敗: {entity.CardNo}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(LoyaltyMember entity)
    {
        try
        {
            const string sql = @"
                UPDATE LoyaltyMembers 
                SET MemberName = @MemberName, 
                    Phone = @Phone, 
                    Email = @Email, 
                    TotalPoints = @TotalPoints, 
                    AvailablePoints = @AvailablePoints, 
                    ExpDate = @ExpDate, 
                    Status = @Status, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE CardNo = @CardNo";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改忠誠度會員失敗: {entity.CardNo}", ex);
            throw;
        }
    }

    public async Task UpdatePointsAsync(string cardNo, decimal totalPoints, decimal availablePoints)
    {
        try
        {
            const string sql = @"
                UPDATE LoyaltyMembers 
                SET TotalPoints = @TotalPoints, 
                    AvailablePoints = @AvailablePoints, 
                    UpdatedAt = GETDATE()
                WHERE CardNo = @CardNo";

            await ExecuteAsync(sql, new { CardNo = cardNo, TotalPoints = totalPoints, AvailablePoints = availablePoints });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新忠誠度會員點數失敗: {cardNo}", ex);
            throw;
        }
    }
}

