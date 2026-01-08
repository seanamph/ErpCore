using ErpCore.Application.DTOs.Kiosk;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Kiosk;

/// <summary>
/// Kiosk報表服務介面
/// </summary>
public interface IKioskReportService
{
    /// <summary>
    /// 查詢Kiosk交易記錄
    /// </summary>
    Task<PagedResult<KioskTransactionDto>> GetTransactionsAsync(KioskTransactionQueryDto query);

    /// <summary>
    /// 查詢Kiosk交易統計
    /// </summary>
    Task<List<KioskStatisticsDto>> GetStatisticsAsync(KioskStatisticsQueryDto query);

    /// <summary>
    /// 查詢Kiosk功能代碼統計
    /// </summary>
    Task<List<KioskFunctionStatisticsDto>> GetFunctionStatisticsAsync(KioskStatisticsQueryDto query);

    /// <summary>
    /// 查詢Kiosk錯誤分析
    /// </summary>
    Task<List<KioskErrorAnalysisDto>> GetErrorAnalysisAsync(KioskStatisticsQueryDto query);

    /// <summary>
    /// 匯出Kiosk交易報表
    /// </summary>
    Task<byte[]> ExportReportAsync(KioskReportExportDto dto);
}

/// <summary>
/// Kiosk處理服務介面
/// </summary>
public interface IKioskProcessService
{
    /// <summary>
    /// 處理Kiosk請求
    /// </summary>
    Task<KioskProcessResponseDto> ProcessRequestAsync(KioskProcessRequestDto request);
}

