using System.Data;
using Dapper;
using ErpCore.Domain.Entities.MirModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.MirModule;

/// <summary>
/// MIRH000 薪資 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class MirH000SalaryRepository : BaseRepository, IMirH000SalaryRepository
{
    public MirH000SalaryRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<MirH000Salary?> GetByIdAsync(string salaryId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM MirH000Salaries 
                WHERE SalaryId = @SalaryId";

            return await QueryFirstOrDefaultAsync<MirH000Salary>(sql, new { SalaryId = salaryId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢薪資資料失敗: {salaryId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<MirH000Salary>> QueryAsync(MirH000SalaryQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM MirH000Salaries 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SalaryId))
            {
                sql += " AND SalaryId LIKE @SalaryId";
                parameters.Add("SalaryId", $"%{query.SalaryId}%");
            }

            if (!string.IsNullOrEmpty(query.PersonnelId))
            {
                sql += " AND PersonnelId = @PersonnelId";
                parameters.Add("PersonnelId", query.PersonnelId);
            }

            if (!string.IsNullOrEmpty(query.SalaryMonth))
            {
                sql += " AND SalaryMonth = @SalaryMonth";
                parameters.Add("SalaryMonth", query.SalaryMonth);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY SalaryMonth DESC, SalaryId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var offset = (query.PageIndex - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<MirH000Salary>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢薪資列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(MirH000SalaryQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM MirH000Salaries 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SalaryId))
            {
                sql += " AND SalaryId LIKE @SalaryId";
                parameters.Add("SalaryId", $"%{query.SalaryId}%");
            }

            if (!string.IsNullOrEmpty(query.PersonnelId))
            {
                sql += " AND PersonnelId = @PersonnelId";
                parameters.Add("PersonnelId", query.PersonnelId);
            }

            if (!string.IsNullOrEmpty(query.SalaryMonth))
            {
                sql += " AND SalaryMonth = @SalaryMonth";
                parameters.Add("SalaryMonth", query.SalaryMonth);
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
            _logger.LogError("查詢薪資數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(MirH000Salary entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO MirH000Salaries 
                (SalaryId, PersonnelId, SalaryMonth, BaseSalary, Bonus, TotalSalary, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@SalaryId, @PersonnelId, @SalaryMonth, @BaseSalary, @Bonus, @TotalSalary, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return entity.SalaryId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增薪資資料失敗: {entity.SalaryId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(MirH000Salary entity)
    {
        try
        {
            const string sql = @"
                UPDATE MirH000Salaries 
                SET PersonnelId = @PersonnelId,
                    SalaryMonth = @SalaryMonth,
                    BaseSalary = @BaseSalary,
                    Bonus = @Bonus,
                    TotalSalary = @TotalSalary,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE SalaryId = @SalaryId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改薪資資料失敗: {entity.SalaryId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string salaryId)
    {
        try
        {
            const string sql = @"
                DELETE FROM MirH000Salaries 
                WHERE SalaryId = @SalaryId";

            await ExecuteAsync(sql, new { SalaryId = salaryId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除薪資資料失敗: {salaryId}", ex);
            throw;
        }
    }
}

