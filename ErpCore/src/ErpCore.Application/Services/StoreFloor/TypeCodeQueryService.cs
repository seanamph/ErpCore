using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 類型代碼查詢服務實作 (SYS6501-SYS6560 - 類型代碼查詢)
/// </summary>
public class TypeCodeQueryService : BaseService, ITypeCodeQueryService
{
    private readonly ITypeCodeRepository _repository;

    public TypeCodeQueryService(
        ITypeCodeRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TypeCodeQueryResultDto>> QueryTypeCodesAsync(TypeCodeQueryRequestDto request)
    {
        try
        {
            _logger.LogInfo("查詢類型代碼列表（進階查詢）");

            var filters = request.Filters ?? new TypeCodeQueryFilters();

            var query = new TypeCodeQuery
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortField = request.SortField,
                SortOrder = request.SortOrder,
                TypeCode = filters.TypeCode,
                TypeName = filters.TypeName,
                Category = filters.Category,
                Status = filters.Status
            };

            var result = await _repository.QueryAsync(query);

            var dtos = result.Items.Select(x => new TypeCodeQueryResultDto
            {
                TKey = x.TKey,
                TypeCode = x.TypeCode,
                TypeName = x.TypeName,
                TypeNameEn = x.TypeNameEn,
                Category = x.Category,
                Status = x.Status,
                UsageCount = 0 // TODO: 實作使用次數統計
            }).ToList();

            return new PagedResult<TypeCodeQueryResultDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢類型代碼列表失敗", ex);
            throw;
        }
    }

    public async Task<TypeCodeStatisticsDto> GetTypeCodeStatisticsAsync(TypeCodeStatisticsRequestDto request)
    {
        try
        {
            _logger.LogInfo("查詢類型代碼統計資訊");

            var query = new TypeCodeQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                Category = request.Category
            };

            var result = await _repository.QueryAsync(query);

            var statistics = new TypeCodeStatisticsDto
            {
                TotalTypeCodes = result.TotalCount,
                ActiveTypeCodes = result.Items.Count(t => t.Status == "A"),
                CategoryStatistics = result.Items
                    .GroupBy(t => t.Category)
                    .Select(g => new TypeCodeCategoryStatisticsDto
                    {
                        TypeCategory = g.Key,
                        Count = g.Count(),
                        UsageCount = 0 // TODO: 實作使用次數統計
                    })
                    .ToList()
            };

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢類型代碼統計資訊失敗", ex);
            throw;
        }
    }
}

