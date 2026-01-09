using System.Data;
using Dapper;
using ErpCore.Domain.Entities.SystemExtensionE;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionE;

/// <summary>
/// 人事 Repository 實作 (SYSPED0 - 人事資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PersonnelRepository : BaseRepository, IPersonnelRepository
{
    public PersonnelRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Personnel?> GetByIdAsync(string personnelId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Personnel 
                WHERE PersonnelId = @PersonnelId";

            return await QueryFirstOrDefaultAsync<Personnel>(sql, new { PersonnelId = personnelId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢人事失敗: {personnelId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Personnel>> QueryAsync(PersonnelQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Personnel
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PersonnelId))
            {
                sql += " AND PersonnelId LIKE @PersonnelId";
                parameters.Add("PersonnelId", $"%{query.PersonnelId}%");
            }

            if (!string.IsNullOrEmpty(query.PersonnelName))
            {
                sql += " AND PersonnelName LIKE @PersonnelName";
                parameters.Add("PersonnelName", $"%{query.PersonnelName}%");
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (!string.IsNullOrEmpty(query.PositionId))
            {
                sql += " AND PositionId = @PositionId";
                parameters.Add("PositionId", query.PositionId);
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
                sql += " ORDER BY CreatedAt DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Personnel>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢人事列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PersonnelQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Personnel
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PersonnelId))
            {
                sql += " AND PersonnelId LIKE @PersonnelId";
                parameters.Add("PersonnelId", $"%{query.PersonnelId}%");
            }

            if (!string.IsNullOrEmpty(query.PersonnelName))
            {
                sql += " AND PersonnelName LIKE @PersonnelName";
                parameters.Add("PersonnelName", $"%{query.PersonnelName}%");
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (!string.IsNullOrEmpty(query.PositionId))
            {
                sql += " AND PositionId = @PositionId";
                parameters.Add("PositionId", query.PositionId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢人事數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(Personnel entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Personnel (
                    PersonnelId, PersonnelName, IdNumber, DepartmentId, PositionId,
                    HireDate, ResignDate, Status, Email, Phone, Address,
                    BirthDate, Gender, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @PersonnelId, @PersonnelName, @IdNumber, @DepartmentId, @PositionId,
                    @HireDate, @ResignDate, @Status, @Email, @Phone, @Address,
                    @BirthDate, @Gender, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            await ExecuteAsync(sql, entity);
            return entity.PersonnelId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增人事失敗: {entity.PersonnelId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Personnel entity)
    {
        try
        {
            const string sql = @"
                UPDATE Personnel SET
                    PersonnelName = @PersonnelName,
                    IdNumber = @IdNumber,
                    DepartmentId = @DepartmentId,
                    PositionId = @PositionId,
                    HireDate = @HireDate,
                    ResignDate = @ResignDate,
                    Status = @Status,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address,
                    BirthDate = @BirthDate,
                    Gender = @Gender,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PersonnelId = @PersonnelId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改人事失敗: {entity.PersonnelId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string personnelId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Personnel 
                WHERE PersonnelId = @PersonnelId";

            await ExecuteAsync(sql, new { PersonnelId = personnelId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除人事失敗: {personnelId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string personnelId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Personnel 
                WHERE PersonnelId = @PersonnelId";

            var count = await ExecuteScalarAsync<int>(sql, new { PersonnelId = personnelId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查人事是否存在失敗: {personnelId}", ex);
            throw;
        }
    }
}

