using ErpCore.Shared.Common;
using CustomerEntity = ErpCore.Domain.Entities.Customer.Customer;
using CustomerContact = ErpCore.Domain.Entities.Customer.CustomerContact;
using CustomerTransaction = ErpCore.Domain.Entities.Customer.CustomerTransaction;
using QueryHistory = ErpCore.Domain.Entities.Customer.QueryHistory;

namespace ErpCore.Infrastructure.Repositories.Customer;

/// <summary>
/// 客戶 Repository 介面
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// 根據客戶編號查詢客戶
    /// </summary>
    Task<CustomerEntity?> GetByIdAsync(string customerId);

    /// <summary>
    /// 根據統一編號查詢客戶
    /// </summary>
    Task<CustomerEntity?> GetByGuiIdAsync(string guiId);

    /// <summary>
    /// 查詢客戶列表（分頁）
    /// </summary>
    Task<PagedResult<CustomerEntity>> QueryAsync(CustomerQuery query);

    /// <summary>
    /// 新增客戶
    /// </summary>
    Task<CustomerEntity> CreateAsync(CustomerEntity customer);

    /// <summary>
    /// 修改客戶
    /// </summary>
    Task<CustomerEntity> UpdateAsync(CustomerEntity customer);

    /// <summary>
    /// 刪除客戶
    /// </summary>
    Task DeleteAsync(string customerId);

    /// <summary>
    /// 檢查客戶編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string customerId);

    /// <summary>
    /// 檢查統一編號是否存在
    /// </summary>
    Task<bool> ExistsByGuiIdAsync(string guiId);

    /// <summary>
    /// 查詢客戶聯絡人列表
    /// </summary>
    Task<List<CustomerContact>> GetContactsByCustomerIdAsync(string customerId);

    /// <summary>
    /// 新增客戶聯絡人
    /// </summary>
    Task<CustomerContact> CreateContactAsync(CustomerContact contact);

    /// <summary>
    /// 刪除客戶聯絡人
    /// </summary>
    Task DeleteContactsByCustomerIdAsync(string customerId);

    /// <summary>
    /// 進階查詢客戶列表 (CUS5120)
    /// </summary>
    Task<PagedResult<CustomerEntity>> AdvancedQueryAsync(CustomerAdvancedQuery query);

    /// <summary>
    /// 快速搜尋客戶 (CUS5120)
    /// </summary>
    Task<List<CustomerEntity>> SearchAsync(string keyword, int limit);

    /// <summary>
    /// 查詢客戶交易記錄 (CUS5120)
    /// </summary>
    Task<PagedResult<CustomerTransaction>> GetTransactionsAsync(CustomerTransactionQuery query);

    /// <summary>
    /// 儲存查詢歷史記錄 (CUS5120)
    /// </summary>
    Task<QueryHistory> SaveQueryHistoryAsync(QueryHistory history);

    /// <summary>
    /// 取得查詢歷史記錄列表 (CUS5120)
    /// </summary>
    Task<List<QueryHistory>> GetQueryHistoryAsync(string userId, string moduleCode);

    /// <summary>
    /// 刪除查詢歷史記錄 (CUS5120)
    /// </summary>
    Task DeleteQueryHistoryAsync(Guid historyId);
}

/// <summary>
/// 客戶查詢條件
/// </summary>
public class CustomerQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? GuiId { get; set; }
    public string? Status { get; set; }
    public string? SalesId { get; set; }
}

/// <summary>
/// 客戶進階查詢條件 (CUS5120)
/// </summary>
public class CustomerAdvancedQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? GuiId { get; set; }
    public string? GuiType { get; set; }
    public string? ContactStaff { get; set; }
    public string? CompTel { get; set; }
    public string? Cell { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? Canton { get; set; }
    public string? SalesId { get; set; }
    public string? Status { get; set; }
    public string? DiscountYn { get; set; }
    public string? MonthlyYn { get; set; }
    public DateTime? TransDateFrom { get; set; }
    public DateTime? TransDateTo { get; set; }
    public decimal? AccAmtFrom { get; set; }
    public decimal? AccAmtTo { get; set; }
}

/// <summary>
/// 客戶交易記錄查詢條件 (CUS5120)
/// </summary>
public class CustomerTransactionQuery
{
    public string CustomerId { get; set; } = string.Empty;
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

