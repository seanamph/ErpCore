using ErpCore.Application.DTOs.CustomerCustomJgjn;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerCustomJgjn;

/// <summary>
/// JGJN客戶服務介面
/// </summary>
public interface IJgjNCustomerService
{
    Task<PagedResult<JgjNCustomerDto>> GetJgjNCustomerListAsync(JgjNCustomerQueryDto query);
    Task<JgjNCustomerDto?> GetJgjNCustomerByIdAsync(long tKey);
    Task<JgjNCustomerDto?> GetJgjNCustomerByCustomerIdAsync(string customerId);
    Task<long> CreateJgjNCustomerAsync(CreateJgjNCustomerDto dto);
    Task UpdateJgjNCustomerAsync(long tKey, UpdateJgjNCustomerDto dto);
    Task DeleteJgjNCustomerAsync(long tKey);
}

