using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 庫別服務介面
/// </summary>
public interface IWarehouseService
{
    /// <summary>
    /// 查詢庫別列表
    /// </summary>
    Task<PagedResult<WarehouseDto>> GetWarehousesAsync(WarehouseQueryDto query);

    /// <summary>
    /// 查詢單筆庫別
    /// </summary>
    Task<WarehouseDto> GetWarehouseByIdAsync(string warehouseId);

    /// <summary>
    /// 新增庫別
    /// </summary>
    Task<string> CreateWarehouseAsync(CreateWarehouseDto dto);

    /// <summary>
    /// 修改庫別
    /// </summary>
    Task UpdateWarehouseAsync(string warehouseId, UpdateWarehouseDto dto);

    /// <summary>
    /// 刪除庫別
    /// </summary>
    Task DeleteWarehouseAsync(string warehouseId);

    /// <summary>
    /// 批次刪除庫別
    /// </summary>
    Task DeleteWarehousesBatchAsync(BatchDeleteWarehouseDto dto);

    /// <summary>
    /// 更新庫別狀態
    /// </summary>
    Task UpdateStatusAsync(string warehouseId, UpdateWarehouseStatusDto dto);
}

