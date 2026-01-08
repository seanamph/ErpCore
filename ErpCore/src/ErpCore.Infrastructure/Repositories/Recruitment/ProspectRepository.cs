using Dapper;
using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Recruitment;

/// <summary>
/// 潛客 Repository 實作 (SYSC180)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ProspectRepository : BaseRepository, IProspectRepository
{
    public ProspectRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Prospect?> GetByIdAsync(string prospectId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Prospects 
                WHERE ProspectId = @ProspectId";

            return await QueryFirstOrDefaultAsync<Prospect>(sql, new { ProspectId = prospectId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢潛客失敗: {prospectId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Prospect>> QueryAsync(ProspectQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Prospects
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProspectId))
            {
                sql += " AND ProspectId LIKE @ProspectId";
                parameters.Add("ProspectId", $"%{query.ProspectId}%");
            }

            if (!string.IsNullOrEmpty(query.ProspectName))
            {
                sql += " AND ProspectName LIKE @ProspectName";
                parameters.Add("ProspectName", $"%{query.ProspectName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.RecruitId))
            {
                sql += " AND RecruitId = @RecruitId";
                parameters.Add("RecruitId", query.RecruitId);
            }

            if (query.ContactDateFrom.HasValue)
            {
                sql += " AND ContactDate >= @ContactDateFrom";
                parameters.Add("ContactDateFrom", query.ContactDateFrom.Value);
            }

            if (query.ContactDateTo.HasValue)
            {
                sql += " AND ContactDate <= @ContactDateTo";
                parameters.Add("ContactDateTo", query.ContactDateTo.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ProspectName" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Prospect>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Prospects
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ProspectId))
            {
                countSql += " AND ProspectId LIKE @ProspectId";
                countParameters.Add("ProspectId", $"%{query.ProspectId}%");
            }
            if (!string.IsNullOrEmpty(query.ProspectName))
            {
                countSql += " AND ProspectName LIKE @ProspectName";
                countParameters.Add("ProspectName", $"%{query.ProspectName}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND SiteId = @SiteId";
                countParameters.Add("SiteId", query.SiteId);
            }
            if (!string.IsNullOrEmpty(query.RecruitId))
            {
                countSql += " AND RecruitId = @RecruitId";
                countParameters.Add("RecruitId", query.RecruitId);
            }
            if (query.ContactDateFrom.HasValue)
            {
                countSql += " AND ContactDate >= @ContactDateFrom";
                countParameters.Add("ContactDateFrom", query.ContactDateFrom.Value);
            }
            if (query.ContactDateTo.HasValue)
            {
                countSql += " AND ContactDate <= @ContactDateTo";
                countParameters.Add("ContactDateTo", query.ContactDateTo.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Prospect>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢潛客列表失敗", ex);
            throw;
        }
    }

    public async Task<Prospect> CreateAsync(Prospect prospect)
    {
        try
        {
            const string sql = @"
                INSERT INTO Prospects (
                    ProspectId, ProspectName, ContactPerson, ContactTel, ContactFax, ContactEmail, ContactAddress,
                    StoreName, StoreTel, SiteId, RecruitId, StoreId, VendorId, OrgId, BtypeId, SalesType,
                    Status, OverallStatus, PaperType, LocationType, DecoType, CommType, PdType,
                    BaseRent, Deposit, CreditCard, TargetAmountM, TargetAmountV, ExerciseFees, CheckDay,
                    AgmDateB, AgmDateE, ContractProidB, ContractProidE, FeedbackDate, DueDate, ContactDate,
                    VersionNo, GuiId, BankId, AccName, AccNo, InvEmail,
                    EdcYn, ReceYn, PosYn, CashYn, CommYn, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ProspectId, @ProspectName, @ContactPerson, @ContactTel, @ContactFax, @ContactEmail, @ContactAddress,
                    @StoreName, @StoreTel, @SiteId, @RecruitId, @StoreId, @VendorId, @OrgId, @BtypeId, @SalesType,
                    @Status, @OverallStatus, @PaperType, @LocationType, @DecoType, @CommType, @PdType,
                    @BaseRent, @Deposit, @CreditCard, @TargetAmountM, @TargetAmountV, @ExerciseFees, @CheckDay,
                    @AgmDateB, @AgmDateE, @ContractProidB, @ContractProidE, @FeedbackDate, @DueDate, @ContactDate,
                    @VersionNo, @GuiId, @BankId, @AccName, @AccNo, @InvEmail,
                    @EdcYn, @ReceYn, @PosYn, @CashYn, @CommYn, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Prospect>(sql, prospect);
            if (result == null)
            {
                throw new InvalidOperationException("新增潛客失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增潛客失敗: {prospect.ProspectId}", ex);
            throw;
        }
    }

    public async Task<Prospect> UpdateAsync(Prospect prospect)
    {
        try
        {
            const string sql = @"
                UPDATE Prospects SET
                    ProspectName = @ProspectName,
                    ContactPerson = @ContactPerson,
                    ContactTel = @ContactTel,
                    ContactFax = @ContactFax,
                    ContactEmail = @ContactEmail,
                    ContactAddress = @ContactAddress,
                    StoreName = @StoreName,
                    StoreTel = @StoreTel,
                    SiteId = @SiteId,
                    RecruitId = @RecruitId,
                    StoreId = @StoreId,
                    VendorId = @VendorId,
                    OrgId = @OrgId,
                    BtypeId = @BtypeId,
                    SalesType = @SalesType,
                    Status = @Status,
                    OverallStatus = @OverallStatus,
                    PaperType = @PaperType,
                    LocationType = @LocationType,
                    DecoType = @DecoType,
                    CommType = @CommType,
                    PdType = @PdType,
                    BaseRent = @BaseRent,
                    Deposit = @Deposit,
                    CreditCard = @CreditCard,
                    TargetAmountM = @TargetAmountM,
                    TargetAmountV = @TargetAmountV,
                    ExerciseFees = @ExerciseFees,
                    CheckDay = @CheckDay,
                    AgmDateB = @AgmDateB,
                    AgmDateE = @AgmDateE,
                    ContractProidB = @ContractProidB,
                    ContractProidE = @ContractProidE,
                    FeedbackDate = @FeedbackDate,
                    DueDate = @DueDate,
                    ContactDate = @ContactDate,
                    VersionNo = @VersionNo,
                    GuiId = @GuiId,
                    BankId = @BankId,
                    AccName = @AccName,
                    AccNo = @AccNo,
                    InvEmail = @InvEmail,
                    EdcYn = @EdcYn,
                    ReceYn = @ReceYn,
                    PosYn = @PosYn,
                    CashYn = @CashYn,
                    CommYn = @CommYn,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE ProspectId = @ProspectId";

            var result = await QueryFirstOrDefaultAsync<Prospect>(sql, prospect);
            if (result == null)
            {
                throw new InvalidOperationException($"潛客不存在: {prospect.ProspectId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改潛客失敗: {prospect.ProspectId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string prospectId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Prospects
                WHERE ProspectId = @ProspectId";

            var rowsAffected = await ExecuteAsync(sql, new { ProspectId = prospectId });
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"潛客不存在: {prospectId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除潛客失敗: {prospectId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string prospectId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Prospects
                WHERE ProspectId = @ProspectId";

            var count = await QuerySingleAsync<int>(sql, new { ProspectId = prospectId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查潛客是否存在失敗: {prospectId}", ex);
            throw;
        }
    }

    public async Task<bool> HasRelatedInterviewsAsync(string prospectId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Interviews
                WHERE ProspectId = @ProspectId";

            var count = await QuerySingleAsync<int>(sql, new { ProspectId = prospectId });
            return count > 0;
        }
        catch
        {
            // 如果 Interviews 表不存在，返回 false
            return false;
        }
    }
}

