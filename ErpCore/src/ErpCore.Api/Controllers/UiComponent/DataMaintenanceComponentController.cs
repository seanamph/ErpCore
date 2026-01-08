using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.UiComponent;
using ErpCore.Application.Services.UiComponent;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.UiComponent;

/// <summary>
/// 資料維護UI組件控制器 (IMS30系列)
/// </summary>
[Route("api/v1/ui-components")]
public class DataMaintenanceComponentController : BaseController
{
    private readonly IDataMaintenanceComponentService _service;

    public DataMaintenanceComponentController(
        IDataMaintenanceComponentService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢UI組件列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UIComponentDto>>>> GetComponents([FromQuery] UIComponentQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetComponentsAsync(query);
        }, "查詢UI組件列表失敗");
    }

    /// <summary>
    /// 根據ID查詢UI組件
    /// </summary>
    [HttpGet("{componentId}")]
    public async Task<ActionResult<ApiResponse<UIComponentDto>>> GetComponent(long componentId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetComponentByIdAsync(componentId);
            if (result == null)
            {
                throw new Exception("UI組件不存在");
            }
            return result;
        }, "查詢UI組件失敗");
    }

    /// <summary>
    /// 新增UI組件
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UIComponentDto>>> CreateComponent([FromBody] CreateUIComponentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.CreateComponentAsync(dto);
        }, "新增UI組件失敗");
    }

    /// <summary>
    /// 修改UI組件
    /// </summary>
    [HttpPut("{componentId}")]
    public async Task<ActionResult<ApiResponse<UIComponentDto>>> UpdateComponent(long componentId, [FromBody] UpdateUIComponentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.UpdateComponentAsync(componentId, dto);
        }, "修改UI組件失敗");
    }

    /// <summary>
    /// 刪除UI組件
    /// </summary>
    [HttpDelete("{componentId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteComponent(long componentId)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.DeleteComponentAsync(componentId);
        }, "刪除UI組件失敗");
    }

    /// <summary>
    /// 查詢UI組件使用記錄
    /// </summary>
    [HttpGet("{componentId}/usages")]
    public async Task<ActionResult<ApiResponse<List<UIComponentUsageDto>>>> GetUsages(long componentId)
    {
        return await ExecuteAsync(async () =>
        {
            return await _service.GetUsagesAsync(componentId);
        }, "查詢UI組件使用記錄失敗");
    }
}

