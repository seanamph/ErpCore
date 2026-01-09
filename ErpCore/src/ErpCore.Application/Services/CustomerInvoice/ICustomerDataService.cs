using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 客戶資料服務介面 (SYS2000 - 客戶資料維護)
/// </summary>
public interface ICustomerDataService
{
    /// <summary>
    /// 查詢客戶列表
    /// </summary>
    Task<PagedResult<CustomerDataDto>> GetCustomersAsync(CustomerDataQueryDto query);

    /// <summary>
    /// 查詢單筆客戶
    /// </summary>
    Task<CustomerDataDto> GetCustomerByIdAsync(string customerId);

    /// <summary>
    /// 新增客戶
    /// </summary>
    Task<string> CreateCustomerAsync(CreateCustomerDataDto dto);

    /// <summary>
    /// 修改客戶
    /// </summary>
    Task UpdateCustomerAsync(string customerId, UpdateCustomerDataDto dto);

    /// <summary>
    /// 刪除客戶
    /// </summary>
    Task DeleteCustomerAsync(string customerId);

    /// <summary>
    /// 批次刪除客戶
    /// </summary>
    Task BatchDeleteCustomersAsync(BatchDeleteCustomerDataDto dto);

    /// <summary>
    /// 檢查客戶編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string customerId);
}

