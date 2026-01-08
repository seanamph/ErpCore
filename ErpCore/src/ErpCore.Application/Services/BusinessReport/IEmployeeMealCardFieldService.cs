using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 員餐卡欄位服務介面 (SYSL206/SYSL207)
/// </summary>
public interface IEmployeeMealCardFieldService
{
    /// <summary>
    /// 查詢員餐卡欄位列表
    /// </summary>
    Task<PagedResult<EmployeeMealCardFieldDto>> GetFieldsAsync(EmployeeMealCardFieldQueryDto query);

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    Task<EmployeeMealCardFieldDto?> GetFieldAsync(long tKey);

    /// <summary>
    /// 新增員餐卡欄位
    /// </summary>
    Task<long> CreateFieldAsync(CreateEmployeeMealCardFieldDto dto);

    /// <summary>
    /// 修改員餐卡欄位
    /// </summary>
    Task<bool> UpdateFieldAsync(long tKey, UpdateEmployeeMealCardFieldDto dto);

    /// <summary>
    /// 刪除員餐卡欄位
    /// </summary>
    Task<bool> DeleteFieldAsync(long tKey);

    /// <summary>
    /// 載入上一筆名稱
    /// </summary>
    Task<EmployeeMealCardFieldDto?> GetPreviousFieldAsync(string fieldId);

    /// <summary>
    /// 切換Y/N值
    /// </summary>
    Task<EmployeeMealCardFieldDto> ToggleYnAsync(long tKey, ToggleYnDto dto);
}

