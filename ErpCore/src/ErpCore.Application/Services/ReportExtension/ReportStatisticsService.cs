using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Infrastructure.Repositories.ReportExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.ReportExtension;

/// <summary>
/// 報表統計服務實作 (SYS7C10, SYS7C30)
/// </summary>
public class ReportStatisticsService : BaseService, IReportStatisticsService
{
    private readonly IReportStatisticRepository _repository;

    public ReportStatisticsService(
        IReportStatisticRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ReportStatisticDto>> GetStatisticsAsync(ReportStatisticQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢報表統計記錄: {query.ReportCode}");

            var repositoryQuery = new ReportStatisticQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ReportCode = query.ReportCode,
                StatisticType = query.StatisticType,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ReportStatisticDto
            {
                StatisticId = x.StatisticId,
                ReportCode = x.ReportCode,
                ReportName = x.ReportName,
                StatisticType = x.StatisticType,
                StatisticDate = x.StatisticDate,
                StatisticValue = x.StatisticValue,
                StatisticData = x.StatisticData,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<ReportStatisticDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表統計記錄失敗", ex);
            throw;
        }
    }

    public async Task<ReportStatisticDto> CreateStatisticAsync(CreateReportStatisticDto dto)
    {
        try
        {
            _logger.LogInfo($"建立報表統計記錄: {dto.ReportCode}");

            var entity = new ReportStatistic
            {
                ReportCode = dto.ReportCode,
                ReportName = dto.ReportName,
                StatisticType = dto.StatisticType,
                StatisticDate = dto.StatisticDate,
                StatisticValue = dto.StatisticValue,
                StatisticData = dto.StatisticData,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);

            return new ReportStatisticDto
            {
                StatisticId = result.StatisticId,
                ReportCode = result.ReportCode,
                ReportName = result.ReportName,
                StatisticType = result.StatisticType,
                StatisticDate = result.StatisticDate,
                StatisticValue = result.StatisticValue,
                StatisticData = result.StatisticData,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立報表統計記錄失敗: {dto.ReportCode}", ex);
            throw;
        }
    }
}

