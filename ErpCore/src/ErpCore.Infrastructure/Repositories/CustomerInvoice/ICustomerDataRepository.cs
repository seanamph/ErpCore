using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.CustomerInvoice;

/// <summary>
/// 客戶資料 Repository 介面 (SYS2000 - 客戶資料維護)
/// </summary>
public interface ICustomerDataRepository
{
    /// <summary>
    /// 根據客戶編號查詢客戶
    /// </summary>
    Task<CustomerData?> GetByIdAsync(string customerId);

    /// <summary>
    /// 查詢客戶列表（分頁）
    /// </summary>
    Task<PagedResult<CustomerData>> QueryAsync(CustomerDataQuery query);

    /// <summary>
    /// 查詢客戶總數
    /// </summary>
    Task<int> GetCountAsync(CustomerDataQuery query);

    /// <summary>
    /// 新增客戶
    /// </summary>
    Task<CustomerData> CreateAsync(CustomerData customer, List<CustomerContact>? contacts = null, List<CustomerAddress>? addresses = null, List<CustomerBankAccount>? bankAccounts = null);

    /// <summary>
    /// 修改客戶
    /// </summary>
    Task<CustomerData> UpdateAsync(CustomerData customer, List<CustomerContact>? contacts = null, List<CustomerAddress>? addresses = null, List<CustomerBankAccount>? bankAccounts = null);

    /// <summary>
    /// 刪除客戶（軟刪除，將狀態設為停用）
    /// </summary>
    Task DeleteAsync(string customerId);

    /// <summary>
    /// 檢查客戶編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string customerId);

    /// <summary>
    /// 查詢客戶聯絡人
    /// </summary>
    Task<IEnumerable<CustomerContact>> GetContactsByCustomerIdAsync(string customerId);

    /// <summary>
    /// 查詢客戶地址
    /// </summary>
    Task<IEnumerable<CustomerAddress>> GetAddressesByCustomerIdAsync(string customerId);

    /// <summary>
    /// 查詢客戶銀行帳戶
    /// </summary>
    Task<IEnumerable<CustomerBankAccount>> GetBankAccountsByCustomerIdAsync(string customerId);
}

/// <summary>
/// 客戶資料查詢條件
/// </summary>
public class CustomerDataQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerType { get; set; }
    public string? TaxId { get; set; }
    public string? Status { get; set; }
}

