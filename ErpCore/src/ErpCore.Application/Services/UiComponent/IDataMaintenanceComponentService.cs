using ErpCore.Application.DTOs.UiComponent;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.UiComponent;

/// <summary>
/// 資料維護UI組件服務介面
/// </summary>
public interface IDataMaintenanceComponentService
{
    /// <summary>
    /// 查詢UI組件列表
    /// </summary>
    Task<PagedResult<UIComponentDto>> GetComponentsAsync(UIComponentQueryDto query);

    /// <summary>
    /// 根據ID查詢UI組件
    /// </summary>
    Task<UIComponentDto?> GetComponentByIdAsync(long componentId);

    /// <summary>
    /// 新增UI組件
    /// </summary>
    Task<UIComponentDto> CreateComponentAsync(CreateUIComponentDto dto);

    /// <summary>
    /// 修改UI組件
    /// </summary>
    Task<UIComponentDto> UpdateComponentAsync(long componentId, UpdateUIComponentDto dto);

    /// <summary>
    /// 刪除UI組件
    /// </summary>
    Task<bool> DeleteComponentAsync(long componentId);

    /// <summary>
    /// 查詢UI組件使用記錄
    /// </summary>
    Task<List<UIComponentUsageDto>> GetUsagesAsync(long componentId);
}

