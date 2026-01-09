using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StoreMember;

/// <summary>
/// 會員 Repository 實作 (SYS3000 - 會員資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class MemberRepository : BaseRepository, IMemberRepository
{
    public MemberRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Member?> GetByIdAsync(string memberId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Members 
                WHERE MemberId = @MemberId";

            return await QueryFirstOrDefaultAsync<Member>(sql, new { MemberId = memberId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Member>> QueryAsync(MemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Members
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

            if (!string.IsNullOrEmpty(query.PersonalId))
            {
                sql += " AND PersonalId LIKE @PersonalId";
                parameters.Add("PersonalId", $"%{query.PersonalId}%");
            }

            if (!string.IsNullOrEmpty(query.Phone))
            {
                sql += " AND Phone LIKE @Phone";
                parameters.Add("Phone", $"%{query.Phone}%");
            }

            if (!string.IsNullOrEmpty(query.Mobile))
            {
                sql += " AND Mobile LIKE @Mobile";
                parameters.Add("Mobile", $"%{query.Mobile}%");
            }

            if (!string.IsNullOrEmpty(query.Email))
            {
                sql += " AND Email LIKE @Email";
                parameters.Add("Email", $"%{query.Email}%");
            }

            if (!string.IsNullOrEmpty(query.MemberLevel))
            {
                sql += " AND MemberLevel = @MemberLevel";
                parameters.Add("MemberLevel", query.MemberLevel);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo LIKE @CardNo";
                parameters.Add("CardNo", $"%{query.CardNo}%");
            }

            // 排序
            var sortField = query.SortField ?? "MemberId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Member>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<Member>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢會員列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(MemberQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Members
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

            if (!string.IsNullOrEmpty(query.PersonalId))
            {
                sql += " AND PersonalId LIKE @PersonalId";
                parameters.Add("PersonalId", $"%{query.PersonalId}%");
            }

            if (!string.IsNullOrEmpty(query.Phone))
            {
                sql += " AND Phone LIKE @Phone";
                parameters.Add("Phone", $"%{query.Phone}%");
            }

            if (!string.IsNullOrEmpty(query.Mobile))
            {
                sql += " AND Mobile LIKE @Mobile";
                parameters.Add("Mobile", $"%{query.Mobile}%");
            }

            if (!string.IsNullOrEmpty(query.Email))
            {
                sql += " AND Email LIKE @Email";
                parameters.Add("Email", $"%{query.Email}%");
            }

            if (!string.IsNullOrEmpty(query.MemberLevel))
            {
                sql += " AND MemberLevel = @MemberLevel";
                parameters.Add("MemberLevel", query.MemberLevel);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo LIKE @CardNo";
                parameters.Add("CardNo", $"%{query.CardNo}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢會員總數失敗", ex);
            throw;
        }
    }

    public async Task<Member> CreateAsync(Member member)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            member.CreatedAt = DateTime.Now;
            member.UpdatedAt = DateTime.Now;

            const string sql = @"
                INSERT INTO Members (
                    MemberId, MemberName, MemberNameEn, Gender, BirthDate, PersonalId,
                    Phone, Mobile, Email, Address, City, Zone, PostalCode,
                    MemberLevel, Points, TotalPoints, CardNo, CardType,
                    JoinDate, ExpireDate, Status, PhotoPath, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @MemberId, @MemberName, @MemberNameEn, @Gender, @BirthDate, @PersonalId,
                    @Phone, @Mobile, @Email, @Address, @City, @Zone, @PostalCode,
                    @MemberLevel, @Points, @TotalPoints, @CardNo, @CardType,
                    @JoinDate, @ExpireDate, @Status, @PhotoPath, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await connection.QuerySingleAsync<long>(sql, member, transaction);
            member.TKey = tKey;

            transaction.Commit();
            _logger.LogInfo($"建立會員成功: {member.MemberId}");
            return member;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立會員失敗: {member.MemberId}", ex);
            throw;
        }
    }

    public async Task<Member> UpdateAsync(Member member)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            member.UpdatedAt = DateTime.Now;

            const string sql = @"
                UPDATE Members SET
                    MemberName = @MemberName,
                    MemberNameEn = @MemberNameEn,
                    Gender = @Gender,
                    BirthDate = @BirthDate,
                    PersonalId = @PersonalId,
                    Phone = @Phone,
                    Mobile = @Mobile,
                    Email = @Email,
                    Address = @Address,
                    City = @City,
                    Zone = @Zone,
                    PostalCode = @PostalCode,
                    MemberLevel = @MemberLevel,
                    Points = @Points,
                    TotalPoints = @TotalPoints,
                    CardNo = @CardNo,
                    CardType = @CardType,
                    JoinDate = @JoinDate,
                    ExpireDate = @ExpireDate,
                    Status = @Status,
                    PhotoPath = @PhotoPath,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE MemberId = @MemberId";

            await connection.ExecuteAsync(sql, member, transaction);

            transaction.Commit();
            _logger.LogInfo($"更新會員成功: {member.MemberId}");
            return member;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新會員失敗: {member.MemberId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string memberId)
    {
        try
        {
            const string sql = @"
                UPDATE Members SET
                    Status = 'I',
                    UpdatedAt = GETDATE()
                WHERE MemberId = @MemberId";

            await ExecuteAsync(sql, new { MemberId = memberId });
            _logger.LogInfo($"刪除會員成功: {memberId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string memberId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Members
                WHERE MemberId = @MemberId";

            var count = await ExecuteScalarAsync<int>(sql, new { MemberId = memberId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查會員編號是否存在失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string memberId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE Members SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE MemberId = @MemberId";

            await ExecuteAsync(sql, new { MemberId = memberId, Status = status });
            _logger.LogInfo($"更新會員狀態成功: {memberId}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新會員狀態失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task UpdatePhotoPathAsync(string memberId, string photoPath)
    {
        try
        {
            const string sql = @"
                UPDATE Members SET
                    PhotoPath = @PhotoPath,
                    UpdatedAt = GETDATE()
                WHERE MemberId = @MemberId";

            await ExecuteAsync(sql, new { MemberId = memberId, PhotoPath = photoPath });
            _logger.LogInfo($"更新會員照片路徑成功: {memberId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新會員照片路徑失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<MemberPoint>> GetMemberPointsAsync(string memberId, MemberPointQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM MemberPoints
                WHERE MemberId = @MemberId";

            var parameters = new DynamicParameters();
            parameters.Add("MemberId", memberId);

            if (query.TransactionDateFrom.HasValue)
            {
                sql += " AND TransactionDate >= @TransactionDateFrom";
                parameters.Add("TransactionDateFrom", query.TransactionDateFrom.Value);
            }

            if (query.TransactionDateTo.HasValue)
            {
                sql += " AND TransactionDate <= @TransactionDateTo";
                parameters.Add("TransactionDateTo", query.TransactionDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.TransactionType))
            {
                sql += " AND TransactionType = @TransactionType";
                parameters.Add("TransactionType", query.TransactionType);
            }

            // 排序
            sql += " ORDER BY TransactionDate DESC";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<MemberPoint>(sql, parameters);

            // 計算總數
            var countSql = @"
                SELECT COUNT(*) FROM MemberPoints
                WHERE MemberId = @MemberId";
            var countParameters = new DynamicParameters();
            countParameters.Add("MemberId", memberId);

            if (query.TransactionDateFrom.HasValue)
            {
                countSql += " AND TransactionDate >= @TransactionDateFrom";
                countParameters.Add("TransactionDateFrom", query.TransactionDateFrom.Value);
            }

            if (query.TransactionDateTo.HasValue)
            {
                countSql += " AND TransactionDate <= @TransactionDateTo";
                countParameters.Add("TransactionDateTo", query.TransactionDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.TransactionType))
            {
                countSql += " AND TransactionType = @TransactionType";
                countParameters.Add("TransactionType", query.TransactionType);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<MemberPoint>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會員積分記錄失敗: {memberId}", ex);
            throw;
        }
    }
}

