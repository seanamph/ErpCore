using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表管理服務介面 (SYSL145)
/// </summary>
public interface IBusinessReportManagementService
{
    /// <summary>
    /// 查詢業務報表管理列表
    /// </summary>
    Task<PagedResult<BusinessReportManagementDto>> GetBusinessReportManagementsAsync(BusinessReportManagementQueryDto query);

    /// <summary>
    /// 查詢單筆業務報表管理
    /// </summary>
    Task<BusinessReportManagementDto> GetBusinessReportManagementByIdAsync(long tKey);

    /// <summary>
    /// 新增業務報表管理
    /// </summary>
    Task<long> CreateBusinessReportManagementAsync(CreateBusinessReportManagementDto dto);

    /// <summary>
    /// 修改業務報表管理
    /// </summary>
    Task UpdateBusinessReportManagementAsync(long tKey, UpdateBusinessReportManagementDto dto);

    /// <summary>
    /// 刪除業務報表管理
    /// </summary>
    Task DeleteBusinessReportManagementAsync(long tKey);

    /// <summary>
    /// 批次刪除業務報表管理
    /// </summary>
    Task<int> BatchDeleteBusinessReportManagementAsync(List<long> tKeys);

    /// <summary>
    /// 載入管理權限資料
    /// </summary>
    Task<List<BusinessReportManagementDto>> LoadManagementDataAsync();

    /// <summary>
    /// 檢查重複資料
    /// </summary>
    Task<CheckDuplicateResultDto> CheckDuplicateAsync(CheckDuplicateDto dto);
}

