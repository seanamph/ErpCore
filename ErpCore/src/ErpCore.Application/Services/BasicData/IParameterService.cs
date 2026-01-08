using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 參數服務介面
/// </summary>
public interface IParameterService
{
    /// <summary>
    /// 查詢參數列表
    /// </summary>
    Task<PagedResult<ParameterDto>> GetParametersAsync(ParameterQueryDto query);

    /// <summary>
    /// 查詢單筆參數
    /// </summary>
    Task<ParameterDto> GetParameterAsync(string title, string tag);

    /// <summary>
    /// 根據標題查詢參數列表
    /// </summary>
    Task<List<ParameterDto>> GetParametersByTitleAsync(string title);

    /// <summary>
    /// 新增參數
    /// </summary>
    Task<ParameterKeyDto> CreateParameterAsync(CreateParameterDto dto);

    /// <summary>
    /// 修改參數
    /// </summary>
    Task UpdateParameterAsync(string title, string tag, UpdateParameterDto dto);

    /// <summary>
    /// 刪除參數
    /// </summary>
    Task DeleteParameterAsync(string title, string tag);

    /// <summary>
    /// 批次刪除參數
    /// </summary>
    Task DeleteParametersBatchAsync(BatchDeleteParameterDto dto);

    /// <summary>
    /// 取得參數值
    /// </summary>
    Task<string> GetParameterValueAsync(string title, string tag, string? lang = null);
}

