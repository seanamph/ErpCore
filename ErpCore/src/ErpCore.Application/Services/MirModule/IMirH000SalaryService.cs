using ErpCore.Application.DTOs.MirModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.MirModule;

/// <summary>
/// MIRH000 薪資服務介面
/// </summary>
public interface IMirH000SalaryService
{
    Task<PagedResult<MirH000SalaryDto>> GetSalaryListAsync(MirH000SalaryQueryDto query);
    Task<MirH000SalaryDto> GetSalaryByIdAsync(string salaryId);
    Task<string> CreateSalaryAsync(CreateMirH000SalaryDto dto);
    Task UpdateSalaryAsync(string salaryId, UpdateMirH000SalaryDto dto);
    Task DeleteSalaryAsync(string salaryId);
}

