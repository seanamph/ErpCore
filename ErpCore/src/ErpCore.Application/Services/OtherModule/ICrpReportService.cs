using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.OtherModule;

/// <summary>
/// CRP報表服務介面
/// 提供 Crystal Reports 報表生成、下載等功能
/// </summary>
public interface ICrpReportService
{
    /// <summary>
    /// 取得報表設定列表
    /// </summary>
    Task<List<CrystalReportDto>> GetReportsAsync();

    /// <summary>
    /// 根據報表代碼取得報表設定
    /// </summary>
    Task<CrystalReportDto?> GetReportByCodeAsync(string reportCode);

    /// <summary>
    /// 生成報表
    /// </summary>
    Task<GenerateReportResponseDto> GenerateReportAsync(GenerateReportRequestDto request);

    /// <summary>
    /// 下載報表
    /// </summary>
    Task<byte[]?> DownloadReportAsync(long reportId);

    /// <summary>
    /// 新增報表設定
    /// </summary>
    Task<long> CreateReportAsync(CreateCrystalReportDto dto);

    /// <summary>
    /// 修改報表設定
    /// </summary>
    Task UpdateReportAsync(long reportId, CreateCrystalReportDto dto);

    /// <summary>
    /// 刪除報表設定
    /// </summary>
    Task DeleteReportAsync(long reportId);

    /// <summary>
    /// 查詢操作記錄列表
    /// </summary>
    Task<PagedResult<CrystalReportLogDto>> GetLogsAsync(string? reportCode, int pageIndex, int pageSize);
}

