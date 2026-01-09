using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// POS查詢服務介面 (SYS6A04-SYS6A19 - POS查詢作業)
/// </summary>
public interface IPosTerminalQueryService
{
    Task<PagedResult<PosTerminalDto>> QueryPosTerminalsAsync(PosTerminalQueryRequestDto request);
    Task<PosTerminalStatisticsDto> GetPosTerminalStatisticsAsync(PosTerminalStatisticsRequestDto request);
}

