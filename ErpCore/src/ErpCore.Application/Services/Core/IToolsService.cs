using ErpCore.Application.DTOs.Core;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 工具服務介面
/// 提供 Excel匯出、字串編碼、ASP轉ASPX 等功能
/// </summary>
public interface IToolsService
{
    #region Export_Excel - Excel匯出功能
    
    /// <summary>
    /// 取得匯出設定列表
    /// </summary>
    Task<List<ExcelExportConfigDto>> GetExportConfigsAsync(string moduleCode);
    
    /// <summary>
    /// 匯出Excel
    /// </summary>
    Task<byte[]> ExportExcelAsync(ExcelExportRequestDto request);
    
    /// <summary>
    /// 建立背景匯出任務
    /// </summary>
    Task<string> CreateExportTaskAsync(ExcelExportRequestDto request);
    
    /// <summary>
    /// 查詢匯出任務狀態
    /// </summary>
    Task<ExcelExportTaskDto?> GetExportTaskStatusAsync(string taskId);
    
    /// <summary>
    /// 下載匯出檔案
    /// </summary>
    Task<byte[]?> DownloadExportFileAsync(string taskId);
    
    /// <summary>
    /// 新增匯出設定
    /// </summary>
    Task<long> CreateExportConfigAsync(CreateExcelExportConfigDto dto);
    
    /// <summary>
    /// 修改匯出設定
    /// </summary>
    Task UpdateExportConfigAsync(long configId, CreateExcelExportConfigDto dto);
    
    /// <summary>
    /// 刪除匯出設定
    /// </summary>
    Task DeleteExportConfigAsync(long configId);
    
    #endregion

    #region Encode_String - 字串編碼工具
    
    /// <summary>
    /// 編碼字串
    /// </summary>
    Task<EncodeStringResultDto> EncodeStringAsync(EncodeStringRequestDto request);
    
    /// <summary>
    /// 解碼字串
    /// </summary>
    Task<DecodeStringResultDto> DecodeStringAsync(DecodeStringRequestDto request);
    
    #endregion

    #region ASPXTOASP - ASP轉ASPX工具
    
    /// <summary>
    /// ASPX轉ASP
    /// </summary>
    Task<string> ConvertAspxToAspAsync(PageTransitionDto dto);
    
    /// <summary>
    /// ASP轉ASPX
    /// </summary>
    Task<string> ConvertAspToAspxAsync(PageTransitionDto dto);
    
    /// <summary>
    /// 查詢頁面轉換記錄
    /// </summary>
    Task<PagedResult<PageTransitionLogDto>> GetPageTransitionsAsync(PageTransitionQueryDto query);
    
    /// <summary>
    /// 取得頁面轉換對應設定列表
    /// </summary>
    Task<List<PageTransitionMappingDto>> GetPageTransitionMappingsAsync();
    
    /// <summary>
    /// 新增頁面轉換對應設定
    /// </summary>
    Task<long> CreatePageTransitionMappingAsync(CreatePageTransitionMappingDto dto);
    
    /// <summary>
    /// 修改頁面轉換對應設定
    /// </summary>
    Task UpdatePageTransitionMappingAsync(long mappingId, CreatePageTransitionMappingDto dto);
    
    /// <summary>
    /// 刪除頁面轉換對應設定
    /// </summary>
    Task DeletePageTransitionMappingAsync(long mappingId);
    
    #endregion
}

