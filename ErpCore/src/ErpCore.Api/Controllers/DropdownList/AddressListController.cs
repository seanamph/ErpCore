using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.DropdownList;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.DropdownList;

/// <summary>
/// 地址列表控制器 (ADDR_CITY_LIST, ADDR_ZONE_LIST)
/// </summary>
[Route("api/v1/lists/addresses")]
public class AddressListController : BaseController
{
    private readonly ICityService _cityService;
    private readonly IZoneService _zoneService;

    public AddressListController(
        ICityService cityService,
        IZoneService zoneService,
        ILoggerService logger) : base(logger)
    {
        _cityService = cityService;
        _zoneService = zoneService;
    }

    /// <summary>
    /// 查詢城市列表
    /// </summary>
    [HttpGet("cities")]
    public async Task<ActionResult<ApiResponse<PagedResult<CityDto>>>> GetCities(
        [FromQuery] CityQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _cityService.GetCitiesAsync(query);
            return result;
        }, "查詢城市列表失敗");
    }

    /// <summary>
    /// 查詢單筆城市
    /// </summary>
    [HttpGet("cities/{cityId}")]
    public async Task<ActionResult<ApiResponse<CityDto>>> GetCity(string cityId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _cityService.GetCityAsync(cityId);
            if (result == null)
            {
                throw new Exception($"城市不存在: {cityId}");
            }
            return result;
        }, $"查詢城市失敗: {cityId}");
    }

    /// <summary>
    /// 查詢城市選項（用於下拉選單）
    /// </summary>
    [HttpGet("cities/options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CityOptionDto>>>> GetCityOptions(
        [FromQuery] string? countryCode = null,
        [FromQuery] string? status = "1")
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _cityService.GetCityOptionsAsync(countryCode, status);
            return result;
        }, "查詢城市選項失敗");
    }

    /// <summary>
    /// 查詢區域列表
    /// </summary>
    [HttpGet("zones")]
    public async Task<ActionResult<ApiResponse<PagedResult<ZoneDto>>>> GetZones(
        [FromQuery] ZoneQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _zoneService.GetZonesAsync(query);
            return result;
        }, "查詢區域列表失敗");
    }

    /// <summary>
    /// 查詢單筆區域
    /// </summary>
    [HttpGet("zones/{zoneId}")]
    public async Task<ActionResult<ApiResponse<ZoneDto>>> GetZone(string zoneId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _zoneService.GetZoneAsync(zoneId);
            if (result == null)
            {
                throw new Exception($"區域不存在: {zoneId}");
            }
            return result;
        }, $"查詢區域失敗: {zoneId}");
    }

    /// <summary>
    /// 查詢區域選項（用於下拉選單）
    /// </summary>
    [HttpGet("zones/options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ZoneOptionDto>>>> GetZoneOptions(
        [FromQuery] string? cityId = null,
        [FromQuery] string? status = "1")
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _zoneService.GetZoneOptionsAsync(cityId, status);
            return result;
        }, "查詢區域選項失敗");
    }
}

