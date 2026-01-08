using Dapper;
using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.OtherModule;

/// <summary>
/// Lab測試 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LabTestRepository : BaseRepository, ILabTestRepository
{
    public LabTestRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TestResult?> GetByIdAsync(long testId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TestResults 
                WHERE TestId = @TestId";

            return await QueryFirstOrDefaultAsync<TestResult>(sql, new { TestId = testId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢測試結果失敗: {testId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TestResult>> QueryAsync(TestResultQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM TestResults
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TestType))
            {
                sql += " AND TestType = @TestType";
                parameters.Add("TestType", query.TestType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND CreatedAt >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND CreatedAt <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            sql += " ORDER BY CreatedAt DESC";

            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<TestResult>(sql, parameters);

            var countSql = @"
                SELECT COUNT(*) FROM TestResults
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.TestType))
            {
                countSql += " AND TestType = @TestType";
                countParameters.Add("TestType", query.TestType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND CreatedAt >= @StartDate";
                countParameters.Add("StartDate", query.StartDate.Value);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND CreatedAt <= @EndDate";
                countParameters.Add("EndDate", query.EndDate.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<TestResult>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢測試結果列表失敗", ex);
            throw;
        }
    }

    public async Task<TestResult> CreateAsync(TestResult testResult)
    {
        try
        {
            const string sql = @"
                INSERT INTO TestResults (
                    TestName, TestType, TestData, TestResult, Status, ErrorMessage, Duration,
                    CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @TestName, @TestType, @TestData, @TestResult, @Status, @ErrorMessage, @Duration,
                    @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<TestResult>(sql, testResult);
            if (result == null)
            {
                throw new InvalidOperationException("新增測試結果失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增測試結果失敗: {testResult.TestName}", ex);
            throw;
        }
    }

    public async Task<TestResult> UpdateAsync(TestResult testResult)
    {
        try
        {
            const string sql = @"
                UPDATE TestResults SET
                    TestResult = @TestResult,
                    Status = @Status,
                    ErrorMessage = @ErrorMessage,
                    Duration = @Duration
                OUTPUT INSERTED.*
                WHERE TestId = @TestId";

            var result = await QueryFirstOrDefaultAsync<TestResult>(sql, testResult);
            if (result == null)
            {
                throw new InvalidOperationException($"測試結果不存在: {testResult.TestId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改測試結果失敗: {testResult.TestId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long testId)
    {
        try
        {
            const string sql = @"
                DELETE FROM TestResults 
                WHERE TestId = @TestId";

            await ExecuteAsync(sql, new { TestId = testId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除測試結果失敗: {testId}", ex);
            throw;
        }
    }
}

