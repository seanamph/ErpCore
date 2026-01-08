using ErpCore.Application.DTOs.Accounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Accounting;

/// <summary>
/// 資產服務介面 (SYSN310)
/// </summary>
public interface IAssetService
{
    Task<PagedResult<AssetDto>> GetAssetsAsync(AssetQueryDto query);
    Task<AssetDto> GetAssetByIdAsync(string assetId);
    Task<string> CreateAssetAsync(CreateAssetDto dto);
    Task UpdateAssetAsync(string assetId, UpdateAssetDto dto);
    Task DeleteAssetAsync(string assetId);
}

