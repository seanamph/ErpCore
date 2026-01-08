using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 可管控欄位服務介面 (SYS0510)
/// </summary>
public interface IControllableFieldService
{
    /// <summary>
    /// 查詢可管控欄位列表
    /// </summary>
    Task<PagedResult<ControllableFieldDto>> GetControllableFieldsAsync(ControllableFieldQueryDto query);

    /// <summary>
    /// 查詢可管控欄位（單筆）
    /// </summary>
    Task<ControllableFieldDto?> GetControllableFieldByIdAsync(string fieldId);

    /// <summary>
    /// 根據資料庫和表格查詢欄位列表
    /// </summary>
    Task<List<ControllableFieldDto>> GetControllableFieldsByDbTableAsync(string dbName, string tableName);

    /// <summary>
    /// 新增可管控欄位
    /// </summary>
    Task<ControllableFieldDto> CreateControllableFieldAsync(CreateControllableFieldDto dto);

    /// <summary>
    /// 修改可管控欄位
    /// </summary>
    Task<ControllableFieldDto> UpdateControllableFieldAsync(string fieldId, UpdateControllableFieldDto dto);

    /// <summary>
    /// 刪除可管控欄位
    /// </summary>
    Task DeleteControllableFieldAsync(string fieldId);

    /// <summary>
    /// 批量刪除可管控欄位
    /// </summary>
    Task<BatchOperationResult> BatchDeleteControllableFieldsAsync(List<string> fieldIds);
}

