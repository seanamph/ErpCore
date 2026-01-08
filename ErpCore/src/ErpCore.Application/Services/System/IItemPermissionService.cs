using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 項目權限服務介面 (SYS0360)
/// </summary>
public interface IItemPermissionService
{
    /// <summary>
    /// 查詢項目權限列表
    /// </summary>
    Task<PagedResult<ItemPermissionDto>> GetItemPermissionsAsync(string itemId, ItemPermissionQueryDto query);

    /// <summary>
    /// 查詢項目系統列表
    /// </summary>
    Task<List<ItemSystemDto>> GetSystemListAsync(string itemId);

    /// <summary>
    /// 查詢項目選單列表
    /// </summary>
    Task<List<ItemMenuDto>> GetMenuListAsync(string itemId, string systemId);

    /// <summary>
    /// 查詢項目作業列表
    /// </summary>
    Task<List<ItemProgramDto>> GetProgramListAsync(string itemId, string menuId);

    /// <summary>
    /// 查詢項目按鈕列表
    /// </summary>
    Task<List<ItemButtonDto>> GetButtonListAsync(string itemId, string programId);

    /// <summary>
    /// 設定項目系統權限
    /// </summary>
    Task<BatchOperationResult> SetSystemPermissionsAsync(string itemId, SetItemSystemPermissionDto dto);

    /// <summary>
    /// 設定項目選單權限
    /// </summary>
    Task<BatchOperationResult> SetMenuPermissionsAsync(string itemId, SetItemMenuPermissionDto dto);

    /// <summary>
    /// 設定項目作業權限
    /// </summary>
    Task<BatchOperationResult> SetProgramPermissionsAsync(string itemId, SetItemProgramPermissionDto dto);

    /// <summary>
    /// 設定項目按鈕權限
    /// </summary>
    Task<BatchOperationResult> SetButtonPermissionsAsync(string itemId, SetItemButtonPermissionDto dto);

    /// <summary>
    /// 刪除項目權限
    /// </summary>
    Task DeleteItemPermissionAsync(string itemId, long tKey);

    /// <summary>
    /// 批量刪除項目權限
    /// </summary>
    Task<BatchOperationResult> BatchDeleteItemPermissionsAsync(string itemId, List<long> tKeys);
}

