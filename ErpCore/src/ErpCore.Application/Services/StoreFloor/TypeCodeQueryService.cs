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

            var dtos = new List<TypeCodeQueryResultDto>();
            foreach (var x in result.Items)
            {
                var usageCount = await _repository.GetUsageCountAsync(x.TypeCode, x.Category);
                dtos.Add(new TypeCodeQueryResultDto
                {
                    TKey = x.TKey,
                    TypeCode = x.TypeCode,
                    TypeName = x.TypeName,
                    TypeNameEn = x.TypeNameEn,
                    Category = x.Category,
                    Status = x.Status,
                    UsageCount = usageCount
                });
            }

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
                CategoryStatistics = new List<TypeCodeCategoryStatisticsDto>()
            };

            // 計算各分類的使用次數
            var categoryGroups = result.Items.GroupBy(t => t.Category);
            foreach (var group in categoryGroups)
            {
                var totalUsageCount = 0;
                foreach (var item in group)
                {
                    totalUsageCount += await _repository.GetUsageCountAsync(item.TypeCode, item.Category);
                }

                statistics.CategoryStatistics.Add(new TypeCodeCategoryStatisticsDto
                {
                    TypeCategory = group.Key,
                    Count = group.Count(),
                    UsageCount = totalUsageCount
                });
            }

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢類型代碼統計資訊失敗", ex);
            throw;
        }
    }
}

