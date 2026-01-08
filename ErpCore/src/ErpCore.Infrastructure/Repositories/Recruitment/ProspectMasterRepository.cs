using Dapper;
using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Recruitment;

/// <summary>
/// 潛客主檔 Repository 實作 (SYSC165)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ProspectMasterRepository : BaseRepository, IProspectMasterRepository
{
    public ProspectMasterRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ProspectMaster?> GetByIdAsync(string masterId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ProspectMasters 
                WHERE MasterId = @MasterId";

            return await QueryFirstOrDefaultAsync<ProspectMaster>(sql, new { MasterId = masterId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢潛客主檔失敗: {masterId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ProspectMaster>> QueryAsync(ProspectMasterQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ProspectMasters
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MasterId))
            {
                sql += " AND MasterId LIKE @MasterId";
                parameters.Add("MasterId", $"%{query.MasterId}%");
            }

            if (!string.IsNullOrEmpty(query.MasterName))
            {
                sql += " AND MasterName LIKE @MasterName";
                parameters.Add("MasterName", $"%{query.MasterName}%");
            }

            if (!string.IsNullOrEmpty(query.MasterType))
            {
                sql += " AND MasterType = @MasterType";
                parameters.Add("MasterType", query.MasterType);
            }

            if (!string.IsNullOrEmpty(query.Category))
            {
                sql += " AND Category = @Category";
                parameters.Add("Category", query.Category);
            }

            if (!string.IsNullOrEmpty(query.Industry))
            {
                sql += " AND Industry = @Industry";
                parameters.Add("Industry", query.Industry);
            }

            if (!string.IsNullOrEmpty(query.BusinessType))
            {
                sql += " AND BusinessType = @BusinessType";
                parameters.Add("BusinessType", query.BusinessType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Source))
            {
                sql += " AND Source = @Source";
                parameters.Add("Source", query.Source);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "MasterId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ProspectMaster>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ProspectMasters
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.MasterId))
            {
                countSql += " AND MasterId LIKE @MasterId";
                countParameters.Add("MasterId", $"%{query.MasterId}%");
            }
            if (!string.IsNullOrEmpty(query.MasterName))
            {
                countSql += " AND MasterName LIKE @MasterName";
                countParameters.Add("MasterName", $"%{query.MasterName}%");
            }
            if (!string.IsNullOrEmpty(query.MasterType))
            {
                countSql += " AND MasterType = @MasterType";
                countParameters.Add("MasterType", query.MasterType);
            }
            if (!string.IsNullOrEmpty(query.Category))
            {
                countSql += " AND Category = @Category";
                countParameters.Add("Category", query.Category);
            }
            if (!string.IsNullOrEmpty(query.Industry))
            {
                countSql += " AND Industry = @Industry";
                countParameters.Add("Industry", query.Industry);
            }
            if (!string.IsNullOrEmpty(query.BusinessType))
            {
                countSql += " AND BusinessType = @BusinessType";
                countParameters.Add("BusinessType", query.BusinessType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.Source))
            {
                countSql += " AND Source = @Source";
                countParameters.Add("Source", query.Source);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ProspectMaster>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢潛客主檔列表失敗", ex);
            throw;
        }
    }

    public async Task<ProspectMaster> CreateAsync(ProspectMaster prospectMaster)
    {
        try
        {
            const string sql = @"
                INSERT INTO ProspectMasters (
                    MasterId, MasterName, MasterType, Category, Industry, BusinessType,
                    Status, Priority, Source,
                    ContactPerson, ContactTel, ContactEmail, ContactAddress, Website, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @MasterId, @MasterName, @MasterType, @Category, @Industry, @BusinessType,
                    @Status, @Priority, @Source,
                    @ContactPerson, @ContactTel, @ContactEmail, @ContactAddress, @Website, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<ProspectMaster>(sql, prospectMaster);
            if (result == null)
            {
                throw new InvalidOperationException("新增潛客主檔失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增潛客主檔失敗: {prospectMaster.MasterId}", ex);
            throw;
        }
    }

    public async Task<ProspectMaster> UpdateAsync(ProspectMaster prospectMaster)
    {
        try
        {
            const string sql = @"
                UPDATE ProspectMasters SET
                    MasterName = @MasterName,
                    MasterType = @MasterType,
                    Category = @Category,
                    Industry = @Industry,
                    BusinessType = @BusinessType,
                    Status = @Status,
                    Priority = @Priority,
                    Source = @Source,
                    ContactPerson = @ContactPerson,
                    ContactTel = @ContactTel,
                    ContactEmail = @ContactEmail,
                    ContactAddress = @ContactAddress,
                    Website = @Website,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE MasterId = @MasterId";

            var result = await QueryFirstOrDefaultAsync<ProspectMaster>(sql, prospectMaster);
            if (result == null)
            {
                throw new InvalidOperationException($"潛客主檔不存在: {prospectMaster.MasterId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改潛客主檔失敗: {prospectMaster.MasterId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string masterId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ProspectMasters
                WHERE MasterId = @MasterId";

            var rowsAffected = await ExecuteAsync(sql, new { MasterId = masterId });
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"潛客主檔不存在: {masterId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除潛客主檔失敗: {masterId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string masterId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ProspectMasters
                WHERE MasterId = @MasterId";

            var count = await QuerySingleAsync<int>(sql, new { MasterId = masterId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查潛客主檔是否存在失敗: {masterId}", ex);
            throw;
        }
    }

    public async Task<bool> HasRelatedProspectsAsync(string masterId)
    {
        try
        {
            // 檢查是否有關聯的潛客資料（如果 Prospects 表存在）
            const string sql = @"
                SELECT COUNT(*) FROM Prospects
                WHERE RecruitId = @MasterId OR MasterId = @MasterId";

            var count = await QuerySingleAsync<int>(sql, new { MasterId = masterId });
            return count > 0;
        }
        catch
        {
            // 如果 Prospects 表不存在，返回 false
            return false;
        }
    }
}

