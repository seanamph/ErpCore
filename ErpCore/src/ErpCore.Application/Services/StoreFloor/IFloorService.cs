using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 樓層服務介面 (SYS6310-SYS6370 - 樓層資料維護)
/// </summary>
public interface IFloorService
{
    /// <summary>
    /// 查詢樓層列表
    /// </summary>
    Task<PagedResult<FloorDto>> GetFloorsAsync(FloorQueryDto query);

    /// <summary>
    /// 查詢單筆樓層
    /// </summary>
    Task<FloorDto> GetFloorByIdAsync(string floorId);

    /// <summary>
    /// 新增樓層
    /// </summary>
    Task<string> CreateFloorAsync(CreateFloorDto dto);

    /// <summary>
    /// 修改樓層
    /// </summary>
    Task UpdateFloorAsync(string floorId, UpdateFloorDto dto);

    /// <summary>
    /// 刪除樓層
    /// </summary>
    Task DeleteFloorAsync(string floorId);

    /// <summary>
    /// 檢查樓層代碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string floorId);

    /// <summary>
    /// 更新樓層狀態
    /// </summary>
    Task UpdateStatusAsync(string floorId, string status);
}

