using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表列印服務介面 (SYSL150)
/// </summary>
public interface IBusinessReportPrintService
{
    /// <summary>
    /// 查詢業務報表列印列表
    /// </summary>
    Task<PagedResult<BusinessReportPrintDto>> GetBusinessReportPrintsAsync(BusinessReportPrintQueryDto query);

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    Task<BusinessReportPrintDto?> GetBusinessReportPrintByIdAsync(long tKey);

    /// <summary>
    /// 新增業務報表列印
    /// </summary>
    Task<long> CreateBusinessReportPrintAsync(CreateBusinessReportPrintDto dto);

    /// <summary>
    /// 修改業務報表列印
    /// </summary>
    Task<bool> UpdateBusinessReportPrintAsync(long tKey, UpdateBusinessReportPrintDto dto);

    /// <summary>
    /// 刪除業務報表列印
    /// </summary>
    Task<bool> DeleteBusinessReportPrintAsync(long tKey);

    /// <summary>
    /// 批次審核
    /// </summary>
    Task<BatchAuditResultDto> BatchAuditAsync(BatchAuditBusinessReportPrintDto dto);

    /// <summary>
    /// 複製下一年度資料
    /// </summary>
    Task<CopyResultDto> CopyNextYearAsync(CopyNextYearDto dto);

    /// <summary>
    /// 計算數量
    /// </summary>
    Task<CalculateQtyResultDto> CalculateQtyAsync(CalculateQtyDto dto);
}

