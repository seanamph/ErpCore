using ErpCore.Application.DTOs.SystemExtensionH;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SystemExtensionH;

/// <summary>
/// 系統擴展PH服務介面 (SYSPH00 - 感應卡維護作業)
/// </summary>
public interface ISystemExtensionPHService
{
    Task<PagedResult<EmpCardDto>> GetEmpCardsAsync(EmpCardQueryDto query);
    Task<EmpCardDto> GetEmpCardByIdAsync(long tKey);
    Task<long> CreateEmpCardAsync(CreateEmpCardDto dto);
    Task<int> CreateBatchEmpCardsAsync(CreateBatchEmpCardDto dto);
    Task UpdateEmpCardAsync(long tKey, UpdateEmpCardDto dto);
    Task DeleteEmpCardAsync(long tKey);
}

