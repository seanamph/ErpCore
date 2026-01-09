using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// POS終端服務介面 (SYS6610-SYS6999 - POS資料維護)
/// </summary>
public interface IPosTerminalService
{
    Task<PagedResult<PosTerminalDto>> GetPosTerminalsAsync(PosTerminalQueryDto query);
    Task<PosTerminalDto> GetPosTerminalByIdAsync(string posTerminalId);
    Task<string> CreatePosTerminalAsync(CreatePosTerminalDto dto);
    Task UpdatePosTerminalAsync(string posTerminalId, UpdatePosTerminalDto dto);
    Task DeletePosTerminalAsync(string posTerminalId);
    Task<bool> ExistsAsync(string posTerminalId);
    Task SyncPosTerminalAsync(string posTerminalId);
}

