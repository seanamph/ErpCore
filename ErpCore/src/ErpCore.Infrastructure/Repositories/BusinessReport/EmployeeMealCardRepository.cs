using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 員工餐卡申請 Repository 實作 (SYSL130)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EmployeeMealCardRepository : BaseRepository, IEmployeeMealCardRepository
{
    public EmployeeMealCardRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EmployeeMealCard?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EmployeeMealCards 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<EmployeeMealCard>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢員工餐卡申請失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EmployeeMealCard>> QueryAsync(EmployeeMealCardQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EmployeeMealCards
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.EmpId))
            {
                sql += " AND EmpId LIKE @EmpId";
                parameters.Add("EmpId", $"%{query.EmpId}%");
            }

            if (!string.IsNullOrEmpty(query.EmpName))
            {
                sql += " AND EmpName LIKE @EmpName";
                parameters.Add("EmpName", $"%{query.EmpName}%");
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.CardType))
            {
                sql += " AND CardType = @CardType";
                parameters.Add("CardType", query.CardType);
            }

            if (!string.IsNullOrEmpty(query.ActionType))
            {
                sql += " AND ActionType = @ActionType";
                parameters.Add("ActionType", query.ActionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
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

            // 計算總筆數
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "TKey" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EmployeeMealCard>(sql, parameters);

            return new PagedResult<EmployeeMealCard>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員工餐卡申請列表失敗", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCard> CreateAsync(EmployeeMealCard mealCard)
    {
        try
        {
            const string sql = @"
                INSERT INTO EmployeeMealCards 
                (EmpId, EmpName, OrgId, SiteId, CardType, ActionType, ActionTypeD, 
                 StartDate, EndDate, Status, Notes, TxnNo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup)
                VALUES 
                (@EmpId, @EmpName, @OrgId, @SiteId, @CardType, @ActionType, @ActionTypeD, 
                 @StartDate, @EndDate, @Status, @Notes, @TxnNo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, mealCard);
            mealCard.TKey = tKey;

            return mealCard;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增員工餐卡申請失敗", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCard> UpdateAsync(EmployeeMealCard mealCard)
    {
        try
        {
            const string sql = @"
                UPDATE EmployeeMealCards SET
                    EmpName = @EmpName,
                    OrgId = @OrgId,
                    SiteId = @SiteId,
                    CardType = @CardType,
                    ActionType = @ActionType,
                    ActionTypeD = @ActionTypeD,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Notes = @Notes,
                    TxnNo = @TxnNo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, mealCard);
            return mealCard;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改員工餐卡申請失敗: {mealCard.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = "DELETE FROM EmployeeMealCards WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除員工餐卡申請失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchVerifyAsync(List<long> tKeys, string action, string verifier, string? notes)
    {
        try
        {
            var status = action.ToLower() == "approve" ? "A" : "R";
            var verifyDate = DateTime.Now;

            const string sql = @"
                UPDATE EmployeeMealCards SET
                    Status = @Status,
                    Verifier = @Verifier,
                    VerifyDate = @VerifyDate,
                    Notes = @Notes,
                    UpdatedBy = @Verifier,
                    UpdatedAt = @VerifyDate
                WHERE TKey IN @TKeys AND Status = 'P'";

            var parameters = new
            {
                Status = status,
                Verifier = verifier,
                VerifyDate = verifyDate,
                Notes = notes,
                TKeys = tKeys
            };

            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次審核員工餐卡申請失敗", ex);
            throw;
        }
    }
}

