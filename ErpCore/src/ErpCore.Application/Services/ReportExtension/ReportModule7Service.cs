using System.Text.Json;
using Dapper;
using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Infrastructure.Repositories.ReportExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.ReportExtension;

/// <summary>
/// 報表模組7服務實作 (SYS7000)
/// </summary>
public class ReportModule7Service : BaseService, IReportModule7Service
{
    private readonly IReportQueryRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public ReportModule7Service(
        IReportQueryRepository repository,
        IDbConnectionFactory connectionFactory,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<ReportQueryDto>> GetReportsAsync(ReportQueryListDto query)
    {
        try
        {
            _logger.LogInfo($"查詢報表列表: {query.ReportCode}");

            var repositoryQuery = new ReportQueryListQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ReportCode = query.ReportCode,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ReportQueryDto
            {
                QueryId = x.QueryId,
                ReportCode = x.ReportCode,
                ReportName = x.ReportName,
                QueryName = x.QueryName,
                QueryParams = x.QueryParams,
                QuerySql = x.QuerySql,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ReportQueryDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表列表失敗", ex);
            throw;
        }
    }

    public async Task<ReportQueryResultDto> QueryReportAsync(ReportQueryRequestDto request)
    {
        try
        {
            _logger.LogInfo($"執行報表查詢: {request.ReportCode}");

            var startTime = DateTime.Now;

            // 取得報表查詢設定
            var reportQuery = await _repository.GetByReportCodeAsync(request.ReportCode);
            if (reportQuery == null || string.IsNullOrEmpty(reportQuery.QuerySql))
            {
                throw new Exception($"報表查詢設定不存在或未設定SQL: {request.ReportCode}");
            }

            // 執行查詢
            var sql = reportQuery.QuerySql;
            var parameters = new DynamicParameters();

            if (request.QueryParams != null)
            {
                foreach (var param in request.QueryParams)
                {
                    parameters.Add(param.Key, param.Value);
                }
            }

            using var connection = _connectionFactory.CreateConnection();
            var items = await connection.QueryAsync(sql, parameters);

            var resultList = items.Select(x => (x as IDictionary<string, object>)?.ToDictionary(k => k.Key, v => v.Value) ?? new Dictionary<string, object>()).ToList();

            // 分頁處理
            var totalCount = resultList.Count;
            var pagedItems = resultList
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var executionTime = (int)(DateTime.Now - startTime).TotalMilliseconds;

            // 記錄查詢日誌
            var log = new ReportQueryLog
            {
                LogId = Guid.NewGuid(),
                QueryId = reportQuery.QueryId,
                ReportCode = request.ReportCode,
                UserId = GetCurrentUserId(),
                QueryParams = JsonSerializer.Serialize(request.QueryParams),
                QueryTime = DateTime.Now,
                ExecutionTime = executionTime,
                RecordCount = totalCount,
                Status = "1"
            };
            await _repository.CreateLogAsync(log);

            return new ReportQueryResultDto
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                ExecutionTime = executionTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行報表查詢失敗: {request.ReportCode}", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportReportAsync(string reportCode, ReportQueryRequestDto request, string format)
    {
        try
        {
            _logger.LogInfo($"匯出報表: {reportCode}, 格式: {format}");

            // 執行查詢取得資料
            var queryResult = await QueryReportAsync(request);

            // 根據格式匯出
            if (format.ToUpper() == "EXCEL")
            {
                return await _exportHelper.ExportToExcelAsync(queryResult.Items, $"{reportCode}_報表");
            }
            else if (format.ToUpper() == "PDF")
            {
                return await _exportHelper.ExportToPdfAsync(queryResult.Items, $"{reportCode}_報表");
            }
            else
            {
                throw new Exception($"不支援的匯出格式: {format}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出報表失敗: {reportCode}", ex);
            throw;
        }
    }
}

