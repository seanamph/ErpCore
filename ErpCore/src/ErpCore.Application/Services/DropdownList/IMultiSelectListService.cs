using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 多選列表服務介面
/// </summary>
public interface IMultiSelectListService
{
    // 多選區域
    Task<IEnumerable<MultiAreaDto>> GetMultiAreasAsync(MultiAreaQueryDto query);
    Task<IEnumerable<OptionDto>> GetAreaOptionsAsync(string? status = "A");

    // 多選店別
    Task<IEnumerable<MultiShopDto>> GetMultiShopsAsync(MultiShopQueryDto query);
    Task<IEnumerable<OptionDto>> GetShopOptionsAsync(MultiShopQueryDto query);

    // 多選使用者
    Task<PagedResult<MultiUserDto>> GetMultiUsersAsync(MultiUserQueryDto query);
    Task<IEnumerable<OptionDto>> GetUserOptionsAsync(string? orgId = null, string? status = "A", string? keyword = null);
}

