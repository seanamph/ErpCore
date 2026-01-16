using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 部別服務介面
/// </summary>
public interface IDepartmentService
{
    /// <summary>
    /// 查詢部別列表
    /// </summary>
    Task<PagedResult<DepartmentDto>> GetDepartmentsAsync(DepartmentQueryDto query);

    /// <summary>
    /// 查詢單筆部別
    /// </summary>
    Task<DepartmentDto> GetDepartmentAsync(string deptId);

    /// <summary>
    /// 新增部別
    /// </summary>
    Task<string> CreateDepartmentAsync(CreateDepartmentDto dto);

    /// <summary>
    /// 修改部別
    /// </summary>
    Task UpdateDepartmentAsync(string deptId, UpdateDepartmentDto dto);

    /// <summary>
    /// 刪除部別
    /// </summary>
    Task DeleteDepartmentAsync(string deptId);

    /// <summary>
    /// 批次刪除部別
    /// </summary>
    Task DeleteDepartmentsBatchAsync(BatchDeleteDepartmentDto dto);

    /// <summary>
    /// 更新部別狀態
    /// </summary>
    Task UpdateStatusAsync(string deptId, string status);
}
