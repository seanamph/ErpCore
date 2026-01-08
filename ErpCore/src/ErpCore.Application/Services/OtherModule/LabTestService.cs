using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.OtherModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Text.Json;

namespace ErpCore.Application.Services.OtherModule;

/// <summary>
/// Lab測試服務實作
/// 提供測試和開發相關功能
/// </summary>
public class LabTestService : BaseService, ILabTestService
{
    private readonly ILabTestRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public LabTestService(
        ILabTestRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<ConnectionTestResponseDto> TestConnectionAsync()
    {
        try
        {
            _logger.LogInfo("執行資料庫連線測試");

            var startTime = DateTime.Now;
            var status = "SUCCESS";
            string? errorMessage = null;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                status = "FAILED";
                errorMessage = ex.Message;
                _logger.LogError("資料庫連線測試失敗", ex);
            }

            var duration = (int)(DateTime.Now - startTime).TotalMilliseconds;

            return new ConnectionTestResponseDto
            {
                Status = status,
                Duration = duration,
                ErrorMessage = errorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("資料庫連線測試失敗", ex);
            throw;
        }
    }

    public async Task<ExecuteTestResponseDto> ExecuteTestAsync(ExecuteTestRequestDto request)
    {
        try
        {
            _logger.LogInfo($"執行測試: {request.TestName} ({request.TestType})");

            var startTime = DateTime.Now;
            var testDataJson = request.TestData != null ? JsonSerializer.Serialize(request.TestData) : null;
            var status = "SUCCESS";
            string? testResultJson = null;
            string? errorMessage = null;

            try
            {
                // 根據測試類型執行不同的測試
                switch (request.TestType.ToUpper())
                {
                    case "CONNECTION":
                        var connectionTest = await TestConnectionAsync();
                        testResultJson = JsonSerializer.Serialize(connectionTest);
                        status = connectionTest.Status;
                        errorMessage = connectionTest.ErrorMessage;
                        break;
                    case "FUNCTION":
                        // 功能測試邏輯
                        testResultJson = "{\"message\": \"功能測試完成\"}";
                        break;
                    case "PERFORMANCE":
                        // 效能測試邏輯
                        testResultJson = "{\"message\": \"效能測試完成\"}";
                        break;
                    default:
                        throw new InvalidOperationException($"不支援的測試類型: {request.TestType}");
                }
            }
            catch (Exception ex)
            {
                status = "FAILED";
                errorMessage = ex.Message;
                _logger.LogError($"執行測試失敗: {request.TestName}", ex);
            }

            var duration = (int)(DateTime.Now - startTime).TotalMilliseconds;

            // 儲存測試結果
            var testResult = new TestResult
            {
                TestName = request.TestName,
                TestType = request.TestType,
                TestData = testDataJson,
                TestResult = testResultJson,
                Status = status,
                ErrorMessage = errorMessage,
                Duration = duration,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(testResult);

            return new ExecuteTestResponseDto
            {
                TestId = result.TestId,
                Status = status,
                TestResult = testResultJson,
                ErrorMessage = errorMessage,
                Duration = duration
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行測試失敗: {request.TestName}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TestResultDto>> GetTestResultsAsync(TestResultQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢測試結果列表");

            var repositoryQuery = new TestResultQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TestType = query.TestType,
                Status = query.Status,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _repository.QueryAsync(repositoryQuery);
            return new PagedResult<TestResultDto>
            {
                Items = result.Items.Select(t => new TestResultDto
                {
                    TestId = t.TestId,
                    TestName = t.TestName,
                    TestType = t.TestType,
                    TestData = t.TestData,
                    TestResult = t.TestResult,
                    Status = t.Status,
                    ErrorMessage = t.ErrorMessage,
                    Duration = t.Duration,
                    CreatedBy = t.CreatedBy,
                    CreatedAt = t.CreatedAt
                }).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢測試結果列表失敗", ex);
            throw;
        }
    }

    public async Task<TestResultDto?> GetTestResultByIdAsync(long testId)
    {
        try
        {
            _logger.LogInfo($"取得測試結果: {testId}");

            var testResult = await _repository.GetByIdAsync(testId);
            if (testResult == null)
            {
                return null;
            }

            return new TestResultDto
            {
                TestId = testResult.TestId,
                TestName = testResult.TestName,
                TestType = testResult.TestType,
                TestData = testResult.TestData,
                TestResult = testResult.TestResult,
                Status = testResult.Status,
                ErrorMessage = testResult.ErrorMessage,
                Duration = testResult.Duration,
                CreatedBy = testResult.CreatedBy,
                CreatedAt = testResult.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得測試結果失敗: {testId}", ex);
            throw;
        }
    }

    public async Task DeleteTestResultAsync(long testId)
    {
        try
        {
            _logger.LogInfo($"刪除測試結果: {testId}");

            await _repository.DeleteAsync(testId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除測試結果失敗: {testId}", ex);
            throw;
        }
    }
}

