using ErpCore.Application.DTOs.Core;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 資料維護服務介面 (IMS30系列)
/// </summary>
public interface IDataMaintenanceService
{
    #region IMS30_FB - 資料瀏覽功能
    
    /// <summary>
    /// 查詢資料列表
    /// </summary>
    Task<PagedResult<Dictionary<string, object>>> BrowseDataAsync(string moduleCode, DataBrowseQueryDto query);
    
    /// <summary>
    /// 取得瀏覽設定
    /// </summary>
    Task<DataBrowseConfigDto?> GetBrowseConfigAsync(string moduleCode);
    
    /// <summary>
    /// 新增瀏覽設定
    /// </summary>
    Task<long> CreateBrowseConfigAsync(CreateDataBrowseConfigDto dto);
    
    /// <summary>
    /// 修改瀏覽設定
    /// </summary>
    Task UpdateBrowseConfigAsync(long configId, UpdateDataBrowseConfigDto dto);
    
    /// <summary>
    /// 刪除瀏覽設定
    /// </summary>
    Task DeleteBrowseConfigAsync(long configId);
    
    #endregion

    #region IMS30_FI - 資料新增功能
    
    /// <summary>
    /// 取得新增設定
    /// </summary>
    Task<DataInsertConfigDto?> GetInsertConfigAsync(string moduleCode);
    
    /// <summary>
    /// 新增資料
    /// </summary>
    Task<object> InsertDataAsync(string moduleCode, Dictionary<string, object> data);
    
    /// <summary>
    /// 新增新增設定
    /// </summary>
    Task<long> CreateInsertConfigAsync(CreateDataInsertConfigDto dto);
    
    /// <summary>
    /// 修改新增設定
    /// </summary>
    Task UpdateInsertConfigAsync(long configId, CreateDataInsertConfigDto dto);
    
    /// <summary>
    /// 刪除新增設定
    /// </summary>
    Task DeleteInsertConfigAsync(long configId);
    
    #endregion

    #region IMS30_FQ - 資料查詢功能
    
    /// <summary>
    /// 查詢資料列表
    /// </summary>
    Task<PagedResult<Dictionary<string, object>>> QueryDataAsync(string moduleCode, DataBrowseQueryDto query);
    
    /// <summary>
    /// 取得查詢設定
    /// </summary>
    Task<DataQueryConfigDto?> GetQueryConfigAsync(string moduleCode);
    
    /// <summary>
    /// 儲存查詢條件
    /// </summary>
    Task<long> SaveQueryAsync(string moduleCode, string queryName, string queryConditions, bool isDefault);
    
    /// <summary>
    /// 取得儲存的查詢條件列表
    /// </summary>
    Task<List<SavedQueryDto>> GetSavedQueriesAsync(string moduleCode);
    
    /// <summary>
    /// 刪除儲存的查詢條件
    /// </summary>
    Task DeleteSavedQueryAsync(long queryId);
    
    #endregion

    #region IMS30_FS - 資料排序功能
    
    /// <summary>
    /// 取得排序設定
    /// </summary>
    Task<DataSortConfigDto?> GetSortConfigAsync(string moduleCode);
    
    /// <summary>
    /// 套用排序
    /// </summary>
    Task ApplySortAsync(string moduleCode, List<SortRuleDto> sortRules);
    
    /// <summary>
    /// 儲存排序規則
    /// </summary>
    Task<long> SaveSortAsync(string moduleCode, string sortName, List<SortRuleDto> sortRules, bool isDefault);
    
    /// <summary>
    /// 取得儲存的排序規則列表
    /// </summary>
    Task<List<SavedSortDto>> GetSavedSortsAsync(string moduleCode);
    
    /// <summary>
    /// 刪除儲存的排序規則
    /// </summary>
    Task DeleteSavedSortAsync(long sortId);
    
    #endregion

    #region IMS30_FU - 資料修改功能
    
    /// <summary>
    /// 查詢單筆資料
    /// </summary>
    Task<Dictionary<string, object>?> GetDataForUpdateAsync(string moduleCode, string id);
    
    /// <summary>
    /// 取得修改設定
    /// </summary>
    Task<DataUpdateConfigDto?> GetUpdateConfigAsync(string moduleCode);
    
    /// <summary>
    /// 修改資料
    /// </summary>
    Task UpdateDataAsync(string moduleCode, string id, Dictionary<string, object> data);
    
    /// <summary>
    /// 新增修改設定
    /// </summary>
    Task<long> CreateUpdateConfigAsync(DataUpdateConfigDto dto);
    
    /// <summary>
    /// 修改修改設定
    /// </summary>
    Task UpdateUpdateConfigAsync(long configId, DataUpdateConfigDto dto);
    
    /// <summary>
    /// 刪除修改設定
    /// </summary>
    Task DeleteUpdateConfigAsync(long configId);
    
    #endregion

    #region IMS30_PR - 資料列印功能
    
    /// <summary>
    /// 取得列印設定列表
    /// </summary>
    Task<List<DataPrintConfigDto>> GetPrintConfigsAsync(string moduleCode);
    
    /// <summary>
    /// 列印預覽
    /// </summary>
    Task<byte[]> PreviewPrintAsync(string moduleCode, long configId, Dictionary<string, object> data, Dictionary<string, object>? parameters);
    
    /// <summary>
    /// 列印資料
    /// </summary>
    Task<byte[]> PrintDataAsync(string moduleCode, long configId, Dictionary<string, object> data, Dictionary<string, object>? parameters);
    
    /// <summary>
    /// 新增列印設定
    /// </summary>
    Task<long> CreatePrintConfigAsync(DataPrintConfigDto dto);
    
    /// <summary>
    /// 修改列印設定
    /// </summary>
    Task UpdatePrintConfigAsync(long configId, DataPrintConfigDto dto);
    
    /// <summary>
    /// 刪除列印設定
    /// </summary>
    Task DeletePrintConfigAsync(long configId);
    
    #endregion
}

