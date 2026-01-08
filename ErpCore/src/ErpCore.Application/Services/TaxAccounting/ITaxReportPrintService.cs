using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 稅務報表列印服務介面 (SYST510-SYST530)
/// </summary>
public interface ITaxReportPrintService
{
    /// <summary>
    /// 查詢SAP銀行往來資料
    /// </summary>
    Task<PagedResult<SapBankTotalDto>> GetSapBankTotalAsync(SapBankTotalQueryDto query);

    /// <summary>
    /// 產生SAP銀行往來CSV檔案
    /// </summary>
    Task<CsvFileDto> GenerateSapBankTotalCsvAsync(GenerateCsvDto dto);

    /// <summary>
    /// 下載CSV檔案
    /// </summary>
    Task<Stream> DownloadCsvAsync(string fileName);

    /// <summary>
    /// 查詢列印記錄列表
    /// </summary>
    Task<PagedResult<TaxReportPrintDto>> GetPrintLogsAsync(TaxReportPrintQueryDto query);

    /// <summary>
    /// 新增列印記錄
    /// </summary>
    Task<long> CreatePrintLogAsync(CreateTaxReportPrintDto dto);

    /// <summary>
    /// 修改列印記錄
    /// </summary>
    Task UpdatePrintLogAsync(long tKey, UpdateTaxReportPrintDto dto);

    /// <summary>
    /// 刪除列印記錄
    /// </summary>
    Task DeletePrintLogAsync(long tKey);
}

