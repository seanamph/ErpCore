using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 區域服務介面
/// </summary>
public interface IAreaService
{
    /// <summary>
    /// 查詢區域列表
    /// </summary>
    Task<PagedResult<AreaDto>> GetAreasAsync(AreaQueryDto query);

    /// <summary>
    /// 查詢單筆區域
    /// </summary>
    Task<AreaDto> GetAreaByIdAsync(string areaId);

    /// <summary>
    /// 新增區域
    /// </summary>
    Task<string> CreateAreaAsync(CreateAreaDto dto);

    /// <summary>
    /// 修改區域
    /// </summary>
    Task UpdateAreaAsync(string areaId, UpdateAreaDto dto);

    /// <summary>
    /// 刪除區域
    /// </summary>
    Task DeleteAreaAsync(string areaId);

    /// <summary>
    /// 批次刪除區域
    /// </summary>
    Task DeleteAreasBatchAsync(BatchDeleteAreaDto dto);

    /// <summary>
    /// 更新區域狀態
    /// </summary>
    Task UpdateStatusAsync(string areaId, string status);
}

