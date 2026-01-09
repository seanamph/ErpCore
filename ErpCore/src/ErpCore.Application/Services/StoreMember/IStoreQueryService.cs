using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 商店查詢服務介面 (SYS3210-SYS3299 - 商店查詢作業)
/// </summary>
public interface IStoreQueryService
{
    /// <summary>
    /// 查詢商店列表（進階查詢）
    /// </summary>
    Task<PagedResult<ShopQueryDto>> QueryShopsAsync(ShopQueryRequestDto request);

    /// <summary>
    /// 匯出商店查詢結果
    /// </summary>
    Task<byte[]> ExportShopsAsync(ShopExportRequestDto request);
}

