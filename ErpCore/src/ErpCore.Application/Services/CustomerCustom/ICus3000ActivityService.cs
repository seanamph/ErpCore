using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerCustom;

/// <summary>
/// CUS3000 活動服務介面 (SYS3510-SYS3580 - 活動管理)
/// </summary>
public interface ICus3000ActivityService
{
    Task<PagedResult<Cus3000ActivityDto>> GetCus3000ActivityListAsync(Cus3000ActivityQueryDto query);
    Task<Cus3000ActivityDto?> GetCus3000ActivityByIdAsync(long tKey);
    Task<Cus3000ActivityDto?> GetCus3000ActivityByActivityIdAsync(string activityId);
    Task<long> CreateCus3000ActivityAsync(CreateCus3000ActivityDto dto);
    Task UpdateCus3000ActivityAsync(long tKey, UpdateCus3000ActivityDto dto);
    Task DeleteCus3000ActivityAsync(long tKey);
}

