using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 系統作業服務介面 (SYS0430)
/// </summary>
public interface IProgramService
{
    /// <summary>
    /// 查詢作業列表
    /// </summary>
    Task<PagedResult<ProgramDto>> GetProgramsAsync(ProgramQueryDto query);

    /// <summary>
    /// 查詢單筆作業
    /// </summary>
    Task<ProgramDto> GetProgramAsync(string programId);

    /// <summary>
    /// 新增作業
    /// </summary>
    Task<string> CreateProgramAsync(CreateProgramDto dto);

    /// <summary>
    /// 修改作業
    /// </summary>
    Task UpdateProgramAsync(string programId, UpdateProgramDto dto);

    /// <summary>
    /// 刪除作業
    /// </summary>
    Task DeleteProgramAsync(string programId);

    /// <summary>
    /// 批次刪除作業
    /// </summary>
    Task DeleteProgramsBatchAsync(BatchDeleteProgramsDto dto);
}
