using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.DropdownList;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 區域服務實作
/// </summary>
public class ZoneService : BaseService, IZoneService
{
    private readonly IZoneRepository _repository;

    public ZoneService(
        IZoneRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<ZoneDto?> GetZoneAsync(string zoneId)
    {
        try
        {
            _logger.LogInfo($"查詢區域: {zoneId}");
            var zone = await _repository.GetByIdAsync(zoneId);
            if (zone == null) return null;

            return new ZoneDto
            {
                ZoneId = zone.ZoneId,
                ZoneName = zone.ZoneName,
                CityId = zone.CityId,
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
            var result = await _repository.QueryAsync(query);

            return new PagedResult<ZoneDto>
            {
                Items = result.Items.Select(z => new ZoneDto
                {
                    ZoneId = z.ZoneId,
                    ZoneName = z.ZoneName,
                    CityId = z.CityId,
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
            return await _repository.GetOptionsAsync(cityId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢區域選項失敗", ex);
            throw;
        }
    }
}

