using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.DropdownList;
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
            var result = await _repository.QueryAsync(query);

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
            return await _repository.GetOptionsAsync(countryCode, status);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢城市選項失敗", ex);
            throw;
        }
    }
}

