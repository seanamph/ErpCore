using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 員工餐卡申請服務介面 (SYSL130)
/// </summary>
public interface IEmployeeMealCardService
{
    /// <summary>
    /// 查詢員工餐卡申請列表
    /// </summary>
    Task<PagedResult<EmployeeMealCardDto>> GetMealCardsAsync(EmployeeMealCardQueryDto query);

    /// <summary>
    /// 查詢單筆員工餐卡申請
    /// </summary>
    Task<EmployeeMealCardDto> GetMealCardByIdAsync(long tKey);

    /// <summary>
    /// 新增員工餐卡申請
    /// </summary>
    Task<long> CreateMealCardAsync(CreateEmployeeMealCardDto dto);

    /// <summary>
    /// 修改員工餐卡申請
    /// </summary>
    Task UpdateMealCardAsync(long tKey, UpdateEmployeeMealCardDto dto);

    /// <summary>
    /// 刪除員工餐卡申請
    /// </summary>
    Task DeleteMealCardAsync(long tKey);

    /// <summary>
    /// 批次審核員工餐卡申請
    /// </summary>
    Task<BatchVerifyResultDto> BatchVerifyAsync(BatchVerifyDto dto);

    /// <summary>
    /// 取得下拉選單資料
    /// </summary>
    Task<MealCardDropdownsDto> GetDropdownsAsync();
}

