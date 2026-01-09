using ErpCore.Application.DTOs.CommunicationModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CommunicationModule;

/// <summary>
/// 系統通訊設定服務介面
/// </summary>
public interface ISystemCommunicationService
{
    Task<PagedResult<SystemCommunicationDto>> GetSystemCommunicationsAsync(SystemCommunicationQueryDto query);
    Task<SystemCommunicationDto?> GetSystemCommunicationByIdAsync(long communicationId);
    Task<SystemCommunicationDto?> GetSystemCommunicationByCodeAsync(string systemCode);
    Task<long> CreateSystemCommunicationAsync(CreateSystemCommunicationDto dto);
    Task UpdateSystemCommunicationAsync(long communicationId, UpdateSystemCommunicationDto dto);
    Task DeleteSystemCommunicationAsync(long communicationId);
}

/// <summary>
/// XCOM系統參數服務介面
/// </summary>
public interface IXComSystemParamService
{
    Task<PagedResult<XComSystemParamDto>> GetSystemParamsAsync(XComSystemParamQueryDto query);
    Task<XComSystemParamDto?> GetSystemParamByIdAsync(string paramCode);
    Task CreateSystemParamAsync(CreateXComSystemParamDto dto);
    Task UpdateSystemParamAsync(string paramCode, UpdateXComSystemParamDto dto);
    Task DeleteSystemParamAsync(string paramCode);
    Task UpdateStatusAsync(string paramCode, string status);
}

