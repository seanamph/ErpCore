using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 類型代碼查詢服務介面 (SYS6501-SYS6560 - 類型代碼查詢)
/// </summary>
public interface ITypeCodeQueryService
{
    /// <summary>
    /// 查詢類型代碼列表（進階查詢）
    /// </summary>
    Task<PagedResult<TypeCodeQueryResultDto>> QueryTypeCodesAsync(TypeCodeQueryRequestDto request);

    /// <summary>
    /// 查詢類型代碼統計資訊
    /// </summary>
    Task<TypeCodeStatisticsDto> GetTypeCodeStatisticsAsync(TypeCodeStatisticsRequestDto request);
}

