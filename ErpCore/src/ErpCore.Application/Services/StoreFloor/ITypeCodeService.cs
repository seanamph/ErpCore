using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 類型代碼服務介面 (SYS6405-SYS6490 - 類型代碼維護)
/// </summary>
public interface ITypeCodeService
{
    /// <summary>
    /// 查詢類型代碼列表
    /// </summary>
    Task<PagedResult<TypeCodeDto>> GetTypeCodesAsync(TypeCodeQueryDto query);

    /// <summary>
    /// 查詢單筆類型代碼
    /// </summary>
    Task<TypeCodeDto> GetTypeCodeByIdAsync(long tKey);

    /// <summary>
    /// 新增類型代碼
    /// </summary>
    Task<long> CreateTypeCodeAsync(CreateTypeCodeDto dto);

    /// <summary>
    /// 修改類型代碼
    /// </summary>
    Task UpdateTypeCodeAsync(long tKey, UpdateTypeCodeDto dto);

    /// <summary>
    /// 刪除類型代碼
    /// </summary>
    Task DeleteTypeCodeAsync(long tKey);

    /// <summary>
    /// 批次刪除類型代碼
    /// </summary>
    Task BatchDeleteTypeCodesAsync(List<long> tKeys);

    /// <summary>
    /// 檢查類型代碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string typeCode, string? category);
}

