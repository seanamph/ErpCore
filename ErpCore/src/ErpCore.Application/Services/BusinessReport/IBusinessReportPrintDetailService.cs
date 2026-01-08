using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表列印明細服務介面 (SYSL160)
/// </summary>
public interface IBusinessReportPrintDetailService
{
    /// <summary>
    /// 查詢業務報表列印明細列表
    /// </summary>
    Task<PagedResult<BusinessReportPrintDetailDto>> GetBusinessReportPrintDetailsAsync(BusinessReportPrintDetailQueryDto query);

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    Task<BusinessReportPrintDetailDto?> GetBusinessReportPrintDetailByIdAsync(long tKey);

    /// <summary>
    /// 根據 PrintId 查詢明細列表
    /// </summary>
    Task<List<BusinessReportPrintDetailDto>> GetBusinessReportPrintDetailsByPrintIdAsync(long printId);

    /// <summary>
    /// 新增業務報表列印明細
    /// </summary>
    Task<long> CreateBusinessReportPrintDetailAsync(CreateBusinessReportPrintDetailDto dto);

    /// <summary>
    /// 修改業務報表列印明細
    /// </summary>
    Task<bool> UpdateBusinessReportPrintDetailAsync(long tKey, UpdateBusinessReportPrintDetailDto dto);

    /// <summary>
    /// 刪除業務報表列印明細
    /// </summary>
    Task<bool> DeleteBusinessReportPrintDetailAsync(long tKey);

    /// <summary>
    /// 批次處理業務報表列印明細
    /// </summary>
    Task<BatchProcessBusinessReportPrintDetailResultDto> BatchProcessAsync(BatchProcessBusinessReportPrintDetailDto dto);
}

/// <summary>
/// 批次處理業務報表列印明細結果 DTO
/// </summary>
public class BatchProcessBusinessReportPrintDetailResultDto
{
    public int CreateCount { get; set; }
    public int UpdateCount { get; set; }
    public int DeleteCount { get; set; }
    public int FailCount { get; set; }
}

