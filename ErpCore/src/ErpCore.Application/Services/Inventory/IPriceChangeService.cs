using ErpCore.Application.DTOs.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 變價單服務介面
/// </summary>
public interface IPriceChangeService
{
    /// <summary>
    /// 查詢變價單列表
    /// </summary>
    Task<PagedResult<PriceChangeDto>> GetPriceChangesAsync(PriceChangeQueryDto query);

    /// <summary>
    /// 查詢單筆變價單（含明細）
    /// </summary>
    Task<PriceChangeDetailDto> GetPriceChangeByIdAsync(string priceChangeId, string priceChangeType);

    /// <summary>
    /// 新增變價單
    /// </summary>
    Task<string> CreatePriceChangeAsync(CreatePriceChangeDto dto);

    /// <summary>
    /// 修改變價單
    /// </summary>
    Task UpdatePriceChangeAsync(string priceChangeId, string priceChangeType, UpdatePriceChangeDto dto);

    /// <summary>
    /// 刪除變價單
    /// </summary>
    Task DeletePriceChangeAsync(string priceChangeId, string priceChangeType);

    /// <summary>
    /// 審核變價單
    /// </summary>
    Task ApprovePriceChangeAsync(string priceChangeId, string priceChangeType, ApprovePriceChangeDto dto);

    /// <summary>
    /// 確認變價單
    /// </summary>
    Task ConfirmPriceChangeAsync(string priceChangeId, string priceChangeType, ConfirmPriceChangeDto dto);

    /// <summary>
    /// 作廢變價單
    /// </summary>
    Task CancelPriceChangeAsync(string priceChangeId, string priceChangeType);
}

