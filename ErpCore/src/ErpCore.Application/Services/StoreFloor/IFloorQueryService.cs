using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 樓層查詢服務介面 (SYS6381-SYS63A0 - 樓層查詢作業)
/// </summary>
public interface IFloorQueryService
{
    /// <summary>
    /// 查詢樓層列表（進階查詢）
    /// </summary>
    Task<PagedResult<FloorQueryDto>> QueryFloorsAsync(FloorQueryRequestDto request);

    /// <summary>
    /// 查詢樓層統計資訊
    /// </summary>
    Task<FloorStatisticsDto> GetFloorStatisticsAsync(FloorStatisticsRequestDto request);

    /// <summary>
    /// 匯出樓層查詢結果
    /// </summary>
    Task<byte[]> ExportFloorsAsync(FloorExportDto dto);
}

