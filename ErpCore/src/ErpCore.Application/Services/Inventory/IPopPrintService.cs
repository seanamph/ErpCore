using ErpCore.Application.DTOs.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// POP列印服務介面
/// </summary>
public interface IPopPrintService
{
    /// <summary>
    /// 查詢商品列表（用於列印）
    /// </summary>
    Task<PagedResult<PopPrintProductDto>> GetProductsAsync(PopPrintProductQueryDto query);

    /// <summary>
    /// 產生列印資料
    /// </summary>
    Task<PopPrintDataDto> GeneratePrintDataAsync(GeneratePrintDataDto dto);

    /// <summary>
    /// 執行列印
    /// </summary>
    Task<PrintJobDto> PrintAsync(PrintRequestDto dto);

    /// <summary>
    /// 取得列印設定
    /// </summary>
    Task<PopPrintSettingDto?> GetSettingsAsync(string? shopId, string? version = null);

    /// <summary>
    /// 更新列印設定
    /// </summary>
    Task UpdateSettingsAsync(string? shopId, UpdatePopPrintSettingDto dto);

    /// <summary>
    /// 查詢列印記錄列表
    /// </summary>
    Task<PagedResult<PopPrintLogDto>> GetLogsAsync(PopPrintLogQueryDto query);

    /// <summary>
    /// 匯出Excel
    /// </summary>
    Task<byte[]> ExportExcelAsync(GeneratePrintDataDto dto);
}

