using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Infrastructure.Repositories.DropdownList;
using ErpCore.Infrastructure.Repositories.Queries;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 區域服務實作
/// </summary>
public class ZoneService : BaseService, IZoneService
{
    private readonly IZoneRepository _repository;
    private readonly ICityRepository _cityRepository;

    public ZoneService(
        IZoneRepository repository,
        ICityRepository cityRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _cityRepository = cityRepository;
    }

    public async Task<ZoneDto?> GetZoneAsync(string zoneId)
    {
        try
        {
            _logger.LogInfo($"查詢區域: {zoneId}");
            var zone = await _repository.GetByIdAsync(zoneId);
            if (zone == null) return null;

            string? cityName = null;
            if (!string.IsNullOrEmpty(zone.CityId))
            {
                var city = await _cityRepository.GetByIdAsync(zone.CityId);
                cityName = city?.CityName;
            }

            return new ZoneDto
            {
                ZoneId = zone.ZoneId,
                ZoneName = zone.ZoneName,
                CityId = zone.CityId,
                CityName = cityName,
                ZipCode = zone.ZipCode,
                SeqNo = zone.SeqNo,
                Status = zone.Status
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢區域失敗: {zoneId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ZoneDto>> GetZonesAsync(ZoneQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢區域列表");
            // 將 Application DTO 轉換為 Infrastructure Query
            var repositoryQuery = new ZoneQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ZoneName = query.ZoneName,
                CityId = query.CityId,
                ZipCode = query.ZipCode,
                Status = query.Status
            };
            var result = await _repository.QueryAsync(repositoryQuery);

            // 批量查詢城市名稱
            var cityIds = result.Items.Where(z => !string.IsNullOrEmpty(z.CityId))
                .Select(z => z.CityId!)
                .Distinct()
                .ToList();

            var cities = new Dictionary<string, string>();
            foreach (var cityId in cityIds)
            {
                var city = await _cityRepository.GetByIdAsync(cityId);
                if (city != null)
                {
                    cities[cityId] = city.CityName;
                }
            }

            return new PagedResult<ZoneDto>
            {
                Items = result.Items.Select(z => new ZoneDto
                {
                    ZoneId = z.ZoneId,
                    ZoneName = z.ZoneName,
                    CityId = z.CityId,
                    CityName = !string.IsNullOrEmpty(z.CityId) && cities.ContainsKey(z.CityId) 
                        ? cities[z.CityId] 
                        : null,
                    ZipCode = z.ZipCode,
                    SeqNo = z.SeqNo,
                    Status = z.Status
                }).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢區域列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ZoneOptionDto>> GetZoneOptionsAsync(string? cityId = null, string? status = "1")
    {
        try
        {
            _logger.LogInfo("查詢區域選項");
            var options = await _repository.GetOptionsAsync(cityId, status);
            // 將 Infrastructure Option 轉換為 Application DTO
            return options.Select(o => new ZoneOptionDto
            {
                Value = o.Value,
                Label = o.Label,
                ZipCode = o.ZipCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢區域選項失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ZoneDto>> GetZonesByCityIdAsync(string cityId)
    {
        try
        {
            _logger.LogInfo($"查詢城市區域: {cityId}");
            var zones = await _repository.GetByCityIdAsync(cityId);
            
            var city = await _cityRepository.GetByIdAsync(cityId);
            var cityName = city?.CityName;

            return zones.Select(z => new ZoneDto
            {
                ZoneId = z.ZoneId,
                ZoneName = z.ZoneName,
                CityId = z.CityId,
                CityName = cityName,
                ZipCode = z.ZipCode,
                SeqNo = z.SeqNo,
                Status = z.Status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢城市區域失敗: {cityId}", ex);
            throw;
        }
    }

    public async Task CreateZoneAsync(CreateZoneDto dto)
    {
        try
        {
            _logger.LogInfo($"新增區域: {dto.ZoneId}");

            // 檢查區域代碼是否已存在
            var existing = await _repository.GetByIdAsync(dto.ZoneId);
            if (existing != null)
            {
                throw new Exception($"區域代碼已存在: {dto.ZoneId}");
            }

            // 檢查城市是否存在
            if (!string.IsNullOrEmpty(dto.CityId))
            {
                var city = await _cityRepository.GetByIdAsync(dto.CityId);
                if (city == null)
                {
                    throw new Exception($"城市不存在: {dto.CityId}");
                }
            }

            var zone = new Zone
            {
                ZoneId = dto.ZoneId,
                ZoneName = dto.ZoneName,
                CityId = dto.CityId,
                ZipCode = dto.ZipCode,
                SeqNo = dto.SeqNo,
                Status = dto.Status,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await _repository.CreateAsync(zone);
            if (!success)
            {
                throw new Exception("新增區域失敗");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增區域失敗: {dto.ZoneId}", ex);
            throw;
        }
    }

    public async Task UpdateZoneAsync(UpdateZoneDto dto)
    {
        try
        {
            _logger.LogInfo($"更新區域: {dto.ZoneId}");

            var existing = await _repository.GetByIdAsync(dto.ZoneId);
            if (existing == null)
            {
                throw new Exception($"區域不存在: {dto.ZoneId}");
            }

            // 檢查城市是否存在
            if (!string.IsNullOrEmpty(dto.CityId))
            {
                var city = await _cityRepository.GetByIdAsync(dto.CityId);
                if (city == null)
                {
                    throw new Exception($"城市不存在: {dto.CityId}");
                }
            }

            var zone = new Zone
            {
                ZoneId = dto.ZoneId,
                ZoneName = dto.ZoneName,
                CityId = dto.CityId,
                ZipCode = dto.ZipCode,
                SeqNo = dto.SeqNo,
                Status = dto.Status,
                CreatedBy = existing.CreatedBy,
                CreatedAt = existing.CreatedAt,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await _repository.UpdateAsync(zone);
            if (!success)
            {
                throw new Exception("更新區域失敗");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新區域失敗: {dto.ZoneId}", ex);
            throw;
        }
    }

    public async Task DeleteZoneAsync(string zoneId)
    {
        try
        {
            _logger.LogInfo($"刪除區域: {zoneId}");

            var existing = await _repository.GetByIdAsync(zoneId);
            if (existing == null)
            {
                throw new Exception($"區域不存在: {zoneId}");
            }

            // TODO: 檢查是否有關聯資料

            var success = await _repository.DeleteAsync(zoneId);
            if (!success)
            {
                throw new Exception("刪除區域失敗");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除區域失敗: {zoneId}", ex);
            throw;
        }
    }
}

