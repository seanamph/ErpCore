using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.DropdownList;
using ErpCore.Infrastructure.Repositories.Queries;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 城市服務實作
/// </summary>
public class CityService : BaseService, ICityService
{
    private readonly ICityRepository _repository;

    public CityService(
        ICityRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<CityDto?> GetCityAsync(string cityId)
    {
        try
        {
            _logger.LogInfo($"查詢城市: {cityId}");
            var city = await _repository.GetByIdAsync(cityId);
            if (city == null) return null;

            return new CityDto
            {
                CityId = city.CityId,
                CityName = city.CityName,
                CountryCode = city.CountryCode,
                SeqNo = city.SeqNo,
                Status = city.Status
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢城市失敗: {cityId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CityDto>> GetCitiesAsync(CityQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢城市列表");
            // 將 Application DTO 轉換為 Infrastructure Query
            var repositoryQuery = new CityQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                CityName = query.CityName,
                CountryCode = query.CountryCode,
                Status = query.Status
            };
            var result = await _repository.QueryAsync(repositoryQuery);

            return new PagedResult<CityDto>
            {
                Items = result.Items.Select(c => new CityDto
                {
                    CityId = c.CityId,
                    CityName = c.CityName,
                    CountryCode = c.CountryCode,
                    SeqNo = c.SeqNo,
                    Status = c.Status
                }).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢城市列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<CityOptionDto>> GetCityOptionsAsync(string? countryCode = null, string? status = "1")
    {
        try
        {
            _logger.LogInfo("查詢城市選項");
            var options = await _repository.GetOptionsAsync(countryCode, status);
            // 將 Infrastructure Option 轉換為 Application DTO
            return options.Select(o => new CityOptionDto
            {
                Value = o.Value,
                Label = o.Label
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢城市選項失敗", ex);
            throw;
        }
    }
}

