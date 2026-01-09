using ErpCore.Application.DTOs.SapIntegration;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SapIntegration;

/// <summary>
/// SAP整合服務介面 (TransSAP系列)
/// </summary>
public interface ITransSapService
{
    Task<PagedResult<TransSapDto>> GetTransSapListAsync(TransSapQueryDto query);
    Task<TransSapDto?> GetTransSapByIdAsync(long tKey);
    Task<TransSapDto?> GetTransSapByTransIdAsync(string transId);
    Task<long> CreateTransSapAsync(CreateTransSapDto dto);
    Task UpdateTransSapAsync(long tKey, UpdateTransSapDto dto);
    Task DeleteTransSapAsync(long tKey);
}

