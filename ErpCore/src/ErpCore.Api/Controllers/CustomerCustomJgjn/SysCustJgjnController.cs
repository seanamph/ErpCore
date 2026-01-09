using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.CustomerCustomJgjn;
using ErpCore.Application.Services.CustomerCustomJgjn;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.CustomerCustomJgjn;

/// <summary>
/// SYSCUST_JGJN 客戶定制JGJN模組控制器
/// </summary>
[Route("api/v1/jgjn")]
public class SysCustJgjnController : BaseController
{
    private readonly IJgjNDataService _dataService;
    private readonly IJgjNCustomerService _customerService;
    private readonly IJgjNInvoiceService _invoiceService;

    public SysCustJgjnController(
        IJgjNDataService dataService,
        IJgjNCustomerService customerService,
        IJgjNInvoiceService invoiceService,
        ILoggerService logger) : base(logger)
    {
        _dataService = dataService;
        _customerService = customerService;
        _invoiceService = invoiceService;
    }

    #region 資料管理 (SYS1610-SYS1646)

    /// <summary>
    /// 查詢資料列表
    /// </summary>
    [HttpGet("data")]
    public async Task<ActionResult<ApiResponse<PagedResult<JgjNDataDto>>>> GetJgjNDataList(
        [FromQuery] JgjNDataQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dataService.GetJgjNDataListAsync(query);
            return result;
        }, "查詢資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆資料
    /// </summary>
    [HttpGet("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<JgjNDataDto>>> GetJgjNDataById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dataService.GetJgjNDataByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }
            return result;
        }, "查詢資料失敗");
    }

    /// <summary>
    /// 新增資料
    /// </summary>
    [HttpPost("data")]
    public async Task<ActionResult<ApiResponse<long>>> CreateJgjNData([FromBody] CreateJgjNDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _dataService.CreateJgjNDataAsync(dto);
            return result;
        }, "新增資料失敗");
    }

    /// <summary>
    /// 修改資料
    /// </summary>
    [HttpPut("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateJgjNData(long tKey, [FromBody] UpdateJgjNDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _dataService.UpdateJgjNDataAsync(tKey, dto);
            return true;
        }, "修改資料失敗");
    }

    /// <summary>
    /// 刪除資料
    /// </summary>
    [HttpDelete("data/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteJgjNData(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _dataService.DeleteJgjNDataAsync(tKey);
            return true;
        }, "刪除資料失敗");
    }

    #endregion

    #region 客戶管理 (SYSC210)

    /// <summary>
    /// 查詢客戶列表
    /// </summary>
    [HttpGet("customers")]
    public async Task<ActionResult<ApiResponse<PagedResult<JgjNCustomerDto>>>> GetJgjNCustomerList(
        [FromQuery] JgjNCustomerQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _customerService.GetJgjNCustomerListAsync(query);
            return result;
        }, "查詢客戶列表失敗");
    }

    /// <summary>
    /// 查詢單筆客戶
    /// </summary>
    [HttpGet("customers/{tKey}")]
    public async Task<ActionResult<ApiResponse<JgjNCustomerDto>>> GetJgjNCustomerById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _customerService.GetJgjNCustomerByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"客戶不存在: {tKey}");
            }
            return result;
        }, "查詢客戶失敗");
    }

    /// <summary>
    /// 新增客戶
    /// </summary>
    [HttpPost("customers")]
    public async Task<ActionResult<ApiResponse<long>>> CreateJgjNCustomer([FromBody] CreateJgjNCustomerDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _customerService.CreateJgjNCustomerAsync(dto);
            return result;
        }, "新增客戶失敗");
    }

    /// <summary>
    /// 修改客戶
    /// </summary>
    [HttpPut("customers/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateJgjNCustomer(long tKey, [FromBody] UpdateJgjNCustomerDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _customerService.UpdateJgjNCustomerAsync(tKey, dto);
            return true;
        }, "修改客戶失敗");
    }

    /// <summary>
    /// 刪除客戶
    /// </summary>
    [HttpDelete("customers/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteJgjNCustomer(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _customerService.DeleteJgjNCustomerAsync(tKey);
            return true;
        }, "刪除客戶失敗");
    }

    #endregion

    #region 發票管理 (PnInvoice)

    /// <summary>
    /// 查詢發票列表
    /// </summary>
    [HttpGet("invoices")]
    public async Task<ActionResult<ApiResponse<PagedResult<JgjNInvoiceDto>>>> GetJgjNInvoiceList(
        [FromQuery] JgjNInvoiceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _invoiceService.GetJgjNInvoiceListAsync(query);
            return result;
        }, "查詢發票列表失敗");
    }

    /// <summary>
    /// 查詢單筆發票
    /// </summary>
    [HttpGet("invoices/{tKey}")]
    public async Task<ActionResult<ApiResponse<JgjNInvoiceDto>>> GetJgjNInvoiceById(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _invoiceService.GetJgjNInvoiceByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"發票不存在: {tKey}");
            }
            return result;
        }, "查詢發票失敗");
    }

    /// <summary>
    /// 新增發票
    /// </summary>
    [HttpPost("invoices")]
    public async Task<ActionResult<ApiResponse<long>>> CreateJgjNInvoice([FromBody] CreateJgjNInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _invoiceService.CreateJgjNInvoiceAsync(dto);
            return result;
        }, "新增發票失敗");
    }

    /// <summary>
    /// 修改發票
    /// </summary>
    [HttpPut("invoices/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateJgjNInvoice(long tKey, [FromBody] UpdateJgjNInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _invoiceService.UpdateJgjNInvoiceAsync(tKey, dto);
            return true;
        }, "修改發票失敗");
    }

    /// <summary>
    /// 刪除發票
    /// </summary>
    [HttpDelete("invoices/{tKey}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteJgjNInvoice(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _invoiceService.DeleteJgjNInvoiceAsync(tKey);
            return true;
        }, "刪除發票失敗");
    }

    /// <summary>
    /// 列印發票
    /// </summary>
    [HttpPost("invoices/{tKey}/print")]
    public async Task<ActionResult<ApiResponse<bool>>> PrintJgjNInvoice(long tKey, [FromBody] PrintJgjNInvoiceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _invoiceService.PrintJgjNInvoiceAsync(tKey, dto);
            return true;
        }, "列印發票失敗");
    }

    #endregion
}

