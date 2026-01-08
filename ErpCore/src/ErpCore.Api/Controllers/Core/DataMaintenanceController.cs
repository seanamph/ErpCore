using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Core;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Core;

/// <summary>
/// 資料維護控制器 (IMS30系列)
/// 提供資料瀏覽、新增、查詢、排序、修改、列印等功能
/// </summary>
[Route("api/v1/core/data-maintenance")]
public class DataMaintenanceController : BaseController
{
    private readonly IDataMaintenanceService _service;

    public DataMaintenanceController(
        IDataMaintenanceService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region IMS30_FB - 資料瀏覽功能

    /// <summary>
    /// 查詢資料列表
    /// </summary>
    [HttpGet("browse/{moduleCode}")]
    public async Task<ActionResult<ApiResponse<PagedResult<Dictionary<string, object>>>>> BrowseData(
        string moduleCode,
        [FromQuery] DataBrowseQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BrowseDataAsync(moduleCode, query);
            return result;
        }, "查詢資料列表失敗");
    }

    /// <summary>
    /// 取得瀏覽設定
    /// </summary>
    [HttpGet("browse/{moduleCode}/config")]
    public async Task<ActionResult<ApiResponse<DataBrowseConfigDto>>> GetBrowseConfig(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBrowseConfigAsync(moduleCode);
            if (result == null)
            {
                throw new Exception($"模組 {moduleCode} 的瀏覽設定不存在");
            }
            return result;
        }, "取得瀏覽設定失敗");
    }

    /// <summary>
    /// 新增瀏覽設定
    /// </summary>
    [HttpPost("browse/config")]
    public async Task<ActionResult<ApiResponse<long>>> CreateBrowseConfig([FromBody] CreateDataBrowseConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateBrowseConfigAsync(dto);
            return result;
        }, "新增瀏覽設定失敗");
    }

    /// <summary>
    /// 修改瀏覽設定
    /// </summary>
    [HttpPut("browse/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateBrowseConfig(
        long configId,
        [FromBody] UpdateDataBrowseConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateBrowseConfigAsync(configId, dto);
        }, "修改瀏覽設定失敗");
    }

    /// <summary>
    /// 刪除瀏覽設定
    /// </summary>
    [HttpDelete("browse/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBrowseConfig(long configId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteBrowseConfigAsync(configId);
        }, "刪除瀏覽設定失敗");
    }

    #endregion

    #region IMS30_FI - 資料新增功能

    /// <summary>
    /// 取得新增設定
    /// </summary>
    [HttpGet("insert/{moduleCode}/config")]
    public async Task<ActionResult<ApiResponse<DataInsertConfigDto>>> GetInsertConfig(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetInsertConfigAsync(moduleCode);
            if (result == null)
            {
                throw new Exception($"模組 {moduleCode} 的新增設定不存在");
            }
            return result;
        }, "取得新增設定失敗");
    }

    /// <summary>
    /// 新增資料
    /// </summary>
    [HttpPost("insert/{moduleCode}")]
    public async Task<ActionResult<ApiResponse<object>>> InsertData(
        string moduleCode,
        [FromBody] Dictionary<string, object> data)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.InsertDataAsync(moduleCode, data);
            return result;
        }, "新增資料失敗");
    }

    /// <summary>
    /// 新增新增設定
    /// </summary>
    [HttpPost("insert/config")]
    public async Task<ActionResult<ApiResponse<long>>> CreateInsertConfig([FromBody] CreateDataInsertConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateInsertConfigAsync(dto);
            return result;
        }, "新增新增設定失敗");
    }

    /// <summary>
    /// 修改新增設定
    /// </summary>
    [HttpPut("insert/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateInsertConfig(
        long configId,
        [FromBody] CreateDataInsertConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateInsertConfigAsync(configId, dto);
        }, "修改新增設定失敗");
    }

    /// <summary>
    /// 刪除新增設定
    /// </summary>
    [HttpDelete("insert/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteInsertConfig(long configId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteInsertConfigAsync(configId);
        }, "刪除新增設定失敗");
    }

    #endregion

    #region IMS30_FQ - 資料查詢功能

    /// <summary>
    /// 查詢資料列表
    /// </summary>
    [HttpGet("query/{moduleCode}")]
    public async Task<ActionResult<ApiResponse<PagedResult<Dictionary<string, object>>>>> QueryData(
        string moduleCode,
        [FromQuery] DataBrowseQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.QueryDataAsync(moduleCode, query);
            return result;
        }, "查詢資料列表失敗");
    }

    /// <summary>
    /// 取得查詢設定
    /// </summary>
    [HttpGet("query/{moduleCode}/config")]
    public async Task<ActionResult<ApiResponse<DataQueryConfigDto>>> GetQueryConfig(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetQueryConfigAsync(moduleCode);
            if (result == null)
            {
                throw new Exception($"模組 {moduleCode} 的查詢設定不存在");
            }
            return result;
        }, "取得查詢設定失敗");
    }

    /// <summary>
    /// 儲存查詢條件
    /// </summary>
    [HttpPost("query/{moduleCode}/save-query")]
    public async Task<ActionResult<ApiResponse<long>>> SaveQuery(
        string moduleCode,
        [FromBody] SaveQueryRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SaveQueryAsync(moduleCode, dto.QueryName, dto.QueryConditions, dto.IsDefault);
            return result;
        }, "儲存查詢條件失敗");
    }

    /// <summary>
    /// 取得儲存的查詢條件列表
    /// </summary>
    [HttpGet("query/{moduleCode}/saved-queries")]
    public async Task<ActionResult<ApiResponse<List<SavedQueryDto>>>> GetSavedQueries(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSavedQueriesAsync(moduleCode);
            return result;
        }, "取得儲存的查詢條件失敗");
    }

    /// <summary>
    /// 刪除儲存的查詢條件
    /// </summary>
    [HttpDelete("query/saved-queries/{queryId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSavedQuery(long queryId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSavedQueryAsync(queryId);
        }, "刪除儲存的查詢條件失敗");
    }

    #endregion

    #region IMS30_FS - 資料排序功能

    /// <summary>
    /// 取得排序設定
    /// </summary>
    [HttpGet("sort/{moduleCode}/config")]
    public async Task<ActionResult<ApiResponse<DataSortConfigDto>>> GetSortConfig(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSortConfigAsync(moduleCode);
            if (result == null)
            {
                throw new Exception($"模組 {moduleCode} 的排序設定不存在");
            }
            return result;
        }, "取得排序設定失敗");
    }

    /// <summary>
    /// 套用排序
    /// </summary>
    [HttpPost("sort/{moduleCode}/apply")]
    public async Task<ActionResult<ApiResponse<object>>> ApplySort(
        string moduleCode,
        [FromBody] ApplySortRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ApplySortAsync(moduleCode, dto.SortRules);
        }, "套用排序失敗");
    }

    /// <summary>
    /// 儲存排序規則
    /// </summary>
    [HttpPost("sort/{moduleCode}/save-sort")]
    public async Task<ActionResult<ApiResponse<long>>> SaveSort(
        string moduleCode,
        [FromBody] SaveSortRequestDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SaveSortAsync(moduleCode, dto.SortName, dto.SortRules, dto.IsDefault);
            return result;
        }, "儲存排序規則失敗");
    }

    /// <summary>
    /// 取得儲存的排序規則列表
    /// </summary>
    [HttpGet("sort/{moduleCode}/saved-sorts")]
    public async Task<ActionResult<ApiResponse<List<SavedSortDto>>>> GetSavedSorts(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSavedSortsAsync(moduleCode);
            return result;
        }, "取得儲存的排序規則失敗");
    }

    /// <summary>
    /// 刪除儲存的排序規則
    /// </summary>
    [HttpDelete("sort/saved-sorts/{sortId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSavedSort(long sortId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteSavedSortAsync(sortId);
        }, "刪除儲存的排序規則失敗");
    }

    #endregion

    #region IMS30_FU - 資料修改功能

    /// <summary>
    /// 查詢單筆資料
    /// </summary>
    [HttpGet("update/{moduleCode}/{id}")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> GetDataForUpdate(
        string moduleCode,
        string id)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetDataForUpdateAsync(moduleCode, id);
            if (result == null)
            {
                throw new Exception("資料不存在");
            }
            return result;
        }, "查詢單筆資料失敗");
    }

    /// <summary>
    /// 取得修改設定
    /// </summary>
    [HttpGet("update/{moduleCode}/config")]
    public async Task<ActionResult<ApiResponse<DataUpdateConfigDto>>> GetUpdateConfig(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetUpdateConfigAsync(moduleCode);
            if (result == null)
            {
                throw new Exception($"模組 {moduleCode} 的修改設定不存在");
            }
            return result;
        }, "取得修改設定失敗");
    }

    /// <summary>
    /// 修改資料
    /// </summary>
    [HttpPut("update/{moduleCode}/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateData(
        string moduleCode,
        string id,
        [FromBody] Dictionary<string, object> data)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateDataAsync(moduleCode, id, data);
        }, "修改資料失敗");
    }

    /// <summary>
    /// 新增修改設定
    /// </summary>
    [HttpPost("update/config")]
    public async Task<ActionResult<ApiResponse<long>>> CreateUpdateConfig([FromBody] DataUpdateConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateUpdateConfigAsync(dto);
            return result;
        }, "新增修改設定失敗");
    }

    /// <summary>
    /// 修改修改設定
    /// </summary>
    [HttpPut("update/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUpdateConfig(
        long configId,
        [FromBody] DataUpdateConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateUpdateConfigAsync(configId, dto);
        }, "修改修改設定失敗");
    }

    /// <summary>
    /// 刪除修改設定
    /// </summary>
    [HttpDelete("update/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUpdateConfig(long configId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteUpdateConfigAsync(configId);
        }, "刪除修改設定失敗");
    }

    #endregion

    #region IMS30_PR - 資料列印功能

    /// <summary>
    /// 取得列印設定列表
    /// </summary>
    [HttpGet("print/{moduleCode}/config")]
    public async Task<ActionResult<ApiResponse<List<DataPrintConfigDto>>>> GetPrintConfigs(string moduleCode)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPrintConfigsAsync(moduleCode);
            return result;
        }, "取得列印設定列表失敗");
    }

    /// <summary>
    /// 列印預覽
    /// </summary>
    [HttpPost("print/{moduleCode}/preview")]
    public async Task<IActionResult> PreviewPrint(
        string moduleCode,
        [FromBody] PrintRequestDto dto)
    {
        try
        {
            var data = await _service.PreviewPrintAsync(moduleCode, dto.ConfigId, dto.Data, dto.Parameters);
            return File(data, "application/pdf", $"preview_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印預覽失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("列印預覽失敗", "PRINT_ERROR"));
        }
    }

    /// <summary>
    /// 列印資料
    /// </summary>
    [HttpPost("print/{moduleCode}/print")]
    public async Task<IActionResult> PrintData(
        string moduleCode,
        [FromBody] PrintRequestDto dto)
    {
        try
        {
            var data = await _service.PrintDataAsync(moduleCode, dto.ConfigId, dto.Data, dto.Parameters);
            return File(data, "application/pdf", $"print_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印資料失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("列印資料失敗", "PRINT_ERROR"));
        }
    }

    /// <summary>
    /// 新增列印設定
    /// </summary>
    [HttpPost("print/config")]
    public async Task<ActionResult<ApiResponse<long>>> CreatePrintConfig([FromBody] DataPrintConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreatePrintConfigAsync(dto);
            return result;
        }, "新增列印設定失敗");
    }

    /// <summary>
    /// 修改列印設定
    /// </summary>
    [HttpPut("print/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePrintConfig(
        long configId,
        [FromBody] DataPrintConfigDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePrintConfigAsync(configId, dto);
        }, "修改列印設定失敗");
    }

    /// <summary>
    /// 刪除列印設定
    /// </summary>
    [HttpDelete("print/config/{configId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePrintConfig(long configId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePrintConfigAsync(configId);
        }, "刪除列印設定失敗");
    }

    #endregion
}

/// <summary>
/// 儲存查詢條件請求 DTO
/// </summary>
public class SaveQueryRequestDto
{
    public string QueryName { get; set; } = string.Empty;
    public string QueryConditions { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

/// <summary>
/// 套用排序請求 DTO
/// </summary>
public class ApplySortRequestDto
{
    public List<SortRuleDto> SortRules { get; set; } = new();
}

/// <summary>
/// 儲存排序規則請求 DTO
/// </summary>
public class SaveSortRequestDto
{
    public string SortName { get; set; } = string.Empty;
    public List<SortRuleDto> SortRules { get; set; } = new();
    public bool IsDefault { get; set; }
}

/// <summary>
/// 列印請求 DTO
/// </summary>
public class PrintRequestDto
{
    public long ConfigId { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public Dictionary<string, object>? Parameters { get; set; }
}

