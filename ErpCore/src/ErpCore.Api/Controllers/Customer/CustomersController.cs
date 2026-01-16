using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Customer;
using ErpCore.Application.Services.Customer;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Customer;

/// <summary>
/// 客戶基本資料維護作業控制器 (CUS5110)
/// </summary>
[Route("api/v1/customers")]
public class CustomersController : BaseController
{
    private readonly ICustomerService _service;

    public CustomersController(
        ICustomerService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢客戶列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerDto>>>> GetCustomers(
        [FromQuery] CustomerQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetCustomersAsync(query);
            return result;
        }, "查詢客戶列表失敗");
    }

    /// <summary>
    /// 查詢單筆客戶
    /// </summary>
    [HttpGet("{customerId}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomer(string customerId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetCustomerByIdAsync(customerId);
            return result;
        }, $"查詢客戶失敗: {customerId}");
    }

    /// <summary>
    /// 驗證統一編號
    /// </summary>
    [HttpPost("validate-gui-id")]
    public async Task<ActionResult<ApiResponse<GuiIdValidationResultDto>>> ValidateGuiId(
        [FromBody] ValidateGuiIdDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ValidateGuiIdAsync(dto);
            return result;
        }, "驗證統一編號失敗");
    }

    /// <summary>
    /// 新增客戶
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateCustomer(
        [FromBody] CreateCustomerDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateCustomerAsync(dto);
            return result;
        }, "新增客戶失敗");
    }

    /// <summary>
    /// 修改客戶
    /// </summary>
    [HttpPut("{customerId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCustomer(
        string customerId,
        [FromBody] UpdateCustomerDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateCustomerAsync(customerId, dto);
        }, $"修改客戶失敗: {customerId}");
    }

    /// <summary>
    /// 刪除客戶
    /// </summary>
    [HttpDelete("{customerId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCustomer(string customerId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteCustomerAsync(customerId);
        }, $"刪除客戶失敗: {customerId}");
    }

    /// <summary>
    /// 批次刪除客戶
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCustomersBatch(
        [FromBody] BatchDeleteCustomerDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.BatchDeleteCustomersAsync(dto);
        }, "批次刪除客戶失敗");
    }

    /// <summary>
    /// 進階查詢客戶列表 (CUS5120)
    /// </summary>
    [HttpPost("query")]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerDto>>>> AdvancedQuery(
        [FromBody] CustomerAdvancedQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.AdvancedQueryAsync(query);
            return result;
        }, "進階查詢客戶列表失敗");
    }

    /// <summary>
    /// 快速搜尋客戶 (CUS5120)
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<CustomerSearchResultDto>>>> Search(
        [FromQuery] string keyword,
        [FromQuery] int limit = 10)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SearchAsync(new CustomerSearchDto { Keyword = keyword, Limit = limit });
            return result;
        }, "快速搜尋客戶失敗");
    }

    /// <summary>
    /// 查詢客戶交易記錄 (CUS5120)
    /// </summary>
    [HttpGet("{customerId}/transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerTransactionDto>>>> GetTransactions(
        string customerId,
        [FromQuery] CustomerTransactionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            query.CustomerId = customerId;
            var result = await _service.GetTransactionsAsync(query);
            return result;
        }, $"查詢客戶交易記錄失敗: {customerId}");
    }

    /// <summary>
    /// 儲存查詢歷史記錄 (CUS5120)
    /// </summary>
    [HttpPost("query-history")]
    public async Task<ActionResult<ApiResponse<QueryHistoryDto>>> SaveQueryHistory(
        [FromBody] SaveQueryHistoryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.SaveQueryHistoryAsync(dto);
            return result;
        }, "儲存查詢歷史記錄失敗");
    }

    /// <summary>
    /// 取得查詢歷史記錄列表 (CUS5120)
    /// </summary>
    [HttpGet("query-history")]
    public async Task<ActionResult<ApiResponse<List<QueryHistoryDto>>>> GetQueryHistory(
        [FromQuery] string moduleCode = "CUS5120")
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetQueryHistoryAsync(moduleCode);
            return result;
        }, "取得查詢歷史記錄列表失敗");
    }

    /// <summary>
    /// 刪除查詢歷史記錄 (CUS5120)
    /// </summary>
    [HttpDelete("query-history/{historyId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteQueryHistory(Guid historyId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteQueryHistoryAsync(historyId);
        }, $"刪除查詢歷史記錄失敗: {historyId}");
    }

    /// <summary>
    /// 匯出客戶查詢結果到 Excel (CUS5120)
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportToExcel([FromBody] CustomerAdvancedQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportToExcelAsync(query);
            var fileName = $"客戶查詢結果_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出客戶查詢結果到 Excel 失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出客戶查詢結果到 Excel 失敗"));
        }
    }

    /// <summary>
    /// 查詢客戶報表 (CUS5130)
    /// </summary>
    [HttpPost("reports/cus5130")]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerReportDto>>>> GetReport(
        [FromBody] CustomerReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReportAsync(query);
            return result;
        }, "查詢客戶報表失敗");
    }

    /// <summary>
    /// 匯出客戶報表到 Excel (CUS5130)
    /// </summary>
    [HttpPost("reports/cus5130/export/excel")]
    public async Task<IActionResult> ExportReportToExcel(
        [FromBody] CustomerReportQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportReportToExcelAsync(query);
            var fileName = $"客戶報表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出客戶報表到 Excel 失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出客戶報表到 Excel 失敗"));
        }
    }

    /// <summary>
    /// 匯出客戶報表到 PDF (CUS5130)
    /// </summary>
    [HttpPost("reports/cus5130/export/pdf")]
    public async Task<IActionResult> ExportReportToPdf(
        [FromBody] CustomerReportQueryDto query)
    {
        try
        {
            var fileBytes = await _service.ExportReportToPdfAsync(query);
            var fileName = $"客戶報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出客戶報表到 PDF 失敗", ex);
            return BadRequest(ApiResponse<object>.Fail("匯出客戶報表到 PDF 失敗"));
        }
    }
}

