using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 銷退卡服務介面 (SYSL310)
/// </summary>
public interface IReturnCardService
{
    /// <summary>
    /// 查詢銷退卡列表
    /// </summary>
    Task<PagedResult<ReturnCardDto>> GetReturnCardsAsync(ReturnCardQueryDto query);

    /// <summary>
    /// 根據UUID查詢單筆銷退卡
    /// </summary>
    Task<ReturnCardDto?> GetReturnCardByUuidAsync(Guid uuid);

    /// <summary>
    /// 新增銷退卡
    /// </summary>
    Task<long> CreateReturnCardAsync(CreateReturnCardDto dto);

    /// <summary>
    /// 修改銷退卡
    /// </summary>
    Task UpdateReturnCardAsync(Guid uuid, UpdateReturnCardDto dto);

    /// <summary>
    /// 刪除銷退卡
    /// </summary>
    Task DeleteReturnCardAsync(Guid uuid);
}

