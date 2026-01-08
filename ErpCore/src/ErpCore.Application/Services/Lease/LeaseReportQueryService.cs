using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃報表查詢記錄服務實作 (SYSM141-SYSM144)
/// </summary>
public class LeaseReportQueryService : BaseService, ILeaseReportQueryService
{
    private readonly ILeaseReportQueryRepository _repository;

    public LeaseReportQueryService(
        ILeaseReportQueryRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseReportQueryDto>> GetLeaseReportQueriesAsync(LeaseReportQueryQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseReportQueryQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                QueryId = query.QueryId,
                ReportType = query.ReportType,
                QueryName = query.QueryName,
                QueryDateFrom = query.QueryDateFrom,
                QueryDateTo = query.QueryDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseReportQueryDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃報表查詢記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseReportQueryDto> GetLeaseReportQueryByIdAsync(string queryId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(queryId);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃報表查詢記錄不存在: {queryId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃報表查詢記錄失敗: {queryId}", ex);
            throw;
        }
    }

    public async Task<LeaseReportQueryDto> CreateLeaseReportQueryAsync(CreateLeaseReportQueryDto dto)
    {
        try
        {
            var entity = new LeaseReportQuery
            {
                QueryId = dto.QueryId,
                ReportType = dto.ReportType,
                QueryName = dto.QueryName,
                QueryParams = dto.QueryParams,
                QueryResult = dto.QueryResult,
                QueryDate = dto.QueryDate,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增租賃報表查詢記錄成功: {dto.QueryId}");

            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃報表查詢記錄失敗: {dto.QueryId}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseReportQueryAsync(string queryId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(queryId);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃報表查詢記錄不存在: {queryId}");
            }

            await _repository.DeleteAsync(queryId);
            _logger.LogInfo($"刪除租賃報表查詢記錄成功: {queryId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃報表查詢記錄失敗: {queryId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string queryId)
    {
        try
        {
            return await _repository.ExistsAsync(queryId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃報表查詢記錄是否存在失敗: {queryId}", ex);
            throw;
        }
    }

    private LeaseReportQueryDto MapToDto(LeaseReportQuery entity)
    {
        return new LeaseReportQueryDto
        {
            TKey = entity.TKey,
            QueryId = entity.QueryId,
            ReportType = entity.ReportType,
            QueryName = entity.QueryName,
            QueryParams = entity.QueryParams,
            QueryResult = entity.QueryResult,
            QueryDate = entity.QueryDate,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt
        };
    }
}

